using DotNetExamples.StreamBuffer.Program.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetExamples.StreamBuffer.Program
{

    /// <summary>
    /// Execute test on stream datastructure using generator class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StreamTest<T> : IStreamTest
        where T : IEquatable<T>
    {
        /// <summary>
        /// Random generator.
        /// </summary>
        Random Random { get; } = new Random();

        /// <summary>
        /// Stream buffer to use for testing.
        /// </summary>
        public IStreamBuffer<T> Buffer;

        /// <summary>
        /// List of generators to use to fill stream.
        /// </summary>
        public IList<IDataGenerator<T>> GeneratorList;

        /// <summary>
        /// Get list of the registered generator names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GeneratorNames { get => GeneratorList.Select(x => x.Name); }

        /// <summary>
        /// Construct instance of the stream tester.
        /// </summary>
        /// <param name="buffer"></param>
        public StreamTest(IStreamBuffer<T> buffer)
        {
            Buffer = buffer;
            GeneratorList = new List<IDataGenerator<T>>();
        }

        /// <summary>
        /// Register generator.
        /// </summary>
        /// <param name="generator"></param>
        public void Register(IDataGenerator<T> generator) => GeneratorList.Add(generator);

        /// <summary>
        /// Test the stream buffer's add function - catches any out of bounds exceptions and returns the default value.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        T TestAdd(IStreamBuffer<T> buffer, int index)
        {
            try
            {
                return buffer.Get(index);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Run this test stream's test. 
        /// </summary>
        /// <param name="fillCount">The number of times to fill the stream when testing.</param>
        /// <param name="delay"></param>
        public void Run(int fillCount = 5, int delay = 5)
        {
            // Test empty stream.
            T element = GeneratorList[0].Next();
            int index = Buffer.IndexOf(element);
            Console.WriteLine("[{0}] tf IndexOf({1}): -1 == {2}", DateTime.Now.ToFileTime(), default(T), index);
            Console.WriteLine("[{0}] tf IndexOf({1}): -1 == {2}", DateTime.Now.ToFileTime(), element, index);
            Console.WriteLine("[{0}] tf Get({1}): null == {2}", DateTime.Now.ToFileTime(), index, TestAdd(Buffer, index));

            // Check for negative index value.
            index = -10;
            Console.WriteLine("[{0}] tf Get({1}): null == {2}", DateTime.Now.ToFileTime(), index, TestAdd(Buffer, index));

            // Check for index (well) over capacity.
            index = Buffer.Capacity * 2;
            Console.WriteLine("[{0}] tf Get({1}): null == {2}", DateTime.Now.ToFileTime(), index, TestAdd(Buffer, index));

            // Perform fill test:
            for (int testCount = 0; testCount < fillCount; testCount++)
            {
                // Build test tasks
                Task[] taskList = new Task[]
                {
                    BuildGeneratorTask(Buffer, GeneratorList, Random, 5, 100),
                    BuildGeneratorTask(Buffer, GeneratorList, Random, 5, 100),
                    BuildGeneratorTask(Buffer, GeneratorList, Random, 5, 100),
                    BuildGeneratorTask(Buffer, GeneratorList, Random, 5, 100),
                    new Task(() => Print(Buffer, delay)),
                };

                // Start tasks at random
                foreach (Task t in NextRandomTask(new List<Task>(taskList), Random))
                {
                    t.Start();
                }
                Task.WaitAll(taskList.ToArray());
                Print(Buffer);
            }

            // Print buffer final state
            Print(Buffer, delay);
        }

        /// <summary>
        /// Get random task to execute from list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        IEnumerable<Task> NextRandomTask(IList<Task> list, Random random)
        {
            while (0 < list.Count)
            {
                int i = random.Next(0, list.Count);
                yield return list[i];
                list.RemoveAt(i);
            }
        }

        /// <summary>
        /// Build generator task to add items to stream in the background.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="generatorList"></param>
        /// <param name="random"></param>
        /// <param name="minDelay"></param>
        /// <param name="maxDelay"></param>
        /// <returns></returns>
        Task BuildGeneratorTask(IStreamBuffer<T> buffer, IList<IDataGenerator<T>> generatorList, Random random, int minDelay, int maxDelay) => new Task(() =>
        {
            lock (Buffer.SyncRoot)
            {
                foreach (IDataGenerator<T> generator in generatorList)
                {
                    Task.Delay(random.Next(minDelay, maxDelay));
                    T data = generator.Next();
                    Console.WriteLine("[{0}] gn {1}", DateTime.Now.ToFileTime(), data);
                    buffer.Add(data);
                }
            }
        });

        /// <summary>
        /// Print buffer contents with a delay between each buffer print.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="delay"></param>
        public void Print(IStreamBuffer<T> buffer, int delay = 0)
        {
            lock (buffer.SyncRoot)
            {
                Console.WriteLine("[{0}] bi size={1}, capacity={2}", DateTime.Now.ToFileTime(), buffer.Count, buffer.Capacity);
                for (int i = buffer.Count - 1; i > -1; i--)
                {
                    Task.Delay(delay).Wait();
                    Console.WriteLine("[{0}] bc [{1}] {2}", DateTime.Now.ToFileTime(), i, buffer.Get(i));
                }

                for (int testIndex = 0; testIndex < Buffer.Count; testIndex++)
                {
                    T element = Buffer.Get(testIndex);
                    Console.WriteLine("[{0}] tp Get({1}): {2}", DateTime.Now.ToFileTime(), testIndex, element);

                    int index = Buffer.IndexOf(element);
                    Console.WriteLine("[{0}] tp IndexOf({1}): {2} == {3}", DateTime.Now.ToFileTime(), element, testIndex, index);
                }
            }
        }
    }
}
