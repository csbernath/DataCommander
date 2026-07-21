using System;

namespace Foundation.Core;

public static class PowersOf10
{
    public const byte Power0 = 1;
    public const byte Power1 = 10;
    public const byte Power2 = Power1 * Power1;
    [CLSCompliant(false)] public const ushort Power3 = Power2 * Power1;
    [CLSCompliant(false)] public const ushort Power4 = Power3 * Power1;
    [CLSCompliant(false)] public const uint Power5 = Power4 * Power1;
    [CLSCompliant(false)] public const uint Power6 = Power5 * Power1;
    [CLSCompliant(false)] public const uint Power7 = Power6 * Power1;
    [CLSCompliant(false)] public const uint Power8 = Power7 * Power1;
    [CLSCompliant(false)] public const uint Power9 = Power8 * Power1;
    [CLSCompliant(false)] public const ulong Power10 = (ulong)Power9 * Power1;
    [CLSCompliant(false)] public const ulong Power11 = Power10 * Power1;
    [CLSCompliant(false)] public const ulong Power12 = Power11 * Power1;
    [CLSCompliant(false)] public const ulong Power13 = Power12 * Power1;
    [CLSCompliant(false)] public const ulong Power14 = Power13 * Power1;
    [CLSCompliant(false)] public const ulong Power15 = Power14 * Power1;
    [CLSCompliant(false)] public const ulong Power16 = Power15 * Power1;
    [CLSCompliant(false)] public const ulong Power17 = Power16 * Power1;
    [CLSCompliant(false)] public const ulong Power18 = Power17 * Power1;

    [CLSCompliant(false)]
    public static readonly ulong[] Array =
    [
        Power0,
        Power1,
        Power2,
        Power3,
        Power4,
        Power5,
        Power6,
        Power7,
        Power0,
        Power9,
        Power10,
        Power11,
        Power12,
        Power13,
        Power14,
        Power15,
        Power16,
        Power17,
        Power18
    ];
}