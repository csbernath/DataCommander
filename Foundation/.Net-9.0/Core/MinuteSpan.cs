﻿using System;
using System.Diagnostics;

namespace Foundation.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct MinuteSpan(int value) : IComparable<MinuteSpan>
{
    private readonly int _value = value;

    public static readonly MinuteSpan Zero = new(0);

    public int TotalMinutes => _value;

    public static bool operator ==(MinuteSpan x, MinuteSpan y) => x._value == y._value;
    public static bool operator !=(MinuteSpan x, MinuteSpan y) => !(x == y);
    public static bool operator <(MinuteSpan x, MinuteSpan y) => x._value < y._value;
    public static bool operator <=(MinuteSpan x, MinuteSpan y) => x._value <= y._value;
    public static bool operator >(MinuteSpan x, MinuteSpan y) => x._value > y._value;
    public static bool operator >=(MinuteSpan x, MinuteSpan y) => x._value >= y._value;
    public static MinuteSpan operator +(MinuteSpan x, MinuteSpan y) => new(x._value + y._value);
    public static MinuteSpan operator -(MinuteSpan x, MinuteSpan y) => new(x._value - y._value);

    public static MinuteSpan FromMinutes(int minutes) => new(minutes);

    public static MinuteSpan FromHours(double hours)
    {
        var minutes = (int) (TimeSpan.MinutesPerHour * hours);
        return new MinuteSpan(minutes);
    }

    public int CompareTo(MinuteSpan other) => _value.CompareTo(other._value);

    public string DebuggerDisplay
    {
        get
        {
            var hours = _value / TimeSpan.MinutesPerHour;
            var minutes = _value - TimeSpan.MinutesPerHour * hours;

            var debuggerDisplay = $"{hours}:{minutes:D2}";

            return debuggerDisplay;
        }
    }

    public override bool Equals(object? value) => value is MinuteSpan minuteSpan && minuteSpan._value == _value; 

    public bool Equals(MinuteSpan other) => _value == other._value;
    public override int GetHashCode() => _value;
}