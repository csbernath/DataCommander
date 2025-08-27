using Xunit;

namespace Foundation.Diagnostics.Test;

public class RoundTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var value = 123.456789m;

        // Act
        var rounded = value.Round(5, 2);

        // Assert
        Assert.Equal(123.46m, rounded);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var value = 123m;

        // Act
        var rounded = value.Round(5, 2);

        // Assert
        Assert.Equal(123m, rounded);
    }
    
    [Fact]
    public void Test3()
    {
        // Arrange
        var value = 0.123456m;

        // Act
        var rounded = value.Round(10, 6);

        // Assert
        Assert.Equal(value, rounded);
    }

    [Fact]
    public void Test4()
    {
        // Arrange
        var value = 0.123456m;

        // Act
        var rounded = value.Round(10, 5);

        // Assert
        Assert.Equal(0.12346m, rounded);
    }
}