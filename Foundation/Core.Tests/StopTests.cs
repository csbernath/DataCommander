using System.Diagnostics;
using Foundation.Core;

namespace Core.Tests;

public class StopwatchTimeSpanTests
{
    [Fact]
    public void Test1()
    {
        long ticks = Stopwatch.Frequency * 3;
        int scale = 0;
        var s = StopwatchTimeSpan.ToString(ticks, scale);
        Assert.Equal("00:03", s);
    }
    
    [Fact]
    public void Test2()
    {
        long ticks = Stopwatch.Frequency * 3 - 1;
        int scale = 0;
        var s = StopwatchTimeSpan.ToString(ticks, 9);
        Assert.Equal("00:02.999999900", s);
    }
    
    [Fact]
    public void Test3()
    {
        long ticks = Stopwatch.Frequency * 3 + 1;
        int scale = 0;
        var s = StopwatchTimeSpan.ToString(ticks, 9);
        Assert.Equal("00:03.000000100", s);
    }
    
    [Fact]
    public void Test4()
    {
        long ticks = Stopwatch.Frequency * 3 + 2;
        int scale = 0;
        var s = StopwatchTimeSpan.ToString(ticks, 9);
        Assert.Equal("00:03.000000200", s);
    }
    
    [Fact]
    public void Test5()
    {
        long ticks = (long)(Stopwatch.Frequency * 3.5);
        int scale = 0;
        var s = StopwatchTimeSpan.ToString(ticks, 9);
        Assert.Equal("00:03.500000000", s);
    }    
}