using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public interface ICollectionIndex<T> : ICollection<T>
{
    string? Name { get; }
}