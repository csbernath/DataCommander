using System.Data.SqlTypes;

namespace Foundation.Data.PTypes;

public struct PBoolean : INullable
{
    private readonly SqlBoolean _sql;

    public static readonly PBoolean Null = new(PValueType.Null);
    public static readonly PBoolean Default = new(PValueType.Default);
    public static readonly PBoolean Empty = new(PValueType.Empty);
    public static readonly PBoolean True = new(true);
    public static readonly PBoolean False = new(false);

    #region Constructors

    public PBoolean(bool value)
    {
        _sql = value;
        ValueType = PValueType.Value;
    }

    public PBoolean(SqlBoolean value)
    {
        _sql = value;
        ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
    }

    private PBoolean(PValueType type)
    {
        ValueType = type;
        _sql = SqlBoolean.Null;
    }

    #endregion

    public static implicit operator PBoolean(bool value) => new(value);

    public static implicit operator PBoolean(bool? value)
    {
        PBoolean target;
        if (value == null)
            target = Null;
        else if (value.Value)
            target = True;
        else
            target = False;

        return target;
    }

    public static implicit operator PBoolean(SqlBoolean value) => new(value);
    public static implicit operator bool(PBoolean value) => (bool)value._sql;

    public static bool operator ==(PBoolean x, PBoolean y)
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

    public static bool operator !=(PBoolean x, PBoolean y) => !(x == y);

    public static PBoolean Parse(string s, PValueType type) => string.IsNullOrEmpty(s) ? new PBoolean(type) : SqlBoolean.Parse(s);

    public override bool Equals(object y)
    {
        var equals = y is PBoolean;
        if (equals)
            equals = this == (PBoolean)y;
        return equals;
    }

    public override int GetHashCode() => _sql.GetHashCode();

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

        //set
        //{
        //    if (value == null)
        //    {
        //        ValueType = PValueType.Default;
        //        _sql = SqlBoolean.Null;
        //    }
        //    else if (value == DBNull.Value)
        //    {
        //        ValueType = PValueType.Null;
        //        _sql = SqlBoolean.Null;
        //    }
        //    else
        //    {
        //        _sql = (SqlBoolean) value;
        //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
        //    }
        //}
    }

    public bool IsTrue => _sql.IsTrue;
    public bool IsFalse => _sql.IsFalse;
    public override string ToString() => _sql.ToString();
}