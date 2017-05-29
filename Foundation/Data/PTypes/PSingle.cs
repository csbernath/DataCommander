using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PSingle : INullable
    {
        private SqlSingle _sql;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PSingle Null = new PSingle( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PSingle Default = new PSingle( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PSingle Empty = new PSingle( PValueType.Empty );

        private PSingle( PValueType type )
        {
            this.ValueType = type;
            this._sql = SqlSingle.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PSingle( long value )
        {
            this._sql = value;
            this.ValueType = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PSingle( SqlSingle value )
        {
            this._sql = value;
            this.ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PSingle( float value )
        {
            return new PSingle( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PSingle( SqlSingle value )
        {
            return new PSingle( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator float( PSingle value )
        {
            return (float) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PSingle x, PSingle y )
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
        public static bool operator !=( PSingle x, PSingle y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PSingle Parse( string s, PValueType type )
        {
            PSingle sp;

            if (string.IsNullOrEmpty(s))
            {
                sp = new PSingle( type );
            }
            else
            {
                sp = SqlSingle.Parse( s );
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
            var equals = y is PSingle;

            if (equals)
            {
                equals = this == (PSingle) y;
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
                    this._sql = SqlSingle.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.ValueType = PValueType.Null;
                    this._sql = SqlSingle.Null;
                }
                else
                {
                    this._sql = (SqlSingle) value;
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