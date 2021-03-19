using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    public struct PDouble : INullable
    {
        private SqlDouble _sql;

        public static readonly PDouble Null = new PDouble(PValueType.Null);
        public static readonly PDouble Default = new PDouble(PValueType.Default);
        public static readonly PDouble Empty = new PDouble(PValueType.Empty);

        public PDouble(double value)
        {
            _sql = value;
            ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
        }

        public PDouble(SqlDouble value)
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PDouble(PValueType type)
        {
            ValueType = type;
            _sql = SqlDouble.Null;
        }

        public static implicit operator PDouble(double value) => new PDouble(value);
        public static implicit operator PDouble(SqlDouble value) => new PDouble(value);
        public static implicit operator double(PDouble value) => (double)value._sql;

        public static bool operator ==(PDouble x, PDouble y)
        {
            var isEqual = x.ValueType == y.ValueType;
            if (isEqual)
                if (x.ValueType == PValueType.Value)
                    isEqual = x._sql.Value == y._sql.Value;
            return isEqual;
        }

        public static bool operator !=(PDouble x, PDouble y) => !(x == y);

        public static PDouble Parse(string s, PValueType type)
        {
            var sp = string.IsNullOrEmpty(s) ? new PDouble(type) : SqlDouble.Parse(s);
            return sp;
        }

        public override bool Equals(object y)
        {
            var equals = y is PDouble;
            if (equals)
                equals = this == (PDouble)y;
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
            //        _sql = SqlDouble.Null;
            //    }
            //    else if (value == DBNull.Value)
            //    {
            //        ValueType = PValueType.Null;
            //        _sql = SqlDouble.Null;
            //    }
            //    else
            //    {
            //        _sql = (SqlDouble) value;
            //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            //    }
            //}
        }

        public override string ToString() => _sql.ToString();
    }
}