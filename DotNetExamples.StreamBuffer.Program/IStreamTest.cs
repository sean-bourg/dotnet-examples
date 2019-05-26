using System.Collections.Generic;

namespace DotNetExamples.StreamBuffer.Program
{
    /// <summary>
    /// Interface for DataStream testing programs.
    /// </summary>
    public interface IStreamTest
    {
        /// <summary>
        /// Run this test stream's test.
        /// </summary>
        /// <param name="fillCount"></param>
        /// <param name="delay"></param>
        void Run(int fillCount = 50, int delay = 5);


        /// <summary>
        /// Get a list of the generator names.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GeneratorNames { get; }
    }
}
