using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct SmallDateInterval
{
    public readonly SmallDate Start;
    public readonly SmallDate End;

    public SmallDateInterval(SmallDate start, SmallDate end)
    {
        Assert.IsTrue(start <= end);
        Start = start;
        End = end;
    }

    [Pure]
    public readonly bool Contains(SmallDate date)
    {
        var contains = Start <= date && date <= End;
        return contains;
    }

    [Pure]
    public readonly bool Contains(SmallDateInterval other)
    {
        var contains = Start <= other.Start && other.End <= End;
        return contains;
    }

    [Pure]
    public readonly SmallDateInterval? Intersect(SmallDateInterval other)
    {
        var start = ElementPair.Max(Start, other.Start);
        var end = ElementPair.Min(End, other.End);
        var intersects = start <= end;
        return intersects
            ? new SmallDateInterval(start, end)
            : (SmallDateInterval?) null;
    }

    [Pure]
    public readonly bool Intersects(SmallDateInterval other)
    {
        var start = ElementPair.Max(Start, other.Start);
        var end = ElementPair.Min(End, other.End);
        var intersects = start <= end;
        return intersects;
    }

    [Pure]
    public readonly int GetLength()
    {
        var length = End - Start + 1;
        return length;
    }

    [Pure]
    public readonly IEnumerable<SmallDate> GetDates()
    {
        for (var date = Start; date <= End; date = date.AddDays(1))
            yield return date;
    }

    [Pure]
    public readonly DateTimeInterval ToFoundationDateTimeInterval()
    {
        var start = Start.ToDateTime();
        var end = End.ToDateTime().AddDays(1);
        return new DateTimeInterval(start, end);
    }

    private readonly string DebuggerDisplay => $"{Start.DebuggerDisplay}-{End.DebuggerDisplay}";
}