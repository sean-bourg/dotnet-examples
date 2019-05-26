using Gray.DistributedWriter.DocumentManagement;
using Gray.DistributedWriter.DocumentManagement.Devices;
using Gray.DistributedWriter.DocumentManagement.Schedulers;
using Gray.DistributedWriter.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gray.DistributedWriter
{
    /// <summary>
    /// Main execution program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Number of files to test using.
        /// </summary>
        static protected int FileCount;

        /// <summary>
        /// Number of consective task to use when testing concurrent writes.
        /// </summary>
        static protected int TaskCount;

        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                // Load configuration file
                TaskCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Program.TaskCount"));
                FileCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Program.FileCount"));

                // Initialize pogram variables
                string deviceFolder = null;
                SchedulerType schedulerType = default(SchedulerType);
                bool runTest = true;
                int fileCount = FileCount;

                // Testing local
                deviceFolder = System.Configuration.ConfigurationManager.AppSettings.Get("Program.DeviceFolder");

                // Parse user input:
                if (0 < args.Length)
                {
                    List<string> errorList = new List<string>();
                    for (int i = 0; i < args.Length;)
                    {
                        if (!args[i].StartsWith('-') && String.IsNullOrEmpty(deviceFolder))
                        {
                            deviceFolder = args[i];
                            i++;
                        }
                        else
                        {
                            switch (args[i].ToLower())
                            {
                                case "-?":
                                case "--help":
                                    DisplayHelp();
                                    runTest = false;
                                    i = args.Length;
                                    break;

                                case "-c":
                                case "--count":
                                    if (!int.TryParse(args[i + 1], out fileCount))
                                    {
                                        errorList.Add(String.Format("Invalid file count specified: '{0}'", args[i + 1]));
                                    }
                                    i += 2;
                                    break;

                                case "-t":
                                case "--type":
                                    if (!Enum.TryParse<SchedulerType>(args[i + 1], out schedulerType))
                                    {
                                        errorList.Add(String.Format("Invalid test type specified: '{0}'.", args[i + 1]));
                                    }
                                    i += 2;
                                    break;

                                default:
                                    errorList.Add(String.Format("Invalid input {0}", args[i]));
                                    i = args.Length;
                                    break;
                            }
                        }
                    }

                    if (runTest && String.IsNullOrEmpty(deviceFolder))
                    {
                        DisplayError("Device folder required.");
                        runTest = false;
                    }
                }

                if (runTest)
                {
                    // Report program options
                    Console.WriteLine("[{0}] pi scheduler type = {1}", DateTime.Now.ToFileTime(), schedulerType);
                    Console.WriteLine("[{0}] pi device folder = {1}", DateTime.Now.ToFileTime(), deviceFolder);
                    Console.WriteLine("[{0}] pi file count = {1}", DateTime.Now.ToFileTime(), fileCount);
                    Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

                    // Get devices and clear contents
                    DirectoryInfo directoryInfo = new DirectoryInfo(deviceFolder);
                    int min = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Device.Latency.Min"));
                    int max = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Device.Latency.Max"));
                    IDevice[] devices = GetDevices(directoryInfo, new Latency(min, max));
                    foreach (IDevice device in devices)
                    {
                        device.Clear();
                    }

                    // Intialize variables and load configuration settings for the content generator.
                    IWriteScheduler writeScheduler = WriteScheduler.Create(schedulerType, devices);
                    FileGenerator generator = new FileGenerator(
                        System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.Dictionary").Split(','),
                        new Random()
                    );
                    generator.MinWords = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinWords"));
                    generator.MaxWords = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxWords"));
                    generator.MinSentences = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinSentences"));
                    generator.MaxSentences = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxSentences"));
                    generator.MinParagraphs = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MinParagraphs"));
                    generator.MaxParagraphs = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("LipsumIpsumGenerator.MaxParagraphs"));

                    // Print initial scheduler state.
                    PrintScheduler(writeScheduler);

                    // Test using sequential writes.
                    Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);
                    RunTest(generator, writeScheduler, fileCount);
                    Console.WriteLine("[{0}] pm {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);

                    // Test using concurrent writes.
                    Task[] tasks = new Task[TaskCount];
                    for (int i = 0; i < TaskCount; i++)
                    {
                        tasks[i] = new Task(() => RunTest(generator, writeScheduler, fileCount));
                        tasks[i].Start();
                    }
                    Task.WaitAll(tasks);

                    // Print final scheduler state
                    PrintScheduler(writeScheduler);
                    Console.WriteLine("[{0}] pi {1}kb", DateTime.Now.ToFileTime(), GC.GetTotalMemory(true) / 1024);
                }
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
        /// Run scheduler test by writing a number of files.
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="writeScheduler"></param>
        /// <param name="count"></param>
        static protected void RunTest(FileGenerator generator, IWriteScheduler writeScheduler, int count)
        {
            foreach (Tuple<string, byte[]> current in generator.GetFiles(count))
            {
                writeScheduler.Write(current.Item1, current.Item2);
                PrintScheduler(writeScheduler);
            }
        }

        /// <summary>
        /// Display help message.
        /// </summary>
        static protected void DisplayHelp()
        {
            Console.WriteLine("Options: path_to_device [[-t|--type RoundRobin|Priority] [-c|--count #]]");
            Console.WriteLine("\t-?,--help print this help message.");
            Console.WriteLine("\t-c,--count <#> Set the number of files to be used in testing (default={0}).", FileCount);
            Console.WriteLine("\t-t,--type Select scheduler type [RoundRobin|Priority] (default=\"{0}\").", default(SchedulerType));
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
        /// Create a list of devices from a folder using the provided latency.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="latency"></param>
        /// <returns></returns>
        static protected IDevice[] GetDevices(DirectoryInfo directoryInfo, Latency latency) => directoryInfo.GetDirectories()
            .Select((directory, index) => (0 == (index % 2)) ? new FolderDevice(directory, latency) : new ChunkFolderDevice(directory, latency))
            .ToArray();

        /// <summary>
        /// Print scheduler's devices.
        /// </summary>
        /// <param name="writeScheduler"></param>
        static protected void PrintScheduler(IWriteScheduler writeScheduler)
        {
            lock (writeScheduler)
            {
                int priority = 0;
                foreach (DeviceInfo device in writeScheduler.ToArray())
                {
                    Console.WriteLine("[{0}] si [{1}]={2}", DateTime.Now.ToFileTime(), priority++, device);
                }
            }
        }
    }
}