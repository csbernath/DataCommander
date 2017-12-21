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
            FoundationContract.Requires<ArgumentOutOfRangeException>(segmentLength > 0);
#endif

            _segmentLength = segmentLength;
        }

#region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var index = Count % _segmentLength;

            if (index == 0)
            {
                var newSegment = new Segment
                {
                    Items = new T[_segmentLength]
                };

                if (Count == 0)
                {
                    _first = newSegment;
                    _last = newSegment;
                }
                else
                {
                    _last.Next = newSegment;
                    _last = newSegment;
                }
            }

            _last.Items[index] = item;
            Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Count = 0;
            _first = null;
            _last = null;
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
            var segment = _first;

            while (segment != null)
            {
                int count;
                if (segment != _last)
                    count = _segmentLength;
                else
                    count = Count <= _segmentLength ? Count : Count % _segmentLength;

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