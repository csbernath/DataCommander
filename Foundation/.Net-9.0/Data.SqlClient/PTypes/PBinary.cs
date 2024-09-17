using System.Data.SqlTypes;

namespace Foundation.Data.SqlClient.PTypes;

public struct PBinary : INullable
{
    private readonly SqlBinary _sql;

    public static readonly PBinary Null = new(PValueType.Null);
    public static readonly PBinary Default = new(PValueType.Default);
    public static readonly PBinary Empty = new(PValueType.Empty);

    public PBinary(byte[] value)
    {
        _sql = new SqlBinary(value);
        ValueType = PValueType.Value;
    }

    private PBinary(PValueType type)
    {
        ValueType = type;
        _sql = SqlBinary.Null;
    }

    public static implicit operator PBinary(byte[] value) => new(value);
    public static implicit operator byte[](PBinary value) => (byte[])value._sql;

    public static bool operator ==(PBinary x, PBinary y)
    {
        bool isEqual = x.ValueType == y.ValueType;
        if (isEqual)
            if (x.ValueType == PValueType.Value)
                isEqual = x._sql.Value == y._sql.Value;
        return isEqual;
    }

    public static bool operator !=(PBinary x, PBinary y) => !(x == y);

    public override readonly bool Equals(object y)
    {
        bool equals = y is PBinary;
        if (equals)
            equals = this == (PBinary)y;
        return equals;
    }

    public override readonly int GetHashCode() => _sql.GetHashCode();

    public PValueType ValueType { get; private set; }

    public readonly bool IsNull => ValueType == PValueType.Null;
    public readonly bool IsValue => ValueType == PValueType.Value;
    public readonly bool IsEmpty => ValueType == PValueType.Empty;

    public readonly object Value
    {
        get
        {
            object value = ValueType switch
            {
                PValueType.Value or PValueType.Null => (object)_sql,
                _ => null,
            };
            return value;
        }

        //set
        //{
        //    if (value == null)
        //    {
        //        ValueType = PValueType.Default;
        //        _sql = SqlBinary.Null;
        //    }
        //    else if (value == DBNull.Value)
        //    {
        //        ValueType = PValueType.Null;
        //        _sql = SqlBinary.Null;
        //    }
        //    else
        //    {
        //        ValueType = PValueType.Value;
        //        _sql = (byte[]) value;
        //    }
        //}
    }

    public override readonly string ToString() => _sql.ToString();
}