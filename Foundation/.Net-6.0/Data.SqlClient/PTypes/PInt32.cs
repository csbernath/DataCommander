using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.PTypes;

public struct PInt32 : INullable
{
    private SqlInt32 _sql;

    public static readonly PInt32 Null = new(PValueType.Null);
    public static readonly PInt32 Default = new(PValueType.Default);
    public static readonly PInt32 Empty = new(PValueType.Empty);

    [DebuggerStepThrough]
    public PInt32(int value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    [DebuggerStepThrough]
    public PInt32(int? value)
    {
        _sql = ToSqlInt32(value);
        ValueType = value != null ? PValueType.Value : PValueType.Null;
    }

    [DebuggerStepThrough]
    public PInt32(SqlInt32 value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PInt32(PValueType type)
    {
        ValueType = type;
        _sql = SqlInt32.Null;
    }

    [DebuggerStepThrough]
    public static implicit operator PInt32(int value) => new(value);

    [DebuggerStepThrough]
    public static implicit operator PInt32(int? value) => new(value);

    [DebuggerStepThrough]
    public static implicit operator PInt32(SqlInt32 value) => new(value);

    public static implicit operator int(PInt32 value) => (int)value._sql;

    public static bool operator ==(PInt32 x, PInt32 y)
    {
        var isEqual = x.ValueType == y.ValueType;

        if (isEqual)
        {
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;
        }

        return isEqual;
    }

    public static bool operator !=(PInt32 x, PInt32 y) => !(x == y);

    public static PInt32 Parse(string s, PValueType type)
    {
        var sp = string.IsNullOrEmpty(s)
            ? new PInt32(type)
            : SqlInt32.Parse(s);
        return sp;
    }

    public override bool Equals(object obj)
    {
        var equals = obj is PInt32;
        if (equals)
            equals = this == (PInt32)obj;
        return equals;
    }

    public override int GetHashCode()
    {
        var hashCode = _sql.GetHashCode();
        return hashCode;
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
                _sql = SqlInt32.Null;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
                _sql = SqlInt32.Null;
            }
            else
            {
                _sql = (SqlInt32)value;
                ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            }
        }
    }

    public override string ToString() => _sql.ToString();

    private static SqlInt32 ToSqlInt32(int? value) => value ?? SqlInt32.Null;
}