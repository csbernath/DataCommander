using System;
using System.Diagnostics;

namespace Foundation.Core;

[DebuggerDisplay("{" + nameof(_value) + "}")]
public readonly struct NotNullable<T> where T : class
{
    private readonly T _value;

    private NotNullable(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
    }

    public readonly bool HasValue => _value != null;

    public readonly T Value
    {
        get
        {
            if (_value == null)
                throw new InvalidOperationException();

            return _value;
        }
    }

    public static implicit operator NotNullable<T>(T value) => new(value);
    public static implicit operator T(NotNullable<T> notNullable) => notNullable.Value;
}