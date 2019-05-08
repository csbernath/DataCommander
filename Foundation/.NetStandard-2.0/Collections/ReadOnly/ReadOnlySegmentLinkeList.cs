using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public class ReadOnlySegmentLinkedList<T> : IReadOnlyCollection<T>
    {
        private readonly LinkedList<T[]> _linkedList;
        private readonly int _count;

        internal ReadOnlySegmentLinkedList(LinkedList<T[]> linkedList, int count)
        {
            Assert.IsNotNull(linkedList);
            Assert.IsInRange(count >= 0);

            _linkedList = linkedList;
            _count = count;
        }

        public int Count => _count;

        public IEnumerator<T> GetEnumerator()
        {
            var linkedListNode = _linkedList.First;
            while (linkedListNode != null)
            {
                var segment = linkedListNode.Value;
                var count = linkedListNode != _linkedList.Last ? segment.Length : _count % segment.Length;
                for (var i = 0; i < count; ++i)
                    yield return segment[i];
                linkedListNode = linkedListNode.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}