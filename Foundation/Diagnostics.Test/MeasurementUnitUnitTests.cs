using Foundation.Diagnostics.Measurement;
using Xunit;

namespace Foundation.Diagnostics.Test;

public class MeasurementUnitUnitTests
{
    [Fact]
    public void Test0()
    {
        // Arrange
        var value = 0;

        // Act
        var binaryMetricString = MeasurementUnit.ToBinaryMetricString(value, 2, UnitSymbol.Byte);
        var decimalMetricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("0 B", binaryMetricString);
        Assert.Equal("0 B", decimalMetricString);
    }
    
    [Fact]
    public void Test1()
    {
        // Arrange
        var value = 123.456789d * 1000 * 1000;

        // Act
        var decimalMetricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("123.46 MB", decimalMetricString);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var value = (long)(1000 * 1000 * 1.23456789);

        // Act
        var metricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("1.2346 MB", metricString);
    }

    [Fact]
    public void Test3()
    {
        // Arrange
        var value = (long)(1000 * 1000 * 1.23456789);

        // Act
        var metricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("1.23 MB", metricString);
    }

    [Fact]
    public void Test4()
    {
        // Arrange
        var value = (long)(1000 * 1000 * 1.23456789);

        // Act
        var metricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("1.2 MB", metricString);
    }

    [Fact]
    public void Test5()
    {
        // Arrange
        var value = (long)(1000 * 1000 * 1.23456789);

        // Act
        var metricString = MeasurementUnit.ToDecimalMetricString(value, 0, UnitSymbol.Byte);

        // Assert
        Assert.Equal("1 MB", metricString);
    }

    [Fact]
    public void Test6()
    {
        // Arrange
        var value = (long)(1000 * 1000 * 12.3456789);

        // Act
        var metricString = MeasurementUnit.ToDecimalMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("12.3 MB", metricString);
    }

    [Fact]
    public void Test7()
    {
        // Arrange
        var value = 123.456789 * 1024 * 1024;

        // Act
        var binaryMetricString = MeasurementUnit.ToBinaryMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("123.46 MiB", binaryMetricString);
    }

    [Fact]
    public void Test8()
    {
        // Arrange
        var value = 123d * 1024;

        // Act
        var binaryMetricString = MeasurementUnit.ToBinaryMetricString(value, 2, UnitSymbol.Byte);

        // Assert
        Assert.Equal("123 KiB", binaryMetricString);
    }
    
    [Fact]
    public void Test9()
    {
        // Arrange
        var value = 5.0 / 100;

        // Act
        var binaryMetricString = MeasurementUnit.ToDecimalMetricString(value, 2, "m");

        // Assert
        Assert.Equal("5 cm", binaryMetricString);
    }    
}