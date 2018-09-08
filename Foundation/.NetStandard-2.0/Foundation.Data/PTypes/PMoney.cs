using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PMoney : INullable
    {
        private SqlMoney _sql;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PMoney Null = new PMoney( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PMoney Default = new PMoney( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PMoney Empty = new PMoney( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PMoney( decimal value )
        {
            _sql = value;
            ValueType = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PMoney( SqlMoney value )
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PMoney( PValueType type )
        {
            ValueType = type;
            _sql = SqlMoney.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PMoney( decimal value )
        {
            return new PMoney( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PMoney( SqlMoney value )
        {
            return new PMoney( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator decimal( PMoney value )
        {
            return (decimal) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PMoney x, PMoney y )
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
        public static bool operator !=( PMoney x, PMoney y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PMoney Parse( string s, PValueType type )
        {
            var sp = string.IsNullOrEmpty(s) ? new PMoney( type ) : SqlMoney.Parse( s );
            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals( object y )
        {
            var equals = y is PMoney;

            if (equals)
            {
                equals = this == (PMoney) y;
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
                    _sql = SqlMoney.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlMoney.Null;
                }
                else
                {
                    _sql = (SqlMoney) value;
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