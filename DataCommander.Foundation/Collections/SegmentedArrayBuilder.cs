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

        private readonly T[][] _segments;
        private int _currentSegmentArrayIndex;
        private int _currentSegmentIndex;

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
                this._segments = new T[segmentArrayLength][];
                var lastSegmentArrayIndex = segmentArrayLength - 1;

                for (var i = 0; i < lastSegmentArrayIndex; i++)
                {
                    this._segments[i] = new T[segmentLength];
                }

                var lastSegmentLength = length - lastSegmentArrayIndex*segmentLength;
                this._segments[lastSegmentArrayIndex] = new T[lastSegmentLength];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var currentSegment = this._segments[this._currentSegmentArrayIndex];
            currentSegment[this._currentSegmentIndex] = item;

            if (this._currentSegmentIndex < currentSegment.Length - 1)
            {
                this._currentSegmentIndex++;
            }
            else
            {
                this._currentSegmentArrayIndex++;
                this._currentSegmentIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<T> ToReadOnlyList()
        {
            return new ReadOnlySegmentedList(this._segments);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly T[][] _segments;

            public ReadOnlySegmentedList(T[][] segments)
            {
                this._segments = segments;
            }

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = this._segments[0].Length;
                    var segmentArrayIndex = index/segmentLength;
                    var segment = this._segments[segmentArrayIndex];
                    var segmentIndex = index%segmentLength;
                    var value = segment[segmentIndex];
                    return value;
                }
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    var lastSegmentArrayIndex = _segments.Length - 1;
                    var count = lastSegmentArrayIndex * _segments[0].Length + _segments[lastSegmentArrayIndex].Length;
                    return count;
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                for (var segmentArrayIndex = 0; segmentArrayIndex < this._segments.Length; segmentArrayIndex++)
                {
                    var segment = this._segments[segmentArrayIndex];
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