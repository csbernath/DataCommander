using System;
using System.Runtime.CompilerServices;

namespace Foundation.Assertions;

public static class Assert
{
    public static void AreEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>? =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);

    public static void IsNotNull(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) =>
        ArgumentNullException.ThrowIfNull(argument, paramName);

    public static void IsTrue(bool condition)
    {
        if (!condition)
            throw new ArgumentException("Assert.IsTrue failed.");
    }

    public static void ArgumentConditionIsTrue(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentException(conditionString);
    }

    public static void IsNull<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? argumentString = null) where T : class
    {
        if (argument != null)
        {
            var message = $"Argument must be null: {argumentString}";
            throw new ArgumentException(message);
        }
    }

    public static void IsInRange(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentOutOfRangeException(conditionString);
    }

    public static void IsGreaterOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T> =>
        ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);

    public static void IsValidOperation(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new InvalidOperationException(conditionString);
    }
}