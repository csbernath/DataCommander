using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct DateTimeInterval
{
    public readonly DateTime Start;
    public readonly DateTime End;

    public DateTimeInterval(DateTime start, DateTime end)
    {
        Assert.IsTrue(start <= end);
        Start = start;
        End = end;
    }

    [Pure]
    public readonly bool Contains(DateTimeInterval other) => Start <= other.Start && other.End <= End;

    [Pure]
    public readonly DateTimeInterval? Intersect(DateTimeInterval other)
    {
        var start = ElementPair.Max(Start, other.Start);
        var end = ElementPair.Min(End, other.End);
        var intersects = start < end;
        return intersects
            ? new DateTimeInterval(start, end)
            : (DateTimeInterval?) null;
    }

    [Pure]
    public readonly bool Intersects(DateTimeInterval other)
    {
        var start = ElementPair.Max(Start, other.Start);
        var end = ElementPair.Min(End, other.End);
        var intersects = start < end;
        return intersects;
    }

    [Pure]
    public readonly TimeSpan GetLength()
    {
        var length = End - Start;
        return length;
    }

    private static string ToString(DateTime dateTime) => dateTime.ToString("yyyy.MM.dd. HH:mm:ss");

    private readonly string DebuggerDisplay => $"{ToString(Start)}-{ToString(End)}";
}