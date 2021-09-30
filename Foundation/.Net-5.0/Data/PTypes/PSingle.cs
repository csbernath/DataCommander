using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    public struct PSingle : INullable
    {
        private SqlSingle _sql;

        public static readonly PSingle Null = new(PValueType.Null);
        public static readonly PSingle Default = new(PValueType.Default);
        public static readonly PSingle Empty = new(PValueType.Empty);

        private PSingle(PValueType type)
        {
            ValueType = type;
            _sql = SqlSingle.Null;
        }

        public PSingle(long value)
        {
            _sql = value;
            ValueType = PValueType.Value;
        }

        public PSingle(SqlSingle value)
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        public static implicit operator PSingle(float value) => new(value);
        public static implicit operator PSingle(SqlSingle value) => new(value);
        public static implicit operator float(PSingle value) => (float)value._sql;

        public static bool operator ==(PSingle x, PSingle y)
        {
            var isEqual = x.ValueType == y.ValueType;

            if (isEqual)
                if (x.ValueType == PValueType.Value)
                    isEqual = x._sql.Value == y._sql.Value;

            return isEqual;
        }

        public static bool operator !=(PSingle x, PSingle y) => !(x == y);

        public static PSingle Parse(string s, PValueType type)
        {
            var sp = string.IsNullOrEmpty(s) ? new PSingle(type) : SqlSingle.Parse(s);
            return sp;
        }

        public override bool Equals(object y)
        {
            var equals = y is PSingle;

            if (equals)
                equals = this == (PSingle)y;

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
                    _sql = SqlSingle.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlSingle.Null;
                }
                else
                {
                    _sql = (SqlSingle)value;
                    ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        public override string ToString() => _sql.ToString();
    }
}