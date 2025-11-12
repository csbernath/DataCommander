using System;
using Foundation.Core;

namespace Foundation.Diagnostics.Measurement;

public static class PowersOf1024
{
    [CLSCompliant(false)] public const ushort Power1 = PowersOf2.Power10;
    [CLSCompliant(false)] public const uint Power2 = Power1 * Power1;
    [CLSCompliant(false)] public const uint Power3 = Power2 * Power1;
    [CLSCompliant(false)] public const ulong Power4 = (ulong)Power3 * Power1;
    [CLSCompliant(false)] public const ulong Power5 = Power4 * Power1;
    [CLSCompliant(false)] public const ulong Power6 = Power5 * Power1;
}