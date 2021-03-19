using System.Collections.Generic;
using System.Linq;
using Foundation.Linq;

namespace Foundation.Collections
{
    public class ElementIndexComparer<T> : IComparer<T>
    {
        private readonly SortedDictionary<T, int> _items;

        public ElementIndexComparer(IEnumerable<T> items)
        {
            _items = items
                .Select((element, index) => new {Element = element, Index = index})
                .ToSortedDictionary(i => i.Element, i => i.Index);
        }

        public int Compare(T x, T y)
        {
            var xIndex = _items[x];
            var yIndex = _items[y];
            return xIndex.CompareTo(yIndex);
        }
    }
}