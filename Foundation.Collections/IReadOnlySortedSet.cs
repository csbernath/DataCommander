using System.Collections.Generic;

namespace Foundation.Collections
{
    public interface IReadOnlySortedSet<T> : IReadOnlyCollection<T>
    {
        bool Contains(T item);
    }
}