using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly;

public interface IReadOnlySortedSet<T> : IReadOnlyCollection<T>
{
    bool Contains(T item);
}