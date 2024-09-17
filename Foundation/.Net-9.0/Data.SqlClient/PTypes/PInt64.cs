using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

/// <summary>
/// 
/// </summary>
public struct PInt64 : INullable
{
    private SqlInt64 _sql;

    /// <summary>
    /// 
    /// </summary>
    public static readonly PInt64 Null = new(PValueType.Null);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PInt64 Default = new(PValueType.Default);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PInt64 Empty = new(PValueType.Empty);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public PInt64(long value)
    {
        _sql = value;
        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public PInt64(SqlInt64 value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    private PInt64(PValueType type)
    {
        ValueType = type;
        _sql = SqlInt64.Null;
    }

    public static implicit operator PInt64(long value) => new(value);
    public static implicit operator PInt64(long? value) => value != null ? new PInt64(value.Value) : Null;
    public static implicit operator PInt64(SqlInt64 value) => new(value);
    public static implicit operator long(PInt64 value) => (long)value._sql;

    public static bool operator ==(PInt64 x, PInt64 y)
    {
        bool isEqual = x.ValueType == y.ValueType;

        if (isEqual)
        {
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;
        }

        return isEqual;
    }

    public static bool operator !=(PInt64 x, PInt64 y) => !(x == y);

    public static PInt64 Parse(string s, PValueType type)
    {
        PInt64 sp = string.IsNullOrEmpty(s)
            ? new PInt64(type)
            : SqlInt64.Parse(s);
        return sp;
    }

    public override readonly bool Equals(object y)
    {
        bool equals = y is PInt64;
        if (equals)
            equals = this == (PInt64)y;
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
                _sql = SqlInt64.Null;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
                _sql = SqlInt64.Null;
            }
            else
            {
                _sql = (SqlInt64)value;
                ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            }
        }
    }

    public override string ToString() => _sql.ToString();
}