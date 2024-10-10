using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Foundation.Core;

public sealed class TypeDictionary<TValue>
{
    private readonly Dictionary<Type, TValue> _selections = [];

    public void Add<T>(TValue value)
    {
        var type = typeof(T);
        _selections.Add(type, value);
    }

    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value)
    {
        var type = typeof(T);
        return _selections.TryGetValue(type, out value);
    }

    public TValue? GetValueOrDefault(Type type) => _selections.GetValueOrDefault(type);

    public TValue? GetValueOrDefault<T>()
    {
        var type = typeof(T);
        return GetValueOrDefault(type);
    }
}