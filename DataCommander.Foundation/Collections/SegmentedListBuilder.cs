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

        private readonly int segmentItemCapacity;
        private readonly List<T[]> segments = new List<T[]>();
        private int nextSegmentItemIndex;

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
            this.segmentItemCapacity = segmentItemCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            T[] currentSegment;

            if (this.segments.Count > 0 && this.nextSegmentItemIndex < this.segmentItemCapacity)
            {
                var lastSegmentIndex = this.segments.Count - 1;
                currentSegment = this.segments[lastSegmentIndex];
            }
            else
            {
                currentSegment = new T[this.segmentItemCapacity];
                this.segments.Add(currentSegment);
                nextSegmentItemIndex = 0;
            }

            currentSegment[nextSegmentItemIndex] = item;
            nextSegmentItemIndex++;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                var segmentCount = this.segments.Count;
                if (segmentCount > 0)
                {
                    count += (segmentCount - 1)*this.segmentItemCapacity;
                }
                count += this.nextSegmentItemIndex;

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
            return new ReadOnlySegmentedList(this.segments, count);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly IList<T[]> segments;
            private readonly int count;

            public ReadOnlySegmentedList(IList<T[]> segments, int count)
            {
                this.segments = segments;
                this.count = count;
            }

#region IReadOnlyList<T> Members

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = segments[0].Length;

                    var segmentIndex = index/segmentLength;
                    var segment = this.segments[segmentIndex];

                    var segmentItemIndex = index%segmentLength;
                    var value = segment[segmentItemIndex];
                    return value;
                }
            }

            int IReadOnlyCollection<T>.Count => this.count;

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                var segmentIndex = 0;
                var lastSegmentIndex = this.segments.Count - 1;

                foreach (var segment in this.segments)
                {
                    var segmentLength = segment.Length;
                    var segmentItemCount = segmentIndex < lastSegmentIndex ? segmentLength : this.count%segmentLength;

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