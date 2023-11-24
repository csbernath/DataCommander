using System;
using System.Diagnostics;

namespace Foundation.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct SmallTime : IEquatable<SmallTime>, IComparable<SmallTime>
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

    public short Value => _value;

    public override string ToString()
    {
        var timeSpan = ToTimeSpan(_value);
        return timeSpan.ToString();
    }

    private static TimeSpan ToTimeSpan(short value)
    {
        return TimeSpan.FromSeconds(value);
    }

    internal string DebuggerDisplay
    {
        get
        {
            var hours = _value / 60;
            var minutes = _value - 60 * hours;
            return $"{hours:D2}:{minutes:D2}";
        }
    }

    public bool Equals(SmallTime other)
    {
        return _value == other._value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is SmallTime && Equals((SmallTime) obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public int CompareTo(SmallTime other)
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