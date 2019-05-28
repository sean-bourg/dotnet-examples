using DotNetExamples.DocumentManagment;
using DotNetExamples.DocumentManagment.Devices;
using DotNetExamples.DocumentManagment.Schedulers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetExamples.DocumentManagement.Program
{
    /// <summary>
    /// Main execution program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                string deviceFolder = System.Configuration.ConfigurationManager.AppSettings.Get("DeviceFolder");
                if (!Directory.Exists(deviceFolder))
                {
                    throw new DirectoryNotFoundException(String.Format("Invalid configuration: device folder {0}", deviceFolder));
                }

                Action command = BuildCommand(
                    default(SchedulerType),
                    deviceFolder,
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("FileCount")),
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("TaskCount")),
                    args
                );
                command();
            }

            // Catch invalid folder error.
            catch (DirectoryNotFoundException exception)
            {
                DisplayError(exception.Message);
            }

            // Handle invalid input value.
            catch (InvalidOperationException exception)
            {
                DisplayError(exception.Message);
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
        /// Load command line options and return executable.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Action BuildCommand(SchedulerType schedulerType, string deviceFolder, int fileCount, int taskCount, string[] args)
        {
            List<string> errorList = new List<string>();
            for (int i = 0; i < args.Length; i += 2)
            {
                int j = i + 1;
                switch (args[i].ToLower())
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        return () => DisplayHelp();

                    case "-d":
                    case "--devices":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing devive folder");
                        }
                        else if (!Directory.Exists(args[j]))
                        {
                            errorList.Add(String.Format("Invalid device folder : '{0}'", args[j]));
                        }
                        else
                        {
                            deviceFolder = args[j];
                        }
                        break;

                    case "-c":
                    case "--count":
                        if (j > args.Length)
                        {
                            errorList.Add("Missing file value");
                        }
                        else if (!int.TryParse(args[j], out fileCount))
                        {
                            errorList.Add(String.Format("Invalid file count specified: '{0}'", args[j]));
                        }
                        break;

                    case "-t":
                    case "--type":

                        if (j > args.Length)
                        {
                            errorList.Add("Missing value for test type ");
                        }
                        else if (!Enum.TryParse<SchedulerType>(args[j], out schedulerType))
                        {
                            errorList.Add(String.Format("Invalid test type specified: '{0}'.", args[j]));
                        }
                        break;

                    default:
                        errorList.Add(String.Format("Invalid option {0}", args[i]));
                        i = args.Length;
                        break;
                }
            }

            if (0 < errorList.Count())
            {
                return () =>
                {
                    DisplayError(errorList.ToArray());
                    DisplayHelp();
                };
            }
            return () => Run(schedulerType, deviceFolder, fileCount, taskCount);
        }


        /// <summary>
        /// Display help message.
        /// </summary>
        static protected void DisplayHelp()
        {
            Console.WriteLine("Options: path_to_device [[-t|--type RoundRobin|Priority] [-c|--count #]]");
            Console.WriteLine("\t-?,--help print this help message.");
            Console.WriteLine("\t-c,--count <#> Set the number of files to be used in testing.");
            Console.WriteLine("\t-t,--type Select scheduler type [RoundRobin|Priority] (default=\"{0}\").", default(SchedulerType));
        }

        /// <summary>
        /// Display error message.
        /// </summary>
        /// <param name="message"></param>
        static protected void DisplayError(string message) => DisplayError(new string[] { message });

        /// <summary>
        /// Display error message.
        /// </summary>
        /// <param name="messages"></param>
        static protected void DisplayError(string[] messages)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (string msg in messages)
            {
                Console.WriteLine(msg);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Scan folder to buld a list of device folders. Alternates between chunked and normal folder devices.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="latency"></param>
        /// <returns></returns>
        static IDevice[] GetDevices(DirectoryInfo directoryInfo)
        {
            Latency latency = new Latency(
                int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Latency.Min")),
                int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Latency.Max"))
            );

            return directoryInfo.GetDirectories()
                .Select((directory, index) => (0 == (index % 2)) ? new FolderDevice(directory, latency) : new ChunkFolderDevice(directory, latency))
                .ToArray();
        }

        /// <summary>
        /// Get file generator and load defaults from config file.
        /// </summary>
        /// <returns></returns>
        static FileGenerator GetFileGenerator()
        {
            FileGenerator fileGenerator = new FileGenerator(
                System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.Dictionary").Split(','),
                new Random()
            );
            fileGenerator.MinWords = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinWords"));
            fileGenerator.MaxWords = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxWords"));
            fileGenerator.MinSentences = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinSentences"));
            fileGenerator.MaxSentences = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxSentences"));
            fileGenerator.MinParagraphs = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinParagraphs"));
            fileGenerator.MaxParagraphs = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxParagraphs"));
            return fileGenerator;
        }

        /// <summary>
        /// Run test program.
        /// </summary>
        /// <param name="program"></param>
        static void Run(SchedulerType schedulerType, string deviceFolder, int fileCount, int taskCount)
        {
            // Report program options
            Console.WriteLine("[{0}] pi scheduler type = {1}", DateTime.Now.ToFileTime(), schedulerType);
            Console.WriteLine("[{0}] pi device folder = {1}", DateTime.Now.ToFileTime(), deviceFolder);
            Console.WriteLine("[{0}] pi file count = {1}", DateTime.Now.ToFileTime(), fileCount);
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

            FileGenerator fileGenerator = GetFileGenerator();
            IWriteScheduler writeScheduler = CreateWriteScheduler(schedulerType);
            foreach (IDevice device in GetDevices(new DirectoryInfo(deviceFolder)))
            {
                Console.WriteLine("[{0}] pi register device {1}", DateTime.Now.ToFileTime(), device.Id);
                writeScheduler.Register(device);
            }


            // Test using sequential writes.
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);
            DisplayScheduler(writeScheduler);
            foreach (Tuple<string, byte[]> current in fileGenerator.GetFiles(fileCount))
            {
                writeScheduler.Write(current.Item1, current.Item2);
                DisplayScheduler(writeScheduler);
            }
            Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

            // Test using concurrent writes.
            Task[] tasks = new Task[taskCount];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    foreach (Tuple<string, byte[]> current in fileGenerator.GetFiles(fileCount))
                    {
                        writeScheduler.Write(current.Item1, current.Item2);
                        DisplayScheduler(writeScheduler);
                    }
                });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Display scheduler.
        /// </summary>
        /// <param name="writeScheduler"></param>
        static void DisplayScheduler(IWriteScheduler writeScheduler)
        {
            lock (writeScheduler.SyncRoot)
            {
                int priority = 0;
                foreach (DeviceInfo device in writeScheduler.ToArray())
                {
                    Console.WriteLine("[{0}] si [{1}]={2}", DateTime.Now.ToFileTime(), priority++, device);
                }
            }
        }

        /// <summary>
        /// Create instance of write scheduler.
        /// </summary>
        /// <param name="schedulerType"></param>
        /// <returns></returns>
        static public IWriteScheduler CreateWriteScheduler(SchedulerType schedulerType)
        {
            // Create instance of scheduler:
            if (SchedulerType.Priority == schedulerType)
            {
                return new Priority();
            }
            else if (SchedulerType.RoundRobin == schedulerType)
            {
                return new RoundRobin();
            }

            throw new ArgumentException(String.Format("Invalid scheduler type \"{0}\"", schedulerType));
        }
    }
}