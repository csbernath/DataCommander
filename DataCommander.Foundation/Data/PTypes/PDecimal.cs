namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PDecimal : INullable
    {
        private SqlDecimal _sql;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PDecimal( decimal value )
        {
            this._sql = value;
            this.ValueType = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PDecimal( SqlDecimal value )
        {
            this._sql = value;
            this.ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PDecimal( PValueType type )
        {
            this.ValueType = type;
            this._sql = SqlDecimal.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PDecimal( decimal value )
        {
            return new PDecimal( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PDecimal( SqlDecimal value )
        {
            return new PDecimal( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator decimal( PDecimal value )
        {
            return (decimal) value._sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PDecimal x, PDecimal y )
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
        public static bool operator !=( PDecimal x, PDecimal y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PDecimal Parse( string s, PValueType type )
        {
            PDecimal sp;

            if (string.IsNullOrEmpty( s ))
            {
                sp = new PDecimal( type );
            }
            else
            {
                sp = SqlDecimal.Parse( s );
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
            var equals = y is PDecimal;

            if (equals)
            {
                equals = this == (PDecimal) y;
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
                    case PValueType.Null:
                        value = DBNull.Value;
                        break;

                    case PValueType.Value:
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
                    this._sql = SqlDecimal.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.ValueType = PValueType.Null;
                    this._sql = SqlDecimal.Null;
                }
                else
                {
                    this._sql = (SqlDecimal) value;
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDecimal Null = new PDecimal( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDecimal Default = new PDecimal( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDecimal Empty = new PDecimal( PValueType.Empty );
    }
}