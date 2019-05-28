using DotNetExamples.StreamBuffer.Program.Data;
using DotNetExamples.StreamBuffer.Program.Generators;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DotNetExamples.StreamBuffer.Program
{

    /// <summary>
    /// Main execution program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main execution thread.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                // load defaults
                Action command = BuildCommand(
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Capacity")),
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("FillCount")),
                    default(BufferType),
                    default(TestType),
                    args
                );
                command();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid configuration setting.");
                Console.Clear();
            }
        }

        /// <summary>
        /// Print program usage - commannd options.
        /// </summary>
        static void DisplayHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\t-?,--help print this help message.");
            Console.WriteLine("\t-f,--fill <#> Number of times to fill the buffer during testing.");
            Console.WriteLine("\t-c,--capacity <#> Set the capacity of stream buffer to #.");
            Console.WriteLine("\t-t,--type [Message|Transaction|VesselLocation] Perform test type (default=\"{0}\").", default(TestType));
            Console.WriteLine("\t-b,--buffer [SimpleArray|CircularArray|LinkedList] Buffer type to use (default=\"{0}\").", default(BufferType));
        }

        /// <summary>
        /// Parse arg input and builds application command to execute.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="fillCount"></param>
        /// <param name="bufferType"></param>
        /// <param name="testType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Action BuildCommand(int capacity, int fillCount, BufferType bufferType, TestType testType, string[] args)
        {
            // initialize local variables
            List<string> errorList = new List<string>();

            // parse args.
            for (int i = 0; i < args.Length; i += 2)
            {
                int j = i + 1;
                switch (args[i].ToLower(CultureInfo.InvariantCulture))
                {
                    case "-t":
                    case "--type":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing type value");
                        }
                        else if (!Enum.TryParse<TestType>(args[j], out testType))
                        {
                            errorList.Add(String.Format("Invalid test type specified: '{0}'.", args[j]));
                        }
                        break;


                    case "-b":
                    case "--buffer":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing buffer value");
                        }
                        else if (!Enum.TryParse<BufferType>(args[j], out bufferType))
                        {
                            errorList.Add(String.Format("Invalid buffer type specified: '{0}'.", args[j]));
                        }
                        break;

                    case "-c":
                    case "--capacity":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing capacity value");
                        }
                        else if (!int.TryParse(args[j], out capacity))
                        {
                            errorList.Add(String.Format("Invalid capacity specified: '{0}'", args[j]));
                        }
                        break;

                    case "-f":
                    case "--fill":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing fill count value");
                        }
                        else if (!int.TryParse(args[j], out fillCount))
                        {
                            errorList.Add(String.Format("Invalid fill count specified: '{0}'", args[j]));
                        }
                        break;

                    case "-?":
                    case "-h":
                    case "--help":
                        return () => DisplayHelp();

                    default:
                        errorList.Add(String.Format("Invalid command line option specified: '{0}'", args[i]));
                        break;
                }
            }

            if (0 == errorList.Count)
            {
                // Save settings
                return () => Run(capacity, fillCount, testType, bufferType);
            }

            return () =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nCommand-line options in error:");
                foreach (string message in errorList)
                {
                    Console.WriteLine("\t{0}", message);
                }
                Console.ResetColor();
                DisplayHelp();
            };
        }

        /// <summary>
        /// Create test instance from user input.
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="bufferType"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        static IStreamTest CreateTest(TestType testType, BufferType bufferType, int capacity)
        {
            if (TestType.VesselLocation == testType)
            {
                StreamTest<VesselLocation> streamTest = new StreamTest<VesselLocation>(CreateBuffer<VesselLocation>(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Vessels.Names").Split(','))
                {
                    streamTest.Register(new VesselLocationGenerator(name));
                }
                return streamTest;
            }
            else if (TestType.Transaction == testType)
            {
                StreamTest<Transaction> streamTest = new StreamTest<Transaction>(CreateBuffer<Transaction>(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Transaction.Names").Split(','))
                {
                    streamTest.Register(new TransactionGenerator(name));
                }
                return streamTest;
            }
            else if (TestType.Message == testType)
            {
                StreamTest<string> streamTest = new StreamTest<string>(CreateBuffer<string>(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Message.Names").Split(','))
                {
                    streamTest.Register(new MessageGenerator(name));
                }
                return streamTest;
            }
            throw new ArgumentException(String.Format("Invalid test type: \"{0}\"", testType));
        }

        /// <summary>
        /// Generate a stream test using a factory design method.
        /// </summary>
        /// <param name="bufferType"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        static IStreamBuffer<T> CreateBuffer<T>(BufferType bufferType, int capacity)
            where T : IEquatable<T>
        {
            if (BufferType.LinkedList == bufferType)
            {
                return new LinkedListBuffer<T>(capacity);
            }

            if (BufferType.CircularArray == bufferType)
            {
                return new CircularArrayBuffer<T>(capacity);
            }

            if (BufferType.SimpleArray == bufferType)
            {
                return new ArrayBuffer<T>(capacity);
            }

            throw new ArgumentException(String.Format("Invalid buffer type: \"{0}\"", bufferType));
        }

        /// <summary>
        /// Run program.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="fillCount"></param>
        /// <param name="testType"></param>
        /// <param name="bufferType"></param>
        static void Run(int capacity, int fillCount, TestType testType, BufferType bufferType)
        {
            // Display program execution options before starting.
            Console.WriteLine("[{0}] pi capacity={1}", DateTime.Now.ToFileTime(), capacity);
            Console.WriteLine("[{0}] pi fill count={1}", DateTime.Now.ToFileTime(), fillCount);
            Console.WriteLine("[{0}] pi test type={1}", DateTime.Now.ToFileTime(), testType);
            Console.WriteLine("[{0}] pi buffer type={1}", DateTime.Now.ToFileTime(), bufferType);
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

            // Create a stream test instance.
            IStreamTest streamTest = CreateTest(testType, bufferType, capacity);
            foreach (string name in streamTest.GeneratorNames)
            {
                Console.WriteLine("[{0}] gr generator={1}", DateTime.Now.ToFileTime(), name);
            }

            // Run stream test (due to the reliance of generic tipes it is pushed down).
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);
            streamTest.Run(fillCount);
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);
        }
    }
}
