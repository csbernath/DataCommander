using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.PTypes
{
    public struct PString : INullable
    {
        private SqlString _sql;

        public static readonly PString Null = new PString(PValueType.Null);
        public static readonly PString Default = new PString(PValueType.Default);
        public static readonly PString Empty = new PString(PValueType.Empty);

        private PString(PValueType type)
        {
            ValueType = type;
            _sql = SqlString.Null;
        }

        public PString(char value)
        {
            _sql = value.ToString();
            ValueType = PValueType.Value;
        }

        [DebuggerStepThrough]
        public PString(string value)
        {
            _sql = value;
            ValueType = PValueType.Value;
        }

        public PString(SqlString value)
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        public static implicit operator PString(char value) => new PString(value);

        [DebuggerStepThrough]
        public static implicit operator PString(string value) => new PString(value);

        public static implicit operator PString(SqlString value) => new PString(value);

        public static implicit operator string(PString value) => (string)value._sql;

        public static bool operator ==(PString x, PString y)
        {
            var isEqual = x.ValueType == y.ValueType;
            if (isEqual)
                if (x.ValueType == PValueType.Value)
                    isEqual = x._sql.Value == y._sql.Value;

            return isEqual;
        }

        public static bool operator !=(PString x, PString y) => !(x == y);

        public static PString Parse(string s, PValueType type)
        {
            PString sp;

            if (string.IsNullOrEmpty(s))
                sp = new PString(type);
            else
                sp = new PString(s);

            return sp;
        }

        public override bool Equals(object obj)
        {
            var equals = obj is PString;
            if (equals)
                equals = this == (PString)obj;
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
            //        _sql = SqlString.Null;
            //    }
            //    else if (value == DBNull.Value)
            //    {
            //        ValueType = PValueType.Null;
            //        _sql = SqlString.Null;
            //    }
            //    else
            //    {
            //        _sql = (SqlString) value;
            //        ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
            //    }
            //}
        }

        public override string ToString() => _sql.ToString();
    }
}