﻿using System;
using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public struct PGuid : INullable
{
    private readonly SqlGuid _sql;

    public static readonly PGuid Null = new(PValueType.Null);
    public static readonly PGuid Default = new(PValueType.Default);
    public static readonly PGuid Empty = new(PValueType.Empty);

    public PGuid(Guid value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PGuid(SqlGuid value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PGuid(PValueType type)
    {
        ValueType = type;
        _sql = SqlGuid.Null;
    }

    public static implicit operator PGuid(Guid value) => new(value);
    public static implicit operator PGuid(SqlGuid value) => new(value);
    public static implicit operator Guid(PGuid value) => (Guid)value._sql;

    public static bool operator ==(PGuid x, PGuid y)
    {
        var isEqual = x.ValueType == y.ValueType;
        if (isEqual)
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;
        return isEqual;
    }

    public static bool operator !=(PGuid x, PGuid y) => !(x == y);

    public static PGuid Parse(string s, PValueType type)
    {
        var sp = string.IsNullOrEmpty(s) ? new PGuid(type) : SqlGuid.Parse(s);
        return sp;
    }

    public override bool Equals(object y)
    {
        var equals = y is PGuid;
        if (equals)
            equals = this == (PGuid)y;
        return equals;
    }

    public override int GetHashCode()
    {
        var hashCode = _sql.GetHashCode();
        return hashCode;
    }

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

        //set
        //{
        //    if (value == null)
        //    {
        //        ValueType = PValueType.Default;
        //        _sql = SqlGuid.Null;
        //    }
        //    else if (value == DBNull.Value)
        //    {
        //        ValueType = PValueType.Null;
        //        _sql = SqlGuid.Null;
        //    }
        //    else
        //    {
        //        _sql = (SqlGuid) value;
        //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
        //    }
        //}
    }

    public override string ToString() => _sql.ToString();
}