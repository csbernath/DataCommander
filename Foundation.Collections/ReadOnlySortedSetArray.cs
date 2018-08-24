using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public class ReadOnlySortedSetArray<T>
    {
        private readonly T[] _items;
        private readonly Comparison<T> _comparison;

        public ReadOnlySortedSetArray(T[] items, Comparison<T> comparison)
        {
            Assert.IsNotNull(items);
            Assert.IsNotNull(comparison);

            _items = items;
            _comparison = comparison;
        }

        public ReadOnlySortedSetArray(T[] items) : this(items, Comparer<T>.Default.Compare)
        {
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        private int IndexOf(T item)
        {
            int indexOfKey;

            if (_items.Length > 0)
            {
                indexOfKey = BinarySearch.IndexOf(0, _items.Length - 1, index =>
                {
                    var otherItem = _items[index];
                    return _comparison(item, otherItem);
                });
            }
            else
                indexOfKey = -1;

            return indexOfKey;
        }
    }
}