using System;

namespace DotNetExamples.StreamBuffer.Program.Generators
{
    /// <summary>
    /// Generate sequential message based on timestamp.
    /// </summary>
    class MessageGenerator : IDataGenerator<string>
    {
        /// <summary>
        /// Message counter.
        /// </summary>
        protected int Count = 0;

        /// <summary>
        /// Name of this message generator.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  Construct message generator.
        /// </summary>
        /// <param name="name">Name of this message generator.</param>
        public MessageGenerator(string name) => Name = name;

        /// <summary>
        /// Get next message.
        /// </summary>
        /// <returns>Message with id number.</returns>
        public string Next() => String.Format("({0}, {1})", Count++, Name);
    }
}
