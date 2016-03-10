namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using DataCommander.Foundation.Data.SqlClient;

    /// <summary>
    /// 
    /// </summary>
    public struct PDateTime : INullable
    {
        private SqlDateTime sql;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDateTime Null = new PDateTime( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDateTime Default = new PDateTime( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PDateTime Empty = new PDateTime( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PDateTime( DateTime value )
        {
            this.sql = value;
            this.type = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PDateTime( DateTime? value )
        {
            this.sql = value.ToSqlDateTime();
            this.type = value == null ? PValueType.Null : PValueType.Value;
        }

        private PDateTime( PValueType type )
        {
            this.type = type;
            this.sql = SqlDateTime.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public PDateTime( SqlDateTime value )
        {
            this.sql = value;
            this.type = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PDateTime( DateTime value )
        {
            return new PDateTime( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PDateTime( DateTime? value )
        {
            return new PDateTime( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PDateTime( SqlDateTime value )
        {
            return new PDateTime( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator DateTime( PDateTime value )
        {
            return (DateTime) value.sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PDateTime x, PDateTime y )
        {
            bool isEqual = x.type == y.type;

            if (isEqual)
            {
                if (x.type == PValueType.Value)
                {
                    isEqual = x.sql.Value == y.sql.Value;
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
        public static bool operator !=( PDateTime x, PDateTime y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PDateTime Parse( string s, PValueType type )
        {
            PDateTime sp;

            if (s == null || s.Length == 0)
            {
                sp = new PDateTime( type );
            }
            else
            {
                sp = SqlDateTime.Parse( s );
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
            bool equals = y is PDateTime;

            if (equals)
            {
                equals = this == (PDateTime) y;
            }

            return equals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = this.sql.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public PValueType ValueType => this.type;

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull => this.type == PValueType.Null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValue => this.type == PValueType.Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty => this.type == PValueType.Empty;

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                object value;

                switch (this.type)
                {
                    case PValueType.Value:
                    case PValueType.Null:
                        value = this.sql;
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
                    this.type = PValueType.Default;
                    this.sql = SqlDateTime.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                    this.sql = SqlDateTime.Null;
                }
                else
                {
                    this.sql = (SqlDateTime) value;
                    this.type = this.sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.sql.ToString();
        }
    }
}