using System;

namespace Foundation.Core;

public static class MemoryExtensions
{
    public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, int start)
    {
        var slice = span.Slice(start);
        var indexOf = slice.IndexOf(value);
        if (indexOf > 0)
            indexOf += start;
        return indexOf;
    }

    public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value, int start)
    {
        var slice = span.Slice(0, start);
        var lastIndexOf = slice.LastIndexOf(value);
        return lastIndexOf;
    }
}