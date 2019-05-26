using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Linked list - contained in the buffer class so that it can only be used here.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CircularArrayNode<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The value of this linked list item.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The count of this linked list and it's children.
        /// </summary>
        public int Count
        {
            get => (default(CircularArrayNode<T>) == Child) ? 1 : Child.Count + 1;
        }

        /// <summary>
        /// This linked list's child.
        /// </summary>
        public CircularArrayNode<T> Child { get; set; } = default(CircularArrayNode<T>);

        /// <summary>
        /// Construct an instance of the linked list.
        /// </summary>
        /// <param name="value"></param>
        public CircularArrayNode(T value) => Value = value;

        /// <summary>
        /// Construct an instanc eof the linked list.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="child"></param>
        public CircularArrayNode(T value, CircularArrayNode<T> child) : this(value) => Child = child;

        /// <summary>
        /// Returns the element at the given relative index. If the given item is not available, this method will return null. 
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <returns>value</returns>
        public CircularArrayNode<T> Get(int index)
        {
            // have to check for a negative because some people ¯\_(ツ)_/¯
            if (0 <= index)
            {
                return this;
            }
            return default(CircularArrayNode<T>) == Child ? default(CircularArrayNode<T>) : Child.Get(index - 1);
        }

        /// <summary>
        /// Returns the relative index of the given element if it is in the CircularArrayNode or -1 if the element is not found.
        /// </summary>
        /// <param name="value">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        public int IndexOf(T value, int index)
        {
            bool isMatch = value.Equals(Value);
            if (isMatch)
            {
                return index;
            }
            return default(CircularArrayNode<T>) == Child ? -1 : Child.IndexOf(value, index - 1);
        }
    }
}
