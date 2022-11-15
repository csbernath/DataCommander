using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;

namespace Foundation.Collections;

public class SegmentLinkedListBuilder<T>
{
    private readonly LinkedList<T[]> _linkedList = new();
    private readonly int _segmentLength;
    private int _count;

    public SegmentLinkedListBuilder(int segmentLength)
    {
        Assert.IsInRange(segmentLength > 0);
        _segmentLength = segmentLength;
    }

    public void Add(T item)
    {
        T[] segment;
        var index = _count % _segmentLength;
        if (index == 0)
        {
            segment = new T[_segmentLength];
            _linkedList.AddLast(segment);
        }
        else
            segment = _linkedList.Last.Value;

        segment[index] = item;
        ++_count;
    }

    public ReadOnlySegmentLinkedList<T> ToReadOnlySegmentLinkedList() => new(_linkedList, _count);
}