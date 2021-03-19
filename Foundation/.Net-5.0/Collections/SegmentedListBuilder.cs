using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public sealed class SegmentedListBuilder<T>
    {
        public SegmentedListBuilder(int segmentItemCapacity)
        {
            Assert.IsInRange(segmentItemCapacity > 0);
            _segmentItemCapacity = segmentItemCapacity;
        }

        public int Count
        {
            get
            {
                var count = 0;
                var segmentCount = _segments.Count;
                if (segmentCount > 0)
                    count += (segmentCount - 1) * _segmentItemCapacity;
                count += _nextSegmentItemIndex;
                return count;
            }
        }

        public void Add(T item)
        {
            T[] currentSegment;

            if (_segments.Count > 0 && _nextSegmentItemIndex < _segmentItemCapacity)
            {
                var lastSegmentIndex = _segments.Count - 1;
                currentSegment = _segments[lastSegmentIndex];
            }
            else
            {
                currentSegment = new T[_segmentItemCapacity];
                _segments.Add(currentSegment);
                _nextSegmentItemIndex = 0;
            }

            currentSegment[_nextSegmentItemIndex] = item;
            _nextSegmentItemIndex++;
        }

        public IReadOnlyList<T> ToReadOnlyList()
        {
            var count = Count;
            return new ReadOnlySegmentedList(_segments, count);
        }

        private sealed class ReadOnlySegmentedList : IReadOnlyList<T>
        {
            private readonly int _count;
            private readonly IList<T[]> _segments;

            public ReadOnlySegmentedList(IList<T[]> segments, int count)
            {
                _segments = segments;
                _count = count;
            }

            #region IReadOnlyList<T> Members

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    var segmentLength = _segments[0].Length;

                    var segmentIndex = index / segmentLength;
                    var segment = _segments[segmentIndex];

                    var segmentItemIndex = index % segmentLength;
                    var value = segment[segmentItemIndex];
                    return value;
                }
            }

            int IReadOnlyCollection<T>.Count => _count;

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                var segmentIndex = 0;
                var lastSegmentIndex = _segments.Count - 1;

                foreach (var segment in _segments)
                {
                    var segmentLength = segment.Length;
                    var segmentItemCount = segmentIndex < lastSegmentIndex ? segmentLength : _count % segmentLength;

                    for (var i = 0; i < segmentItemCount; i++) yield return segment[i];

                    segmentIndex++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<T>) this).GetEnumerator();
            }

            #endregion
        }

        #region Private Fields

        private readonly int _segmentItemCapacity;
        private readonly List<T[]> _segments = new List<T[]>();
        private int _nextSegmentItemIndex;

        #endregion
    }
}