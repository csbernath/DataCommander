using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
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
            FoundationContract.Requires<ArgumentOutOfRangeException>(length >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(segmentLength >= 0);
#endif

            if (length > 0)
            {
                var segmentArrayLength = (length + segmentLength - 1)/segmentLength;
                _segments = new T[segmentArrayLength][];
                var lastSegmentArrayIndex = segmentArrayLength - 1;

                for (var i = 0; i < lastSegmentArrayIndex; i++)
                {
                    _segments[i] = new T[segmentLength];
                }

                var lastSegmentLength = length - lastSegmentArrayIndex*segmentLength;
                _segments[lastSegmentArrayIndex] = new T[lastSegmentLength];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var currentSegment = _segments[_currentSegmentArrayIndex];
            currentSegment[_currentSegmentIndex] = item;

            if (_currentSegmentIndex < currentSegment.Length - 1)
            {
                _currentSegmentIndex++;
            }
            else
            {
                _currentSegmentArrayIndex++;
                _currentSegmentIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<T> ToReadOnlyList()
        {
            return new ReadOnlySegmentedList(_segments);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly T[][] _segments;

            public ReadOnlySegmentedList(T[][] segments)
            {
                _segments = segments;
            }

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = _segments[0].Length;
                    var segmentArrayIndex = index/segmentLength;
                    var segment = _segments[segmentArrayIndex];
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
                for (var segmentArrayIndex = 0; segmentArrayIndex < _segments.Length; segmentArrayIndex++)
                {
                    var segment = _segments[segmentArrayIndex];
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