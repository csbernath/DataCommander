using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.SqlClient.PTypes;

public struct PInt16 : INullable
{
    private SqlInt16 _sql;

    [DebuggerStepThrough]
    public PInt16(short value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PInt16(SqlInt16 value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PInt16(PValueType type)
    {
        ValueType = type;
        _sql = SqlInt16.Null;
    }

    [DebuggerStepThrough]
    public static implicit operator PInt16(short value) => new(value);

    public static implicit operator PInt16(SqlInt16 value) => new(value);

    public static implicit operator short(PInt16 value) => (short)value._sql;

    public static bool operator ==(PInt16 x, PInt16 y)
    {
        var isEqual = x.ValueType == y.ValueType;

        if (isEqual)
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;

        return isEqual;
    }

    public static bool operator !=(PInt16 x, PInt16 y) => !(x == y);

    public static PInt16 Parse(string s, PValueType type)
    {
        var sp = string.IsNullOrEmpty(s) ? new PInt16(type) : SqlInt16.Parse(s);
        return sp;
    }

    public readonly override bool Equals(object? y)
    {
        var equals = y is PInt16;

        if (equals)
            equals = this == (PInt16)y!;

        return equals;
    }

    public override int GetHashCode()
    {
        var hashCode = _sql.GetHashCode();
        return hashCode;
    }

    public PValueType ValueType { get; private set; }
    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public object? Value
    {
        get
        {
            object? value = ValueType switch
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
                _sql = SqlInt16.Null;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
                _sql = SqlInt16.Null;
            }
            else
            {
                _sql = (SqlInt16)value;
                ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            }
        }
    }

    public override string ToString() => _sql.ToString();
    public static readonly PInt16 Null = new(PValueType.Null);
    public static readonly PInt16 Default = new(PValueType.Default);
    public static readonly PInt16 Empty = new(PValueType.Empty);
}