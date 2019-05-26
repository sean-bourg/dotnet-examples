using DotNetExamples.StreamBuffer.Program.Data;
using System;

namespace DotNetExamples.StreamBuffer.Program.Generators
{
    /// <summary>
    /// Generate random transaction.
    /// </summary>
    public class TransactionGenerator : IGenerator<Transaction>
    {
        /// <summary>
        /// The id for the next transaction.
        /// </summary>
        protected int Id;

        /// <summary>
        /// Random object for generating random content.
        /// </summary>
        protected readonly Random Random;

        /// <summary>
        /// Account name to use with this transaction generator.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public TransactionGenerator(string name, int id = 1)
        {
            Name = name;
            Id = id;
            Random = new Random();
        }

        /// <summary>
        /// Get next transaction.
        /// </summary>
        /// <returns></returns>
        public Transaction Next() => new Transaction(Id++, Name, Random.Next(int.MinValue, int.MaxValue - 1), DateTime.Now);
    }
}
