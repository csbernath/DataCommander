using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections;

public sealed class MultipleMemberEqualityComparer<T> : IEqualityComparer<T>
{
    #region Private Fields

    private readonly IEqualityComparer<T>[] _equalityComparers;

    #endregion

    public MultipleMemberEqualityComparer(params IEqualityComparer<T>[] equalityComparers)
    {
        ArgumentNullException.ThrowIfNull(equalityComparers);
        Assert.IsInRange(equalityComparers.Length > 0);
        //ArgumentNullException.ThrowIfNull(Contract.ForAll(equalityComparers, c => c != null));

        _equalityComparers = equalityComparers;
    }

    public bool Equals(T x, T y)
    {
        return _equalityComparers.All(c => c.Equals(x, y));
    }

    public int GetHashCode(T obj)
    {
        var hashCodes = _equalityComparers.Select(c => c.GetHashCode(obj));
        var hashCode = hashCodes.Aggregate(CombineHashCodes);
        return hashCode;
    }

    private static int CombineHashCodes(int h1, int h2)
    {
        return ((h1 << 5) + h1) ^ h2;
    }
}