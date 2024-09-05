using System;
using System.Runtime.CompilerServices;

namespace Foundation.Assertions;

public static class Assert
{
    public static void IsTrue(bool condition)
    {
        if (!condition)
            throw new ArgumentException("Assert.IsTrue failed.");
    }

    public static void ArgumentConditionIsTrue(bool condition, [CallerArgumentExpression("condition")] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentException(conditionString);
    }

    public static void IsNull<T>(T argument, [CallerArgumentExpression("argument")] string? argumentString = null) where T : class
    {
        if (argument != null)
        {
            var message = $"Argument must be null: {argumentString}";
            throw new ArgumentException(message);
        }
    }

    public static void IsInRange(bool condition, [CallerArgumentExpression("condition")] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentOutOfRangeException(conditionString);
    }

    public static void IsValidOperation(bool condition, [CallerArgumentExpression("condition")] string? conditionString = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(conditionString);
        }
    }
}