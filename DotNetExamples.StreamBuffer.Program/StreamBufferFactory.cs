using System;

namespace DotNetExamples.StreamBuffer.Program
{
    /// <summary>
    /// Factory design model used to generate an instance of the Stream buffer class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public class StreamBufferFactory<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Generate a stream test using a factory design method.
        /// </summary>
        /// <param name="bufferType"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        static public IStreamBuffer<T> Create(BufferType bufferType, int capacity)
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
    }
}
