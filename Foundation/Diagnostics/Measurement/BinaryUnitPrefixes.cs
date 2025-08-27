using System;
using Foundation.Core;

namespace Foundation.Diagnostics.Measurement;

public static class BinaryUnitPrefixes
{
    [CLSCompliant(false)] public static readonly UnitPrefix[] Value =
    [
        new("kibi", "Ki", PowersOf1024.Power1),
        new("mebi", "Mi", PowersOf1024.Power2),
        new("gibi", "Gi", PowersOf1024.Power3),
        new("tebi", "Ti", PowersOf1024.Power4),
        new("pebi", "Pi", PowersOf1024.Power5),
        new("exbi", "Ei", PowersOf1024.Power6)
    ];
}