﻿using System.Collections;
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

    public IReadOnlyList<T> ToReadOnlyCollection()
    {
        var count = Count;
        return new ReadOnlySegmentedList(_segments, count);
    }

    private sealed class ReadOnlySegmentedList(IList<T[]> segments, int count) : IReadOnlyList<T>
    {
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                var segmentLength = segments[0].Length;

                var segmentIndex = index / segmentLength;
                var segment = segments[segmentIndex];

                var segmentItemIndex = index % segmentLength;
                var value = segment[segmentItemIndex];
                return value;
            }
        }

        int IReadOnlyCollection<T>.Count => count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var segmentIndex = 0;
            var lastSegmentIndex = segments.Count - 1;

            foreach (var segment in segments)
            {
                var segmentLength = segment.Length;
                var segmentItemCount = segmentIndex < lastSegmentIndex ? segmentLength : count % segmentLength;

                for (var i = 0; i < segmentItemCount; i++) yield return segment[i];

                segmentIndex++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }

    private readonly int _segmentItemCapacity;
    private readonly List<T[]> _segments = [];
    private int _nextSegmentItemIndex;
}