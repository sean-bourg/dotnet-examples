using System;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Stream buffer interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IStreamBuffer<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Returns the fixed capacity of the CircularArray.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Returns the number of elements in the CircularArray.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an object to the back of the CircularArray.
        /// </summary>
        /// <param name="value">The new object</param>
        void Add(T value);

        /// <summary>
        /// Returns the relative index of the given element if it is in the
        /// CircularArray or -1 if the element is not found.
        /// </summary>
        /// <param name="value">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        int IndexOf(T value);

        /// <summary>
        /// Returns the element at the given relative index. If the given item is not
        /// available, this method will return null. The relative index starts at 0
        /// for the oldest element in the CircularArray.
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <returns>value</returns>
        T Get(int index);

        /// <summary>
        /// Gets a value indicating whether access to this instance of the <see cref="ICollection"/> is synchronized (thread safe).        
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the instance of the <see cref="ICollection"/> class.
        /// </summary>
        object SyncRoot { get; }
    }
}
