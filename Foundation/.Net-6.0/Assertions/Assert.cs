using System;

namespace Foundation.Assertions;

public static class Assert
{
    public static void IsTrue(bool condition)
    {
        if (!condition)
            throw new ArgumentException();
    }

    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }

    public static void IsNull<T>(T value) where T : class
    {
        if (value != null)
            throw new ArgumentException();
    }

    public static void IsInRange(bool condition)
    {
        if (!condition)
            throw new ArgumentOutOfRangeException();
    }

    public static void IsValidOperation(bool condition)
    {
        if (!condition)
            throw new InvalidOperationException();
    }
}