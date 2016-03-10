namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PGuid : INullable
    {
        private SqlGuid sql;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PGuid Null = new PGuid( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PGuid Default = new PGuid( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PGuid Empty = new PGuid( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PGuid( Guid value )
        {
            this.sql = value;
            this.type = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PGuid( SqlGuid value )
        {
            this.sql = value;
            this.type = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PGuid( PValueType type )
        {
            this.type = type;
            this.sql = SqlGuid.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PGuid( Guid value )
        {
            return new PGuid( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PGuid( SqlGuid value )
        {
            return new PGuid( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Guid( PGuid value )
        {
            return (Guid) value.sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PGuid x, PGuid y )
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
        public static bool operator !=( PGuid x, PGuid y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PGuid Parse( string s, PValueType type )
        {
            PGuid sp;

            if (s == null || s.Length == 0)
            {
                sp = new PGuid( type );
            }
            else
            {
                sp = SqlGuid.Parse( s );
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
            bool equals = y is PGuid;

            if (equals)
            {
                equals = this == (PGuid) y;
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
                    this.sql = SqlGuid.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                    this.sql = SqlGuid.Null;
                }
                else
                {
                    this.sql = (SqlGuid) value;
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