using System;

namespace Foundation.Diagnostics.Measurement;

public static class DecimalUnitPrefixes
{
    [CLSCompliant(false)] public static readonly UnitPrefix[] Value =
    [
        new("kilo", "k", PowersOf1000.Power1),
        new("mega", "M", PowersOf1000.Power2),
        new("giga", "G", PowersOf1000.Power3),
        new("tera", "T", PowersOf1000.Power4),
        new("peta", "P", PowersOf1000.Power5),
        new("exa", "E", PowersOf1000.Power6)
    ];
}