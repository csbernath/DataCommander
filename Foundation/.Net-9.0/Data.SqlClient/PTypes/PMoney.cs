using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public struct PMoney : INullable
{
    private SqlMoney _sql;

    public static readonly PMoney Null = new(PValueType.Null);
    public static readonly PMoney Default = new(PValueType.Default);
    public static readonly PMoney Empty = new(PValueType.Empty);

    public PMoney(decimal value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PMoney(SqlMoney value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PMoney(PValueType type)
    {
        ValueType = type;
        _sql = SqlMoney.Null;
    }

    public static implicit operator PMoney(decimal value) => new(value);
    public static implicit operator PMoney(SqlMoney value) => new(value);
    public static implicit operator decimal(PMoney value) => (decimal)value._sql;

    public static bool operator ==(PMoney x, PMoney y)
    {
        var isEqual = x.ValueType == y.ValueType;
        if (isEqual)
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;

        return isEqual;
    }

    public static bool operator !=(PMoney x, PMoney y) => !(x == y);

    public static PMoney Parse(string s, PValueType type)
    {
        var sp = string.IsNullOrEmpty(s) ? new PMoney(type) : SqlMoney.Parse(s);
        return sp;
    }

    public override readonly bool Equals(object y)
    {
        var equals = y is PMoney;
        if (equals)
            equals = this == (PMoney)y;
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
                _sql = SqlMoney.Null;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
                _sql = SqlMoney.Null;
            }
            else
            {
                _sql = (SqlMoney)value;
                ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            }
        }
    }

    public override string ToString() => _sql.ToString();
}