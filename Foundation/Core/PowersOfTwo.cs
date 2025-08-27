using System;

namespace Foundation.Core;

public static class PowersOfTwo
{
    public const byte Power1 = 2;
    public const byte Power2 = Power1 * Power1;
    public const byte Power4 = Power2 * Power2;
    [CLSCompliant(false)] public const ushort Power8 = Power4 * Power4;
    [CLSCompliant(false)] public const ushort Power10 = Power8 * Power2;
    [CLSCompliant(false)] public const ulong Power16 = Power8 * Power8;
    [CLSCompliant(false)] public const ulong Power32 = Power16 * Power16;
}
