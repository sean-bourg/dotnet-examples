using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Sliding window "buffer" used to track stream transactions. 
    /// This implementation is built in languages LinkList class and the linq query langauage in 
    /// order to reduce code maintenance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedListBuffer<T> : IStreamBuffer<T>, IEnumerable<T>, ICollection
        where T : IEquatable<T>
    {
        /// <summary>
        /// Returns the fixed capacity of the CircularArray.
        /// </summary>
        public int Capacity { get; protected set; }

        /// <summary>
        /// The number of elements currently in this buffer.
        /// </summary>
        public int Count { get => List.Count; }

        /// <summary>
        /// Gets a value indicating whether access to this instance of the <see cref="ICollection"/> is synchronized (thread safe).        
        /// </summary>
        public bool IsSynchronized { get => true; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the instance of the <see cref="ICollection"/> class.
        /// </summary>
        public Object SyncRoot { get; } = new Object();

        /// <summary>
        /// The first element in this linked list.
        /// </summary>
        LinkedList<T> List;

        /// <summary>
        /// Construct instance of the LinkedList buffer.
        /// </summary>
        /// <param name="capacity"></param>
        public LinkedListBuffer(int capacity)
        {
            Capacity = capacity;
            List = new LinkedList<T>();
        }

        /// <summary>
        /// Adds an object to the back of the CircularArray.
        /// </summary>
        /// <param name="value">The new object</param>
        public void Add(T value)
        {
            List.AddFirst(value);
            if (Count > Capacity)
            {
                List.RemoveLast();
            }
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index) => List.CopyTo(array as T[], index);

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
                return List.Select(x => x).Reverse().ToArray()[index];
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
            // Use linq query & anonymous types
            IEnumerable<int> results = List.Select((v, i) => new { Value = v, SortOrder = Count - i - 1 })
                .Where(r => { bool match = value.Equals(r.Value); return match; })
                .Select(m => m.SortOrder);
            return 0 == results.Count() ? -1 : results.First();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator() as IEnumerator;
    }
}
