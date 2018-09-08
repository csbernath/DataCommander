using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    public class ReadOnlyArray<T> : IReadOnlyList<T>
    {
        private readonly T[] _items;

        public ReadOnlyArray(T[] items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> enumerable = _items;
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int Count => _items.Length;
        public T this[int index] => _items[index];
    }
}