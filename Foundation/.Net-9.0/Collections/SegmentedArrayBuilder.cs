using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

public sealed class SegmentedArrayBuilder<T>
{
    public SegmentedArrayBuilder(
        int length,
        int segmentLength)
    {
        Assert.IsInRange(length >= 0);
        Assert.IsInRange(segmentLength >= 0);

        if (length > 0)
        {
            int segmentArrayLength = (length + segmentLength - 1) / segmentLength;
            _segments = new T[segmentArrayLength][];
            int lastSegmentArrayIndex = segmentArrayLength - 1;

            for (int i = 0; i < lastSegmentArrayIndex; i++) _segments[i] = new T[segmentLength];

            int lastSegmentLength = length - lastSegmentArrayIndex * segmentLength;
            _segments[lastSegmentArrayIndex] = new T[lastSegmentLength];
        }
    }

    public void Add(T item)
    {
        T[] currentSegment = _segments[_currentSegmentArrayIndex];
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

    public IReadOnlyList<T> ToReadOnlyCollection()
    {
        return new ReadOnlySegmentedList(_segments);
    }

    private sealed class ReadOnlySegmentedList(T[][] segments) : IReadOnlyList<T>
    {
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                int segmentLength = segments[0].Length;
                int segmentArrayIndex = index / segmentLength;
                T[] segment = segments[segmentArrayIndex];
                int segmentIndex = index % segmentLength;
                T value = segment[segmentIndex];
                return value;
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                int lastSegmentArrayIndex = segments.Length - 1;
                int count = lastSegmentArrayIndex * segments[0].Length + segments[lastSegmentArrayIndex].Length;
                return count;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int segmentArrayIndex = 0; segmentArrayIndex < segments.Length; segmentArrayIndex++)
            {
                T[] segment = segments[segmentArrayIndex];
                for (int segmentIndex = 0; segmentIndex < segment.Length; segmentIndex++) yield return segment[segmentIndex];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }
    }

    private readonly T[][] _segments;
    private int _currentSegmentArrayIndex;
    private int _currentSegmentIndex;
}