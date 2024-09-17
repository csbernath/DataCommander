using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlySegmentLinkedList<T> : IReadOnlyCollection<T>
{
    private readonly LinkedList<T[]> _linkedList;
    private readonly int _count;

    internal ReadOnlySegmentLinkedList(LinkedList<T[]> linkedList, int count)
    {
        ArgumentNullException.ThrowIfNull(linkedList);
        Assert.IsInRange(count >= 0);

        _linkedList = linkedList;
        _count = count;
    }

    public int Count => _count;

    public IEnumerator<T> GetEnumerator()
    {
        LinkedListNode<T[]> linkedListNode = _linkedList.First;
        while (linkedListNode != null)
        {
            T[] segment = linkedListNode.Value;

            int count;
            if (linkedListNode == _linkedList.Last)
            {
                int remainder = _count % segment.Length;
                count = remainder == 0 ? segment.Length : remainder;
            }
            else
                count = segment.Length;

            for (int i = 0; i < count; ++i)
                yield return segment[i];
            linkedListNode = linkedListNode.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}