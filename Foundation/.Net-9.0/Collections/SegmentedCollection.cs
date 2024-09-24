using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

public class SegmentedCollection<T> : ICollection<T>
{
    public SegmentedCollection(int segmentLength)
    {
        Assert.IsInRange(segmentLength > 0);
        _segmentLength = segmentLength;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        var segment = _first;

        while (segment != null)
        {
            int count;
            if (segment != _last)
                count = _segmentLength;
            else
                count = Count <= _segmentLength ? Count : Count % _segmentLength;

            for (var i = 0; i < count; i++) yield return segment.Items[i];

            segment = segment.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        var enumerable = (IEnumerable<T>) this;
        return enumerable.GetEnumerator();
    }

    private sealed class Segment
    {
        public T[] Items;
        public Segment Next;
    }

    private readonly int _segmentLength;
    private Segment _first;
    private Segment _last;

    public void Add(T item)
    {
        var index = Count % _segmentLength;

        if (index == 0)
        {
            var newSegment = new Segment
            {
                Items = new T[_segmentLength]
            };

            if (Count == 0)
            {
                _first = newSegment;
                _last = newSegment;
            }
            else
            {
                _last.Next = newSegment;
                _last = newSegment;
            }
        }

        _last.Items[index] = item;
        Count++;
    }

    public void Clear()
    {
        Count = 0;
        _first = null;
        _last = null;
    }

    public bool Contains(T item) => throw new NotSupportedException();

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public int Count { get; private set; }

    bool ICollection<T>.IsReadOnly => false;

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}