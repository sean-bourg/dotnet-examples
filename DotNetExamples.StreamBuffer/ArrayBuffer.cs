using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Array buffer based version of the stream buffer - works best for smaller streams.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayBuffer<T> : IStreamBuffer<T>, IEnumerable<T>, ICollection
        where T : IEquatable<T>
    {
        /// <summary>
        /// Returns the fixed capacity of the CircularArray.
        /// </summary>
        public int Capacity { get; protected set; }

        /// <summary>
        /// Returns the number of elements in the CircularArray.
        /// </summary>
        public int Count { get; protected set; } = 0;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the instance of the <see cref="ICollection"/> class.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Gets a value indicating whether access to this instance of the <see cref="ICollection"/> is synchronized (thread safe).        
        /// </summary>
        public bool IsSynchronized { get => true; }

        /// <summary>
        /// Current stream index of the last added element.
        /// </summary>
        protected int CurrentIndex { get; set; } = 0;

        /// <summary>
        /// Data held by this stream.
        /// </summary>
        protected T[] Data { get; set; }

        /// <summary>
        /// Return sorted buffer.
        /// </summary>
        protected IEnumerable<T> Buffer
        {
            get => Data.Where((v, i) => i < Count)
                    .Select((v, i) => new { Value = v, Order = (Data.Length - CurrentIndex + i) % Data.Length })
                    .OrderBy(r => r.Order)
                    .Select(r => r.Value)
                    .ToArray();
        }

        /// <summary>
        ///  Array buffer constructor.
        /// </summary>
        /// <param name="capacity"></param>
        public ArrayBuffer(int capacity)
        {
            Capacity = capacity;
            Data = new T[Capacity];
        }

        /// <summary>
        /// Adds an object to the back of the CircularArray.
        /// </summary>
        /// <param name="value">The new object</param>
        public void Add(T value)
        {
            Data[CurrentIndex] = value;
            CurrentIndex = (CurrentIndex + 1) % Data.Length;
            if (Count < Capacity)
            {
                Count++;
            }
        }

        /// <summary>
        /// Returns the element at the given relative index. If the given item is not
        /// available, this method will return null. The relative index starts at 0
        /// for the oldest element in the CircularArray.
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <returns>value</returns>
        public T Get(int index)
        {
            if (-1 < index && index < Count)
            {
                return Buffer.ToArray()[index];
            }
            throw new ArgumentOutOfRangeException(String.Format("Requested index {0} exceeds array length.", index));
        }

        /// <summary>
        /// Returns the relative index of the given element if it is in the
        /// CircularArray or -1 if the element is not found.
        /// </summary>
        /// <param name="value">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        public int IndexOf(T value)
        {
            IEnumerable<int> list = Data.Select((v, i) => new { Value = v, Priority = i })
                    .Where(r => value.Equals(r.Value))
                    .Select(r => r.Priority);
            return 0 == list.Count() ? -1 : list.First();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => Buffer.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator() as IEnumerator;

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index) => Buffer.ToList().CopyTo(array as T[], index);
    }
}
