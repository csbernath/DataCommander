﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Core;

/// <summary>
/// 16 bit date type: 1900-01-01 - 2079-06-06
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct SmallDate : IComparable<SmallDate>, IEquatable<SmallDate>
{
    private readonly ushort _value;

    public static readonly DateTime MinDateTime = new(1900, 1, 1);
    public static readonly DateTime MaxDateTime = new(2079, 06, 06);
    public static readonly SmallDate MinValue = new(ushort.MinValue);
    public static readonly SmallDate MaxValue = new(ushort.MaxValue);

    [CLSCompliant(false)]
    public SmallDate(ushort value) => _value = value;

    public SmallDate(DateTime dateTime)
    {
        Assert.IsInRange(dateTime.TimeOfDay == TimeSpan.Zero);
        _value = ToSmallDateValue(dateTime);
    }

    public SmallDate(int year, int month, int day)
    {
        var dateTime = new DateTime(year, month, day);
        _value = ToSmallDateValue(dateTime);
    }

    //public ushort Value => _value;
    public readonly int Year => ToDateTime(_value).Year;
    public readonly int Month => ToDateTime(_value).Month;
    public readonly int Day => ToDateTime(_value).Day;

    public static explicit operator SmallDate(DateTime dateTime)
    {
        var value = ToSmallDateValue(dateTime);
        return new SmallDate(value);
    }

    public static SmallDate operator +(SmallDate smallDate, int value)
    {
        var result = smallDate._value + value;
        if (result < ushort.MinValue || ushort.MaxValue < result)
            throw new OverflowException();
        return new SmallDate((ushort) result);
    }

    public static int operator -(SmallDate smallDate1, SmallDate smallDate2) => smallDate1._value - smallDate2._value;
    public static bool operator <=(SmallDate smallDate1, SmallDate smallDate2) => smallDate1._value <= smallDate2._value;
    public static bool operator >=(SmallDate smallDate1, SmallDate smallDate2) => smallDate1._value >= smallDate2._value;
    public static bool operator <(SmallDate smallDate1, SmallDate smallDate2) => smallDate1._value < smallDate2._value;
    public static bool operator >(SmallDate smallDate1, SmallDate smallDate2) => smallDate1._value > smallDate2._value;

    [Pure]
    public readonly bool In(SmallDateInterval interval) => interval.Start._value <= _value && _value <= interval.End._value;

    [Pure]
    public readonly DateTime ToDateTime() => ToDateTime(_value);

    [Pure]
    public readonly SmallDate AddDays(short value)
    {
        var valueInt32 = _value + value;
        ushort valueUInt16;

        if (valueInt32 < ushort.MinValue)
            valueUInt16 = ushort.MinValue;
        else if (ushort.MaxValue < valueInt32)
            valueUInt16 = ushort.MaxValue;
        else
            valueUInt16 = (ushort) valueInt32;

        return new SmallDate(valueUInt16);
    }

    [Pure]
    public SmallDate Next => AddDays(1);
        
    [Pure]
    public SmallDate Previous => AddDays(-1);

    [Pure]
    public SmallDate AddMonths(int months)
    {
        var dateTime = ToDateTime();
        dateTime = dateTime.AddMonths(months);
        var result = ToSmallDateValue(dateTime);
        return new SmallDate(result);
    }

    public override readonly bool Equals(object? value) => value is SmallDate smallDate && smallDate._value == _value; 

    public override readonly int GetHashCode() => _value;

    public override readonly string ToString()
    {
        var dateTime = ToDateTime(_value);
        return dateTime.ToShortDateString();
    }

    public readonly string ToString(string format)
    {
        var dateTime = ToDateTime(_value);
        return dateTime.ToString(format);
    }

    public readonly int CompareTo(SmallDate other) => _value.CompareTo(other._value);

    readonly bool IEquatable<SmallDate>.Equals(SmallDate other) => _value == other._value;

    public static bool operator ==(SmallDate x, SmallDate y) => x.Equals(y);

    public static bool operator !=(SmallDate x, SmallDate y) => !x.Equals(y);

    internal string DebuggerDisplay
    {
        get
        {
            string debuggerDisplay;

            if (_value == ushort.MinValue)
                debuggerDisplay = $"{ToDateTime():d}(min)";
            else if (_value == ushort.MaxValue)
                debuggerDisplay = $"{ToDateTime():d}(max)";
            else
                debuggerDisplay = $"{ToDateTime():d}";

            return debuggerDisplay;
        }
    }

    [Pure]
    private static DateTime ToDateTime(ushort value) => MinDateTime.AddDays(value);

    private static ushort ToSmallDateValue(DateTime dateTime)
    {
        Assert.IsInRange(MinDateTime <= dateTime);
        Assert.IsInRange(dateTime <= MaxDateTime);

        var timeSpan = dateTime - MinDateTime;
        var totalDays = timeSpan.TotalDays;
        var value = (ushort) totalDays;
        return value;
    }
}