namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

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
            this._sql = value;
            this.ValueType = this._sql.IsNull ? PValueType.Null : PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PInt64(SqlInt64 value)
        {
            this._sql = value;
            this.ValueType = PValueType.Value;
        }

        private PInt64(PValueType type)
        {
            this.ValueType = type;
            this._sql = SqlInt64.Null;
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
                    this._sql = SqlInt64.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.ValueType = PValueType.Null;
                    this._sql = SqlInt64.Null;
                }
                else
                {
                    this._sql = (SqlInt64) value;
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