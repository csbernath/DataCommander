using System;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public sealed class ReverseComparer<T> : IComparer<T>
{
    private static readonly Lazy<ReverseComparer<T>> Instance = new(CreateReversedComparer);

    private readonly IComparer<T> _comparer;

    private ReverseComparer(IComparer<T> comparer) => _comparer = comparer;

    public static IComparer<T> Default => Instance.Value;

    public int Compare(T x, T y) => _comparer.Compare(y, x);

    private static ReverseComparer<T> CreateReversedComparer()
    {
        var comparer = Comparer<T>.Default;
        return new ReverseComparer<T>(comparer);
    }
}