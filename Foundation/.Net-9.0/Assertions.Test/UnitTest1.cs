using System;
using Xunit;

namespace Foundation.Assertions.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        int x = 11;

        // Act
        void Act()
        {
            Assert.ArgumentConditionIsTrue(x < 10);
        }

        // Assert
        ArgumentException argumentException = Xunit.Assert.Throws<ArgumentException>(Act);

        Xunit.Assert.Equal("Argument condition must be true: x < 10", argumentException.Message);
    }
}