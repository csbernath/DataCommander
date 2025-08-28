using BenchmarkDotNet.Running;

namespace Foundation.Diagnostics.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<IntegerDigitCountBenchmark>();        
    }
}