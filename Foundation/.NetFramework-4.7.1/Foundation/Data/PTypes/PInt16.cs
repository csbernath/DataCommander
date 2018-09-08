using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PInt16 : INullable
    {
        private SqlInt16 _sql;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public PInt16( short value )
        {
            _sql = value;
            ValueType = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PInt16( SqlInt16 value )
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PInt16( PValueType type )
        {
            ValueType = type;
            _sql = SqlInt16.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PInt16( short value )
        {
            return new PInt16( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PInt16( SqlInt16 value )
        {
            return new PInt16( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator short( PInt16 value )
        {
            return (short) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PInt16 x, PInt16 y )
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
        public static bool operator !=( PInt16 x, PInt16 y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PInt16 Parse( string s, PValueType type )
        {
            PInt16 sp;

            if (string.IsNullOrEmpty( s ))
            {
                sp = new PInt16( type );
            }
            else
            {
                sp = SqlInt16.Parse( s );
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals( object y )
        {
            var equals = y is PInt16;

            if (equals)
            {
                equals = this == (PInt16) y;
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
                    _sql = SqlInt16.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlInt16.Null;
                }
                else
                {
                    _sql = (SqlInt16) value;
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt16 Null = new PInt16( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt16 Default = new PInt16( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt16 Empty = new PInt16( PValueType.Empty );
    }
}