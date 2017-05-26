using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SegmentedCollection<T> : ICollection<T>
    {
        #region Private Fields

        private readonly int _segmentLength;
        private Segment _first;
        private Segment _last;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentLength"></param>
        public SegmentedCollection(int segmentLength)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(segmentLength > 0);
#endif

            this._segmentLength = segmentLength;
        }

#region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var index = this.Count%this._segmentLength;

            if (index == 0)
            {
                var newSegment = new Segment
                {
                    Items = new T[this._segmentLength]
                };

                if (this.Count == 0)
                {
                    this._first = newSegment;
                    this._last = newSegment;
                }
                else
                {
                    this._last.Next = newSegment;
                    this._last = newSegment;
                }
            }

            this._last.Items[index] = item;
            this.Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.Count = 0;
            this._first = null;
            this._last = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

#endregion

#region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var segment = this._first;

            while (segment != null)
            {
                int count;
                if (segment != this._last)
                    count = this._segmentLength;
                else
                    count = this.Count <= this._segmentLength ? this.Count : this.Count%_segmentLength;

                for (var i = 0; i < count; i++)
                {
                    yield return segment.Items[i];
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
            public T[] Items;
            public Segment Next;
        }

#endregion
    }
}