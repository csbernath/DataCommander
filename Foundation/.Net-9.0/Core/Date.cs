using Foundation.Assertions;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Foundation.Core;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public readonly struct Date : IComparable<Date>, IEquatable<Date>
{
    private readonly ulong _value;

    public static readonly DateTime MinDateTime = new(1900, 1, 1);
    public static readonly DateTime MaxDateTime = new(9999, 12, 31);
    public static readonly Date MinValue = new(MinDateTime);
    public static readonly Date MaxValue = new(MaxDateTime);

    [CLSCompliant(false)]
    public Date(ulong value) => _value = value;

    public Date(DateTime dateTime)
    {
        Assert.IsInRange(dateTime.TimeOfDay == TimeSpan.Zero);
        _value = ToDateValue(dateTime);
    }

    public static long operator -(Date date1, Date date2) => (long) date1._value - (long) date2._value;
    public static bool operator <=(Date date1, Date date2) => date1._value <= date2._value;
    public static bool operator >=(Date date1, Date date2) => date1._value >= date2._value;
    public static bool operator <(Date date1, Date date2) => date1._value < date2._value;
    public static bool operator >(Date date1, Date date2) => date1._value > date2._value;

    [Pure]
    public readonly bool In(DateInterval interval) => interval.Start._value <= _value && _value <= interval.End._value;

    public readonly int CompareTo(Date other) => _value.CompareTo(other._value);

    [Pure]
    public readonly DateTime ToDateTime() => ToDateTime(_value);

    [Pure]
    public readonly Date AddDays(long value)
    {
        var valueInt64 = (long) _value + value;
        ulong valueUInt64;

        if (valueInt64 < 0)
            valueUInt64 = 0;
        else if (MaxValue._value < (ulong) valueInt64)
            valueUInt64 = MaxValue._value;
        else
            valueUInt64 = (ulong) valueInt64;

        return new Date(valueUInt64);
    }

    [Pure]
    public Date Next => AddDays(1);

    [Pure]
    public Date Previous => AddDays(-1);

    [Pure]
    private static DateTime ToDateTime(ulong value) => MinDateTime.AddDays(value);

    private static ulong ToDateValue(DateTime dateTime)
    {
        var timeSpan = dateTime - MinDateTime;
        var totalDays = timeSpan.TotalDays;
        var value = (ulong) totalDays;
        return value;
    }

    readonly bool IEquatable<Date>.Equals(Date other) => _value == other._value;
    public static bool operator ==(Date x, Date y) => x.Equals(y);
    public static bool operator !=(Date x, Date y) => !x.Equals(y);

    internal string DebuggerDisplay
    {
        get
        {
            string debuggerDisplay;

            if (_value == MinValue._value)
                debuggerDisplay = $"{ToDateTime():d}(min)";
            else if (_value == MaxValue._value)
                debuggerDisplay = $"{ToDateTime():d}(max)";
            else
                debuggerDisplay = $"{ToDateTime():d}";

            return debuggerDisplay;
        }
    }

    public override bool Equals(object obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}