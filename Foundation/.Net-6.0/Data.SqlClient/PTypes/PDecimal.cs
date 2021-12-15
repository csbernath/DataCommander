using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes;

public struct PDecimal : INullable
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator decimal(PDecimal value)
    {
        return (decimal)value._sql;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(PDecimal x, PDecimal y)
    {
        return !(x == y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static PDecimal Parse(string s, PValueType type)
    {
        PDecimal sp;

        sp = string.IsNullOrEmpty(s) ? new PDecimal(type) : SqlDecimal.Parse(s);

        return sp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public override bool Equals(object y)
    {
        var equals = y is PDecimal;

        if (equals)
        {
            equals = this == (PDecimal)y;
        }

        return equals;
    }

    public override int GetHashCode() => _sql.GetHashCode();

    public PValueType ValueType { get; }

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
                case PValueType.Null:
                    value = DBNull.Value;
                    break;

                case PValueType.Value:
                    value = _sql;
                    break;

                default:
                    value = null;
                    break;
            }

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

    public override string ToString() => _sql.ToString();
}