using System.Diagnostics.CodeAnalysis;

namespace Foundation.Configuration;

public delegate bool TryGetValue<in TKey, TValue>(TKey key, [MaybeNullWhen(false)] out TValue value);