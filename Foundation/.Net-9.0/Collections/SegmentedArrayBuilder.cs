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
            var segmentArrayLength = (length + segmentLength - 1) / segmentLength;
            _segments = new T[segmentArrayLength][];
            var lastSegmentArrayIndex = segmentArrayLength - 1;

            for (var i = 0; i < lastSegmentArrayIndex; i++) _segments[i] = new T[segmentLength];

            var lastSegmentLength = length - lastSegmentArrayIndex * segmentLength;
            _segments[lastSegmentArrayIndex] = new T[lastSegmentLength];
        }
    }

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

    public IReadOnlyList<T> ToReadOnlyCollection() => new ReadOnlySegmentedList(_segments);

    private sealed class ReadOnlySegmentedList(T[][] segments) : IReadOnlyList<T>
    {
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                var segmentLength = segments[0].Length;
                var segmentArrayIndex = index / segmentLength;
                var segment = segments[segmentArrayIndex];
                var segmentIndex = index % segmentLength;
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
            for (var segmentArrayIndex = 0; segmentArrayIndex < segments.Length; segmentArrayIndex++)
            {
                var segment = segments[segmentArrayIndex];
                for (var segmentIndex = 0; segmentIndex < segment.Length; segmentIndex++) yield return segment[segmentIndex];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }

    private readonly T[][] _segments;
    private int _currentSegmentArrayIndex;
    private int _currentSegmentIndex;
}