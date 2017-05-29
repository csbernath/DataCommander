using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using Foundation.Data.SqlClient;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PInt32 : INullable
    {
        private SqlInt32 _sql;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt32 Null = new PInt32( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt32 Default = new PInt32( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PInt32 Empty = new PInt32( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public PInt32( int value )
        {
            this._sql = value;
            this.ValueType = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public PInt32( int? value )
        {
            this._sql = value.ToSqlInt32();
            this.ValueType = value != null ? PValueType.Value : PValueType.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public PInt32( SqlInt32 value )
        {
            this._sql = value;
            this.ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PInt32( PValueType type )
        {
            this.ValueType = type;
            this._sql = SqlInt32.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PInt32( int value )
        {
            return new PInt32( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PInt32( int? value )
        {
            return new PInt32( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PInt32( SqlInt32 value )
        {
            return new PInt32( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator int( PInt32 value )
        {
            return (int) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PInt32 x, PInt32 y )
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
        public static bool operator !=( PInt32 x, PInt32 y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PInt32 Parse( string s, PValueType type )
        {
            PInt32 sp;

            if (string.IsNullOrEmpty(s))
            {
                sp = new PInt32( type );
            }
            else
            {
                sp = SqlInt32.Parse( s );
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals( object obj )
        {
            var equals = obj is PInt32;

            if (equals)
            {
                equals = this == (PInt32) obj;
            }

            return equals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = this._sql.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public PValueType ValueType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull => this.ValueType == PValueType.Null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValue => this.ValueType == PValueType.Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty => this.ValueType == PValueType.Empty;

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                object value;

                switch (this.ValueType)
                {
                    case PValueType.Value:
                    case PValueType.Null:
                        value = this._sql;
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
                    this.ValueType = PValueType.Default;
                    this._sql = SqlInt32.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.ValueType = PValueType.Null;
                    this._sql = SqlInt32.Null;
                }
                else
                {
                    this._sql = (SqlInt32) value;
                    this.ValueType = this._sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._sql.ToString();
        }
    }
}