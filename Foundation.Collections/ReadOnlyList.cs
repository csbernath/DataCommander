using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _items;

        public ReadOnlyList(IList<T> items)
        {
            Assert.IsNotNull(items);
            _items = items;
        }

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}