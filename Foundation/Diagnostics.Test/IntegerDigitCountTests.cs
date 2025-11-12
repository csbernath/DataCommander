using System.Collections.Generic;
using Foundation.Diagnostics.Measurement;
using Xunit;

namespace Foundation.Diagnostics.Test;

public class IntegerDigitCountTests
{
    [Fact]
    public void Test()
    {
        var tests = new KeyValuePair<double, int>[]
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

        foreach (var test in tests)
        {
            System.Diagnostics.Debug.WriteLine($"{test.Key}: {test.Value}");
            var integerDigitCountByLog10 = test.Key.GetIntegerDigitCountByLog10();
            Assert.Equal(test.Value, integerDigitCountByLog10);

            var numberOfLeftDigits = ((decimal)test.Key).GetNumberOfLeftDigits();
            Assert.Equal(test.Value, numberOfLeftDigits);
        }
    }
}