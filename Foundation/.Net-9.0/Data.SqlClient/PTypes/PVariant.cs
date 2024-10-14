using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public struct PVariant : INullable
{
    private object? _sql;

    public static readonly PVariant Null = new(PValueType.Null);
    public static readonly PVariant Default = new(PValueType.Default);
    public static readonly PVariant Empty = new(PValueType.Empty);

    private PVariant(PValueType type)
    {
        ValueType = type;
        _sql = null;
    }

    public PVariant(object value)
    {
        if (value == null)
            ValueType = PValueType.Default;
        else if (value == DBNull.Value)
            ValueType = PValueType.Null;
        else
            ValueType = PValueType.Value;

        _sql = value;
    }

    public PValueType ValueType { get; private set; }
    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public object? Value
    {
        readonly get
        {
            var value = ValueType switch
            {
                PValueType.Value or PValueType.Null => _sql,
                _ => null,
            };
            return value;
        }

        set
        {
            if (value == null)
            {
                ValueType = PValueType.Default;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
            }
            else
            {
                ValueType = PValueType.Value;
            }

            _sql = value;
        }
    }

    public static implicit operator PVariant(string s) => new(s);
    public readonly override string ToString() => _sql!.ToString()!;
}