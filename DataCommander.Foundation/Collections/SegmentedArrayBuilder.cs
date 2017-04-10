namespace DataCommander.Foundation.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SegmentedArrayBuilder<T>
    {
        #region Private Fields

        private readonly T[][] segments;
        private int currentSegmentArrayIndex;
        private int currentSegmentIndex;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="segmentLength"></param>
        public SegmentedArrayBuilder(
            int length,
            int segmentLength)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(length >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(segmentLength >= 0);
#endif

            if (length > 0)
            {
                var segmentArrayLength = (length + segmentLength - 1)/segmentLength;
                this.segments = new T[segmentArrayLength][];
                var lastSegmentArrayIndex = segmentArrayLength - 1;

                for (var i = 0; i < lastSegmentArrayIndex; i++)
                {
                    this.segments[i] = new T[segmentLength];
                }

                var lastSegmentLength = length - lastSegmentArrayIndex*segmentLength;
                this.segments[lastSegmentArrayIndex] = new T[lastSegmentLength];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var currentSegment = this.segments[this.currentSegmentArrayIndex];
            currentSegment[this.currentSegmentIndex] = item;

            if (this.currentSegmentIndex < currentSegment.Length - 1)
            {
                this.currentSegmentIndex++;
            }
            else
            {
                this.currentSegmentArrayIndex++;
                this.currentSegmentIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<T> ToReadOnlyList()
        {
            return new ReadOnlySegmentedList(this.segments);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly T[][] segments;

            public ReadOnlySegmentedList(T[][] segments)
            {
                this.segments = segments;
            }

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = this.segments[0].Length;
                    var segmentArrayIndex = index/segmentLength;
                    var segment = this.segments[segmentArrayIndex];
                    var segmentIndex = index%segmentLength;
                    var value = segment[segmentIndex];
                    return value;
                }
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    var lastSegmentArrayIndex = segments.Length - 1;
                    var count = lastSegmentArrayIndex * segments[0].Length + segments[lastSegmentArrayIndex].Length;
                    return count;
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                for (var segmentArrayIndex = 0; segmentArrayIndex < this.segments.Length; segmentArrayIndex++)
                {
                    var segment = this.segments[segmentArrayIndex];
                    for (var segmentIndex = 0; segmentIndex < segment.Length; segmentIndex++)
                    {
                        yield return segment[segmentIndex];
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<T>)this).GetEnumerator();
            }
        }
    }
}