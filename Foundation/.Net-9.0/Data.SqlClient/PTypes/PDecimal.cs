using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public readonly struct PDecimal : INullable
{
    private readonly SqlDecimal _sql;

    public static readonly PDecimal Null = new(PValueType.Null);
    public static readonly PDecimal Default = new(PValueType.Default);
    public static readonly PDecimal Empty = new(PValueType.Empty);

    public PDecimal(decimal value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PDecimal(SqlDecimal value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PDecimal(PValueType type)
    {
        ValueType = type;
        _sql = SqlDecimal.Null;
    }

    public static implicit operator PDecimal(decimal value) => new(value);
    public static implicit operator PDecimal(SqlDecimal value) => new(value);

    public static implicit operator decimal(PDecimal value)
    {
        return (decimal)value._sql;
    }

    public static bool operator ==(PDecimal x, PDecimal y)
    {
        var isEqual = x.ValueType == y.ValueType;

        if (isEqual)
        {
            if (x.ValueType == PValueType.Value)
            {
                isEqual = x._sql.Value == y._sql.Value;
            }
        }

        return isEqual;
    }
    
    public static bool operator !=(PDecimal x, PDecimal y) => !(x == y);

    public static PDecimal Parse(string s, PValueType type) =>
        string.IsNullOrEmpty(s)
            ? new PDecimal(type)
            : SqlDecimal.Parse(s);

    public override readonly bool Equals(object y)
    {
        var equals = y is PDecimal;

        if (equals)
        {
            equals = this == (PDecimal)y;
        }

        return equals;
    }

    public override readonly int GetHashCode() => _sql.GetHashCode();

    public PValueType ValueType { get; }

    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public readonly object Value
    {
        get
        {
            object value = ValueType switch
            {
                PValueType.Null => DBNull.Value,
                PValueType.Value => _sql,
                _ => null,
            };
            return value;
        }

        //set
        //{
        //    if (value == null)
        //    {
        //        ValueType = PValueType.Default;
        //        _sql = SqlDecimal.Null;
        //    }
        //    else if (value == DBNull.Value)
        //    {
        //        ValueType = PValueType.Null;
        //        _sql = SqlDecimal.Null;
        //    }
        //    else
        //    {
        //        _sql = (SqlDecimal) value;
        //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
        //    }
        //}
    }

    public override readonly string ToString() => _sql.ToString();
}