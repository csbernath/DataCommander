﻿using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.SqlClient.PTypes;

public struct PDateTime : INullable
{
    private readonly SqlDateTime _sql;

    public static readonly PDateTime Null = new(PValueType.Null);
    public static readonly PDateTime Default = new(PValueType.Default);
    public static readonly PDateTime Empty = new(PValueType.Empty);

    public PDateTime(DateTime value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PDateTime(DateTime? value)
    {
        _sql = ToSqlDateTime(value);
        ValueType = value == null ? PValueType.Null : PValueType.Value;
    }

    private PDateTime(PValueType type)
    {
        ValueType = type;
        _sql = SqlDateTime.Null;
    }

    [DebuggerStepThrough]
    public PDateTime(SqlDateTime value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    public static implicit operator PDateTime(DateTime value) => new(value);
    public static implicit operator PDateTime(DateTime? value) => new(value);

    [DebuggerStepThrough]
    public static implicit operator PDateTime(SqlDateTime value) => new(value);

    public static implicit operator DateTime(PDateTime value) => (DateTime)value._sql;

    public static bool operator ==(PDateTime x, PDateTime y)
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

    public static bool operator !=(PDateTime x, PDateTime y) => !(x == y);

    public static PDateTime Parse(string s, PValueType type)
    {
        PDateTime sp;

        if (string.IsNullOrEmpty(s))
            sp = new PDateTime(type);
        else
            sp = SqlDateTime.Parse(s);

        return sp;
    }

    public readonly override bool Equals(object? y)
    {
        var equals = y is PDateTime;
        if (equals)
            equals = this == (PDateTime)y!;

        return equals;
    }

    public override readonly int GetHashCode() => _sql.GetHashCode();

    public PValueType ValueType { get; private set; }

    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public readonly object? Value
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

        //set
        //{
        //    if (value == null)
        //    {
        //        ValueType = PValueType.Default;
        //        _sql = SqlDateTime.Null;
        //    }
        //    else if (value == DBNull.Value)
        //    {
        //        ValueType = PValueType.Null;
        //        _sql = SqlDateTime.Null;
        //    }
        //    else
        //    {
        //        _sql = (SqlDateTime) value;
        //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
        //    }
        //}
    }

    public override readonly string ToString() => _sql.ToString();

    private static SqlDateTime ToSqlDateTime(DateTime? value) => value ?? SqlDateTime.Null;
}