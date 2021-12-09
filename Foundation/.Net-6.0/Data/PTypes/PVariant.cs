using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes;

public struct PVariant : INullable
{
    private object _sql;

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
    public bool IsNull => ValueType == PValueType.Null;
    public bool IsValue => ValueType == PValueType.Value;
    public bool IsEmpty => ValueType == PValueType.Empty;

    public object Value
    {
        get
        {
            object value;

            switch (ValueType)
            {
                case PValueType.Value:
                case PValueType.Null:
                    value = _sql;
                    break;

                default:
                    value = null;
                    break;
            }

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
    public override string ToString() => _sql.ToString();
}