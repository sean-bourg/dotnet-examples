using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DotNetExamples.StreamBuffer
{
    /// <summary>
    /// Thread safe enumerator for the circular array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CircularArrayEnumerator<T> : IEnumerator<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The base linked list.
        /// </summary>
        readonly CircularArrayNode<T> List;

        /// <summary>
        /// The current pointer for the linked list.
        /// </summary>
        CircularArrayNode<T> _current;

        /// <summary>
        /// Has started flag for enumerator, used to determine if the enumerator has moved into the first element..
        /// </summary>
        bool HasStarted;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current { get => _current.Value; }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current { get => Current; }

        /// <summary>
        /// Construct enumerator for linked list.
        /// </summary>
        /// <param name="list"></param>
        public CircularArrayEnumerator(CircularArrayNode<T> list)
        {
            List = list;
            HasStarted = false;
        }


        public void Dispose()
        { }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (!HasStarted)
            {
                HasStarted = true;
                _current = List;
            }
            else if (default(CircularArrayNode<T>) != _current)
            {
                _current = _current.Child;
            }
            return default(CircularArrayNode<T>) != _current;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            HasStarted = false;
            _current = default(CircularArrayNode<T>);
        }
    }
}
