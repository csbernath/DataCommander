using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PBinary : INullable
    {
        private SqlBinary _sql;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PBinary( byte[] value )
        {
            _sql = new SqlBinary( value );
            ValueType = PValueType.Value;
        }

        private PBinary( PValueType type )
        {
            ValueType = type;
            _sql = SqlBinary.Null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PBinary( byte[] value )
        {
            return new PBinary( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte[]( PBinary value )
        {
            return (byte[]) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PBinary x, PBinary y )
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
        public static bool operator !=( PBinary x, PBinary y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals( object y )
        {
            var equals = y is PBinary;

            if (equals)
            {
                equals = this == (PBinary) y;
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
                    _sql = SqlBinary.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlBinary.Null;
                }
                else
                {
                    ValueType = PValueType.Value;
                    _sql = (byte[]) value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <a href="frlrfsystemdatasqltypessqlbooleanclasstopic.htm">SqlBoolean</a> structure
        /// using the supplied boolean value.
        /// </summary>
        public override string ToString()
        {
            return _sql.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBinary Null = new PBinary( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBinary Default = new PBinary( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBinary Empty = new PBinary( PValueType.Empty );
    }
}