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
        bool contains = Start <= date && date <= End;
        return contains;
    }

    [Pure]
    public readonly bool Contains(SmallDateInterval other)
    {
        bool contains = Start <= other.Start && other.End <= End;
        return contains;
    }

    [Pure]
    public readonly SmallDateInterval? Intersect(SmallDateInterval other)
    {
        SmallDate start = ElementPair.Max(Start, other.Start);
        SmallDate end = ElementPair.Min(End, other.End);
        bool intersects = start <= end;
        return intersects
            ? new SmallDateInterval(start, end)
            : (SmallDateInterval?) null;
    }

    [Pure]
    public readonly bool Intersects(SmallDateInterval other)
    {
        SmallDate start = ElementPair.Max(Start, other.Start);
        SmallDate end = ElementPair.Min(End, other.End);
        bool intersects = start <= end;
        return intersects;
    }

    [Pure]
    public readonly int GetLength()
    {
        int length = End - Start + 1;
        return length;
    }

    [Pure]
    public readonly IEnumerable<SmallDate> GetDates()
    {
        for (SmallDate date = Start; date <= End; date = date.AddDays(1))
            yield return date;
    }

    [Pure]
    public readonly DateTimeInterval ToFoundationDateTimeInterval()
    {
        System.DateTime start = Start.ToDateTime();
        System.DateTime end = End.ToDateTime().AddDays(1);
        return new DateTimeInterval(start, end);
    }

    private readonly string DebuggerDisplay => $"{Start.DebuggerDisplay}-{End.DebuggerDisplay}";
}