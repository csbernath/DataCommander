using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

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
            int count = 0;
            int segmentCount = _segments.Count;
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
            int lastSegmentIndex = _segments.Count - 1;
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

    public IReadOnlyList<T> ToReadOnlyCollection()
    {
        int count = Count;
        return new ReadOnlySegmentedList(_segments, count);
    }

    private sealed class ReadOnlySegmentedList(IList<T[]> segments, int count) : IReadOnlyList<T>
    {
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                int segmentLength = segments[0].Length;

                int segmentIndex = index / segmentLength;
                T[] segment = segments[segmentIndex];

                int segmentItemIndex = index % segmentLength;
                T value = segment[segmentItemIndex];
                return value;
            }
        }

        int IReadOnlyCollection<T>.Count => count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            int segmentIndex = 0;
            int lastSegmentIndex = segments.Count - 1;

            foreach (T[] segment in segments)
            {
                int segmentLength = segment.Length;
                int segmentItemCount = segmentIndex < lastSegmentIndex ? segmentLength : count % segmentLength;

                for (int i = 0; i < segmentItemCount; i++) yield return segment[i];

                segmentIndex++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }
    }

    private readonly int _segmentItemCapacity;
    private readonly List<T[]> _segments = [];
    private int _nextSegmentItemIndex;
}