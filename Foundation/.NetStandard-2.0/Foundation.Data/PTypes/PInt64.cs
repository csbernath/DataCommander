using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PInt64 : INullable
    {
        private SqlInt64 _sql;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt64 Null = new PInt64(PValueType.Null);

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt64 Default = new PInt64(PValueType.Default);

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt64 Empty = new PInt64(PValueType.Empty);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PInt64(long value)
        {
            return new PInt64(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PInt64(long? value)
        {
            return value != null ? new PInt64(value.Value) : Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PInt64(SqlInt64 value)
        {
            return new PInt64(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator long(PInt64 value)
        {
            return (long) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(PInt64 x, PInt64 y)
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
        public static bool operator !=(PInt64 x, PInt64 y)
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PInt64 Parse(string s, PValueType type)
        {
            PInt64 sp;

            if (string.IsNullOrEmpty(s))
            {
                sp = new PInt64(type);
            }
            else
            {
                sp = SqlInt64.Parse(s);
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(object y)
        {
            var equals = y is PInt64;

            if (equals)
            {
                equals = this == (PInt64) y;
            }

            return equals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = _sql.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public PValueType ValueType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull => ValueType == PValueType.Null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValue => ValueType == PValueType.Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty => ValueType == PValueType.Empty;

        /// <summary>
        /// 
        /// </summary>
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
                    _sql = SqlInt64.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlInt64.Null;
                }
                else
                {
                    _sql = (SqlInt64) value;
                    ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _sql.ToString();
        }
    }
}