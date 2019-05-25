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
    public class LinkedListBuffer<T> : IStreamBuffer<T>, ICollection, IEnumerable<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The first element in this linked list.
        /// </summary>
        protected LinkedList<T> List;

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
        /// Construct instance of the LinkedList buffer.
        /// </summary>
        /// <param name="capacity"></param>
        public LinkedListBuffer(int capacity)
        {
            List = new LinkedList<T>();
            Capacity = capacity;
        }

        /// <summary>
        /// Adds an object to the back of the CircularArray.
        /// </summary>
        /// <param name="obj">The new object</param>
        public void Add(T obj)
        {
            List.AddFirst(obj);
            if (Count == Capacity)
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
        public T Get(int index) => (-1 < index && index < Count) ? List.Select(x => x).Reverse().ToArray()[index] : default(T);

        /// <summary>
        /// Returns the relative index of the given element if it is in the
        /// CircularArray or -1 if the element is not found.
        /// </summary>
        /// <param name="obj">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        public int IndexOf(T obj)
        {
            // Use linq query & anonymous types
            IEnumerable<int> results = List.Select((value, index) => new { Value = value, SortOrder = Count - index - 1 })
                .Where(record => { bool match = obj.Equals(record.Value); return match; })
                .Select(match => match.SortOrder);
            return (0 == results.Count()) ? -1 : results.First();
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
