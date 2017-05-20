namespace DataCommander.Foundation.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SegmentedListBuilder<T>
    {
        #region Private Fields

        private readonly int _segmentItemCapacity;
        private readonly List<T[]> _segments = new List<T[]>();
        private int _nextSegmentItemIndex;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentItemCapacity"></param>
        public SegmentedListBuilder(int segmentItemCapacity)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(segmentItemCapacity > 0);
#endif
            this._segmentItemCapacity = segmentItemCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            T[] currentSegment;

            if (this._segments.Count > 0 && this._nextSegmentItemIndex < this._segmentItemCapacity)
            {
                var lastSegmentIndex = this._segments.Count - 1;
                currentSegment = this._segments[lastSegmentIndex];
            }
            else
            {
                currentSegment = new T[this._segmentItemCapacity];
                this._segments.Add(currentSegment);
                _nextSegmentItemIndex = 0;
            }

            currentSegment[_nextSegmentItemIndex] = item;
            _nextSegmentItemIndex++;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                var segmentCount = this._segments.Count;
                if (segmentCount > 0)
                {
                    count += (segmentCount - 1)*this._segmentItemCapacity;
                }
                count += this._nextSegmentItemIndex;

                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<T> ToReadOnlyList()
        {
            var count = this.Count;
            return new ReadOnlySegmentedList(this._segments, count);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly IList<T[]> _segments;
            private readonly int _count;

            public ReadOnlySegmentedList(IList<T[]> segments, int count)
            {
                this._segments = segments;
                this._count = count;
            }

#region IReadOnlyList<T> Members

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = _segments[0].Length;

                    var segmentIndex = index/segmentLength;
                    var segment = this._segments[segmentIndex];

                    var segmentItemIndex = index%segmentLength;
                    var value = segment[segmentItemIndex];
                    return value;
                }
            }

            int IReadOnlyCollection<T>.Count => this._count;

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                var segmentIndex = 0;
                var lastSegmentIndex = this._segments.Count - 1;

                foreach (var segment in this._segments)
                {
                    var segmentLength = segment.Length;
                    var segmentItemCount = segmentIndex < lastSegmentIndex ? segmentLength : this._count%segmentLength;

                    for (var i = 0; i < segmentItemCount; i++)
                    {
                        yield return segment[i];
                    }

                    segmentIndex++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<T>)this).GetEnumerator();
            }

#endregion
        }
    }
}