using System;
using System.Collections.ObjectModel;

namespace Foundation.Collections.ReadOnly;

public static class EmptyReadOnlyCollection<T>
{
    public static readonly ReadOnlyCollection<T> Value = new(Array.Empty<T>());
}