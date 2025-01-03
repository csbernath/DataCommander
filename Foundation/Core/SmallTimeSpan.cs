using System;

namespace Foundation.Core;

public readonly struct SmallTimeSpan
{
    public static readonly SmallTimeSpan MinValue = new(short.MinValue);
    public static readonly SmallTimeSpan MaxValue = new(short.MaxValue);

    private SmallTimeSpan(short value) => TotalMinutes = value;

    public SmallTimeSpan(TimeSpan timeSpan) => TotalMinutes = ToSmallTimeSpanValue(timeSpan);

    public short TotalMinutes { get; }
    public override readonly string ToString() => ToTimeSpan(TotalMinutes).ToString();

    private static TimeSpan ToTimeSpan(short value) => TimeSpan.FromSeconds(value);
    private static short ToSmallTimeSpanValue(TimeSpan timeSpan) => (short) timeSpan.TotalSeconds;
}