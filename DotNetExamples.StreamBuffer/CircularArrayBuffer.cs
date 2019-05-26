using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Sliding window "buffer" used to track stream transactions. Implements a FIFO datastructure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularArrayBuffer<T> : IStreamBuffer<T>, ICollection, IEnumerable<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Returns the fixed capacity of the CircularArrayNode.
        /// </summary>
        public int Capacity { get; protected set; }

        /// <summary>
        /// The number of elements currently in this buffer.
        /// </summary>
        public int Count { get; protected set; } = 0;

        /// <summary>
        /// Gets a value indicating whether access to this instance of the <see cref="ICollection"/> is synchronized (thread safe).        
        /// </summary>
        public bool IsSynchronized { get; } = true;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the instance of the <see cref="ICollection"/> class.
        /// </summary>
        public Object SyncRoot { get; } = new Object();

        /// <summary>
        /// The first element in this linked list.
        /// </summary>
        CircularArrayNode<T> First;

        /// <summary>
        /// The last element in this linked list.
        /// </summary>
        CircularArrayNode<T> Last;

        /// <summary>
        /// Delagate function for dynamic adding of elements.
        /// </summary>
        /// <param name="value"></param>
        delegate void DynamicAdd(T value);

        /// <summary>
        /// Dynamic delagate assignment variable. Since the stream can not remove element the add function dynamically changes as the stream fills to reduce program steps once full.
        /// </summary>
        DynamicAdd addFunc;

        /// <summary>
        /// Construct instance of the LinkedList buffer.
        /// </summary>
        /// <param name="capacity"></param>
        public CircularArrayBuffer(int capacity)
        {
            Capacity = capacity;
            addFunc = new DynamicAdd(AddEmpty);
        }

        /// <summary>
        /// Adds an object to the back of the CircularArrayNode.
        /// </summary>
        /// <param name="value">The new object</param>
        public void Add(T value) => addFunc(value);

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("Null array provided.");
            }

            T[] copyArray = array as T[];
            if (copyArray == null)
            {
                throw new ArgumentException("Invalid array type provided.");
            }

            if (default(CircularArrayNode<T>) != First)
            {
                int i = 0;
                foreach (T current in this)
                {
                    copyArray[i + index] = current;
                }
            }
        }

        /// <summary>
        /// Returns the element at the given relative index. If the given item is not
        /// available, this method will return null. The relative index starts at 0
        /// for the oldest element in the CircularArrayNode.
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <returns>value</returns>
        public T Get(int index)
        {
            if (-1 < index && index < Count)
            {
                CircularArrayNode<T> node = First.Get(index);
                return default(CircularArrayNode<T>) == node ? default(T) : node.Value;
            }
            throw new ArgumentOutOfRangeException(String.Format("Requested index {0} exceeds array length.", index));
        }

        /// <summary>
        /// Returns the relative index of the given element if it is in the
        /// CircularArrayNode or -1 if the element is not found.
        /// </summary>
        /// <param name="value">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        public int IndexOf(T value) => (default(CircularArrayNode<T>) == First) ? -1 : First.IndexOf(value, Count);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => new CircularArrayEnumerator<T>(First);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator() as IEnumerator;

        /// <summary>
        /// Add function for an empty stack.
        /// </summary>
        /// <param name="value">The new object</param>
        protected void AddEmpty(T value)
        {
            Count = 1;
            Last = new CircularArrayNode<T>(value);
            First = Last;
            addFunc = new DynamicAdd(AddPartial);
        }

        /// <summary>
        /// Add function for stack contain 1<= n < Capacity elements.
        /// </summary>
        /// <param name="value">The new object</param>
        protected void AddPartial(T value)
        {
            Last.Child = new CircularArrayNode<T>(value);
            Last = Last.Child;
            Count++;

            if (Count == Capacity)
            {
                addFunc = new DynamicAdd(AddFull);
            }
        }

        /// <summary>
        /// Add function for a full stack.
        /// </summary>
        /// <param name="value">The new object</param>
        protected void AddFull(T value)
        {
            Last.Child = new CircularArrayNode<T>(value);
            Last = Last.Child;
            First = First.Child;
        }
    }
}
