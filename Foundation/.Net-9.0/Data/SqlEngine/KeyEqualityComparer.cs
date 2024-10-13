using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class KeyEqualityComparer : IEqualityComparer<Key>
{
    public bool Equals(Key? x, Key? y) => x!.Values.SequenceEqual(y!.Values);

    public int GetHashCode(Key key)
    {
        var hashCode = new HashCode();
        foreach (var value in key.Values)
            hashCode.Add(value);
        return hashCode.ToHashCode();
    }
}