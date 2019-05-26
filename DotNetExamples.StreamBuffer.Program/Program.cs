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
        /// The buffer's max capacity.
        /// </summary>
        protected static int Capacity;

        /// <summary>
        /// Number of times to fill buffer during testing.
        /// </summary>
        protected static int FillCount;

        /// <summary>
        /// Thread count for concurrency testing.
        /// </summary>
        protected static int TaskCount;

        /// <summary>
        /// Main execution thread.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                // Load config file
                Capacity = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Program.Capacity"));
                FillCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Program.FillCount"));
                TaskCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Program.TaskCount"));

                // Initialize local variables
                bool executeProgram = true;
                int capacity = Capacity;
                int fillCount = FillCount;
                BufferType bufferType = default(BufferType);
                TestType testType = default(TestType);

                // Parse command-line input if exists.
                for (int i = 0; i < args.Length; i += 2)
                {
                    List<string> errorList = new List<string>();
                    switch (args[i].ToLower(CultureInfo.InvariantCulture))
                    {
                        case "-t":
                        case "--type":
                            if (!Enum.TryParse<TestType>(args[i + 1], out testType))
                            {
                                errorList.Add(String.Format("Invalid test type specified: '{0}'.", args[i + 1]));
                            }
                            break;


                        case "-b":
                        case "--buffer":
                            if (!Enum.TryParse<BufferType>(args[i + 1], out bufferType))
                            {
                                errorList.Add(String.Format("Invalid buffer type specified: '{0}'.", args[i + 1]));
                            }
                            break;

                        case "-c":
                        case "--capacity":
                            if (!int.TryParse(args[i + 1], out capacity))
                            {
                                errorList.Add(String.Format("Invalid capacity specified: '{0}'", args[i + 1]));
                            }
                            break;

                        case "-f":
                        case "--fill":
                            if (!int.TryParse(args[i + 1], out fillCount))
                            {
                                errorList.Add(String.Format("Invalid fill count specified: '{0}'", args[i + 1]));
                            }
                            break;

                        case "-?":
                        case "--help":
                            DisplayHelp();
                            executeProgram = false;
                            break;

                        default:
                            errorList.Add(String.Format("Invalid command line option specified: '{0}'", args[i]));
                            break;
                    }

                    if (0 < errorList.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nCommand-line options in error:");
                        foreach (string message in errorList)
                        {
                            Console.WriteLine("\t{0}", message);
                        }
                        Console.ResetColor();
                        DisplayHelp();
                        executeProgram = false;
                    }
                }

                if (executeProgram)
                {
                    // Display program execution options before starting.
                    Console.WriteLine("[{0}] pi capacity={1}", DateTime.Now.ToFileTime(), capacity);
                    Console.WriteLine("[{0}] pi fill count={1}", DateTime.Now.ToFileTime(), fillCount);
                    Console.WriteLine("[{0}] pi test type={1}", DateTime.Now.ToFileTime(), testType);
                    Console.WriteLine("[{0}] pi buffer type={1}", DateTime.Now.ToFileTime(), bufferType);
                    Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

                    // Create a stream test instance.
                    IStreamTest streamTest = StreamTest.Create(testType, bufferType, capacity);
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

            // Missing config value
            catch (ArgumentNullException)
            {
                DisplayError("Incorrect configuration settings.");
            }

            // Incorrect configuration value
            catch (FormatException)
            {
                DisplayError("Invalid configuration settings.");
            }
        }

        /// <summary>
        /// Display error message.
        /// </summary>
        /// <param name="message"></param>
        static protected void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Print program usage - commannd options.
        /// </summary>
        protected static void DisplayHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\t-?,--help print this help message.");
            Console.WriteLine("\t-f,--fill <#> Number of times to fill the buffer during testing (default={0}).", FillCount);
            Console.WriteLine("\t-c,--capacity <#> Set the capacity of stream buffer to # (default={0}).", Capacity);
            Console.WriteLine("\t-t,--type [Message|Transaction|VesselLocation] Perform test type (default=\"{0}\").", default(TestType));
            Console.WriteLine("\t-b,--buffer [SimpleArray|CircularArray|LinkedList] Buffer type to use (default=\"{0}\").", default(BufferType));
        }
    }
}
