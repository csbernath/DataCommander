using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public struct PByte : INullable
{
    private SqlByte _sql;

    public PByte(byte value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PByte(SqlByte value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PByte(PValueType type)
    {
        ValueType = type;
        _sql = SqlByte.Null;
    }

    public static implicit operator PByte(byte value) => new(value);
    public static implicit operator PByte(byte? value) => value != null ? new PByte(value.Value) : Null;
    public static implicit operator PByte(SqlByte value) => new(value);
    public static implicit operator byte(PByte value) => (byte)value._sql;

    public static bool operator ==(PByte x, PByte y)
    {
        bool isEqual = x.ValueType == y.ValueType;

        if (isEqual)
        {
            if (x.ValueType == PValueType.Value)
            {
                isEqual = x._sql.Value == y._sql.Value;
            }
        }

        return isEqual;
    }

    public static bool operator !=(PByte x, PByte y)
    {
        return !(x == y);
    }

    public static PByte Parse(string s, PValueType type)
    {
        PByte sp = string.IsNullOrEmpty(s) ? new PByte(type) : SqlByte.Parse(s);
        return sp;
    }

    public override readonly bool Equals(object y)
    {
        bool equals = y is PByte;

        if (equals)
            equals = this == (PByte)y;

        return equals;
    }

    public override int GetHashCode()
    {
        int hashCode = _sql.GetHashCode();
        return hashCode;
    }

    public PValueType ValueType { get; private set; }

    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public object Value
    {
        readonly get
        {
            object value = ValueType switch
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
                _sql = SqlByte.Null;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
                _sql = SqlByte.Null;
            }
            else
            {
                _sql = (SqlByte)value;
                ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            }
        }
    }

    public override string ToString() => _sql.ToString();
    public static readonly PByte Null = new(PValueType.Null);
    public static readonly PByte Default = new(PValueType.Default);
    public static readonly PByte Empty = new(PValueType.Empty);
}