using System;

namespace Foundation.Diagnostics.Measurement;

public static class DoubleExtensions
{
    public static int GetIntegerDigitCountByLog10(this double value)
    {
        var integerDigitCount = value switch
        {
            > 0 => value.GetPositiveIntegerDigitCountByLog10(),
            0 => 1,
            _ => (-value).GetPositiveIntegerDigitCountByLog10()
        };
        return integerDigitCount;
    }

    private static int GetPositiveIntegerDigitCountByLog10(this double value)
    {
        var log10 = Math.Log10(value);
        var integerDigitCount = (int)Math.Floor(log10) + 1;
        return integerDigitCount;
    }
}