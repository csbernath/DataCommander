using System;
using System.Diagnostics;

namespace Foundation.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct SmallTime : IEquatable<SmallTime>, IComparable<SmallTime>
{
    private readonly short _value;
    public static readonly SmallTime MinValue = new(short.MinValue);
    public static readonly SmallTime MaxValue = new(short.MaxValue);

    public SmallTime(short value)
    {
        _value = value;
    }

    public SmallTime(int hours, int minutes)
    {
        _value = (short) (60 * hours + minutes);
    }

    public readonly short Value => _value;

    public override readonly string ToString()
    {
        TimeSpan timeSpan = ToTimeSpan(_value);
        return timeSpan.ToString();
    }

    private static TimeSpan ToTimeSpan(short value)
    {
        return TimeSpan.FromSeconds(value);
    }

    internal readonly string DebuggerDisplay
    {
        get
        {
            int hours = _value / 60;
            int minutes = _value - 60 * hours;
            return $"{hours:D2}:{minutes:D2}";
        }
    }

    public readonly bool Equals(SmallTime other)
    {
        return _value == other._value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is SmallTime && Equals((SmallTime) obj);
    }

    public override readonly int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public readonly int CompareTo(SmallTime other)
    {
        return _value.CompareTo(other._value);
    }

    public static bool operator ==(SmallTime x, SmallTime y) => x.Equals(y);
    public static bool operator !=(SmallTime x, SmallTime y) => !(x == y);
    public static bool operator <(SmallTime x, SmallTime y) => x.CompareTo(y) < 0;
    public static bool operator >(SmallTime x, SmallTime y) => x.CompareTo(y) > 0;
    public static bool operator <=(SmallTime x, SmallTime y) => x.CompareTo(y) <= 0;
    public static bool operator >=(SmallTime x, SmallTime y) => x.CompareTo(y) >= 0;
}