using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _items;

        public ReadOnlyList(IReadOnlyList<T> items)
        {
            Assert.IsNotNull(items);
            _items = items;
        }

        public ReadOnlyList(IEnumerable<T> items)
        {
            Assert.IsNotNull(items);
            _items = items.ToList();
        }

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}