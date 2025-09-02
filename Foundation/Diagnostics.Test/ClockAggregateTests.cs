using System;
using System.Diagnostics;
using Foundation.Core.ClockAggregate;
using Xunit;

namespace Foundation.Diagnostics.Test;

public class ClockAggregateTests
{
    [Fact]
    public void F()
    {
        var clockAggregateRoot = ClockAggregateRepository.Singleton.Get();

        DateTimeOffset? previousUtcNow = null;
        
        var stopwatch = Stopwatch.StartNew();

        while (true)
        {
            var utcNow = clockAggregateRoot.GetUtcFromCurrentEnvironmentTickCount64();

            if (previousUtcNow != null && !(previousUtcNow <= utcNow))
            {
                Debug.WriteLine($"{previousUtcNow}, {utcNow}");
            }

            if (stopwatch.ElapsedMilliseconds > 30000)
            {
                break;
            }
            
            previousUtcNow = utcNow;
        }
    }
}