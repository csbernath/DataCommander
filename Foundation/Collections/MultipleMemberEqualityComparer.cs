﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections;

public sealed class MultipleMemberEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly IEqualityComparer<T>[] _equalityComparers;

    public MultipleMemberEqualityComparer(params IEqualityComparer<T>[] equalityComparers)
    {
        ArgumentNullException.ThrowIfNull(equalityComparers);
        Assert.IsInRange(equalityComparers.Length > 0);
        //ArgumentNullException.ThrowIfNull(Contract.ForAll(equalityComparers, c => c != null));

        _equalityComparers = equalityComparers;
    }

    public bool Equals(T? x, T? y) => _equalityComparers.All(c => c.Equals(x, y));

    public int GetHashCode([DisallowNull] T obj)
    {
        var hashCodes = _equalityComparers.Select(c => c.GetHashCode(obj));
        var hashCode = hashCodes.Aggregate(CombineHashCodes);
        return hashCode;
    }

    private static int CombineHashCodes(int h1, int h2) => ((h1 << 5) + h1) ^ h2;
}