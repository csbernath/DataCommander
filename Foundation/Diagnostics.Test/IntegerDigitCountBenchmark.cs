using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Foundation.Diagnostics.Measurement;

namespace Foundation.Diagnostics.Test;

public class IntegerDigitCountBenchmark
{
    private readonly KeyValuePair<double, int>[] _tests = new KeyValuePair<double, int>[]
    {
        // new(-1, 1),
        new(0, 1),
        new(1.23, 1),
        new(9, 1),
        new(10, 2),
        new(99, 2),
        new(100, 3),
        new(999, 3),
        new(1000, 4),
        new(9999, 4),
        new(10000, 5)
    };
    
    [Params(0, 9)] public int N;    

    [Benchmark]
    public int GetIntegerDigitCountByLog10()
    {
        var integerDigitCountByLog10 = ((double)N).GetIntegerDigitCountByLog10();
        return integerDigitCountByLog10;
    }

    [Benchmark]
    public int GetNumberOfLeftDigits()
    {
        var numberOfLeftDigits = ((decimal)N).GetNumberOfLeftDigits();
        return numberOfLeftDigits;
    }
}