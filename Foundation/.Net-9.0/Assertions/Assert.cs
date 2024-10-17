using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Foundation.Assertions;

public static class Assert
{
    public static void AreEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>? =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);

    public static void AreNotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>? =>
        ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);    

    public static void ArgumentConditionIsTrue(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentException(conditionString);
    }

    public static void IsGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T> =>
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);

    public static void IsGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T> =>
        ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);

    public static void IsInRange(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new ArgumentOutOfRangeException(conditionString);
    }

    public static void IsLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T> =>
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);    
    
    public static void IsLessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T> =>
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);

    public static void IsNotEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) =>
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);

    public static void IsNotWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) =>
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);

    public static void IsNotNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) =>
        ArgumentNullException.ThrowIfNull(argument, paramName);

    public static void IsNull<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? argumentString = null) where T : class
    {
        if (argument != null)
        {
            var message = $"Argument must be null: {argumentString}";
            throw new ArgumentException(message);
        }
    }

    public static void IsPositive<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : INumberBase<T> =>
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);

    public static void IsPositiveOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : INumberBase<T> =>
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);

    public static void IsTrue(bool condition)
    {
        if (!condition)
            throw new ArgumentException("Assert.IsTrue failed.");
    }

    public static void IsValidOperation(bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionString = null)
    {
        if (!condition)
            throw new InvalidOperationException(conditionString);
    }
}