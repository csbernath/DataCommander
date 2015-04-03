namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SegmentCollection<T> : ICollection<T>
    {
        #region Private Fields

        private readonly int segmentSize;
        private int count;
        private Segment first;
        private Segment last;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentSize"></param>
        public SegmentCollection(int segmentSize)
        {
            Contract.Requires<ArgumentOutOfRangeException>(segmentSize > 0);

            this.segmentSize = segmentSize;
        }

        #region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (this.count == 0)
            {
                this.first = new Segment(this.segmentSize);
                this.last = this.first;
            }

            if (this.last.Count == this.segmentSize)
            {
                var newSegment = new Segment(this.segmentSize);
                this.last.Next = newSegment;
                this.last = newSegment;
            }

            this.last.Add(item);
            this.count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.count = 0;
            this.first = null;
            this.last = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            bool contains = false;
            var segment = this.first;

            while (segment != null)
            {
                int count = segment.Count;

                for (int i = 0; i < count; i++)
                {
                    var current = segment[i];
                    if (comparer.Equals(current, item))
                    {
                        contains = true;
                        break;
                    }
                }

                segment = segment.Next;
            }

            return contains;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var segment = this.first;

            while (segment != null)
            {
                int count = segment.Count;

                for (int i = 0; i < count; i++)
                {
                    yield return segment[i];
                }

                segment = segment.Next;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable<T>)this;
            return enumerable.GetEnumerator();
        }

        #endregion

        #region Private Classes

        private sealed class Segment
        {
            private readonly T[] array;
            private int count;
            private Segment next;

            public Segment(int size)
            {
                this.array = new T[size];
            }

            public int Count
            {
                get
                {
                    return this.count;
                }
            }

            public T this[int index]
            {
                get
                {
                    return this.array[index];
                }
            }

            public Segment Next
            {
                get
                {
                    return this.next;
                }

                set
                {
                    this.next = value;
                }
            }

            public void Add(T item)
            {
                this.array[this.count] = item;
                this.count++;
            }
        }

        #endregion
    }
}