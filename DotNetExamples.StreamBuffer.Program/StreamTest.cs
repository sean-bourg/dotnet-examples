using DotNetExamples.StreamBuffer.Program.Data;
using DotNetExamples.StreamBuffer.Program.Generators;
using DotNetExamples.StreamBuffer.Program.Generic;
using System;

namespace DotNetExamples.StreamBuffer.Program
{
    /// <summary>
    /// Stream test factory - using the factory design model.
    /// </summary>
    static public class StreamTest
    {
        /// <summary>
        /// Create instance of the stream test.
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="bufferType"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        static public IStreamTest Create(TestType testType, BufferType bufferType, int capacity)
        {
            if (TestType.VesselLocation == testType)
            {
                StreamTest<VesselLocation> streamTest = new StreamTest<VesselLocation>(StreamBufferFactory<VesselLocation>.Create(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Program.Vessels.Names").Split(','))
                {
                    streamTest.Register(new VesselLocationGenerator(name));
                }
                return streamTest;
            }
            else if (TestType.Transaction == testType)
            {
                StreamTest<Transaction> streamTest = new StreamTest<Transaction>(StreamBufferFactory<Transaction>.Create(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Program.Transaction.Names").Split(','))
                {
                    streamTest.Register(new TransactionGenerator(name));
                }
                return streamTest;
            }
            else if (TestType.Message == testType)
            {
                StreamTest<string> streamTest = new StreamTest<string>(StreamBufferFactory<string>.Create(bufferType, capacity));
                foreach (string name in System.Configuration.ConfigurationManager.AppSettings.Get("Program.Message.Names").Split(','))
                {
                    streamTest.Register(new MessageGenerator(name));
                }
                return streamTest;
            }
            throw new ArgumentException(String.Format("Invalid test type: \"{0}\"", testType));
        }
    }
}
