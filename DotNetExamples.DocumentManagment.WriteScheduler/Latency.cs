using System;

namespace Gray.DistributedWriter.DocumentManagement
{
    /// <summary>
    /// Generate latency between min and max values.
    /// </summary>
    public readonly struct Latency
    {
        /// <summary>
        /// Minimium latency time.
        /// </summary>
        public readonly int Min;

        /// <summary>
        /// Maximium latency time.
        /// </summary>
        public readonly int Max;

        /// <summary>
        /// Create instance of the latency structure.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Latency(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Get next random latency value.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public int Next(Random random) => random.Next(Min, Max);
    }
}
