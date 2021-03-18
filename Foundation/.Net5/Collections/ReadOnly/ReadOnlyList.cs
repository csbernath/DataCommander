using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _items;
        public static ReadOnlyList<T> Empty = new ReadOnlyList<T>();

        /// <summary>
        /// ctor for Newtonsoft.JSON deserializing
        /// </summary>
        public ReadOnlyList(IEnumerable<T> items)
        {
            Assert.IsNotNull(items);
            var list = items.ToList();
            _items = list.Count > 0 ? (IReadOnlyList<T>) list : EmptyArray<T>.Value;
        }

        internal ReadOnlyList(IReadOnlyList<T> items) => _items = items;
        private ReadOnlyList() => _items = EmptyArray<T>.Value;

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}