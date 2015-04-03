namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PByte : INullable
    {
        private SqlByte sql;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PByte( byte value )
        {
            this.sql = value;
            this.type = PValueType.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PByte( SqlByte value )
        {
            this.sql = value;
            this.type = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PByte( PValueType type )
        {
            this.type = type;
            this.sql = SqlByte.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PByte( byte value )
        {
            return new PByte( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PByte( byte? value )
        {
            return value != null ? new PByte( value.Value ) : Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PByte( SqlByte value )
        {
            return new PByte( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte( PByte value )
        {
            return (byte) value.sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PByte x, PByte y )
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
        public static bool operator !=( PByte x, PByte y )
        {
            return !(x == y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PByte Parse( string s, PValueType type )
        {
            PByte sp;

            if (s == null || s.Length == 0)
            {
                sp = new PByte( type );
            }
            else
            {
                sp = SqlByte.Parse( s );
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
            bool equals = y is PByte;

            if (equals)
            {
                equals = this == (PByte) y;
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
        public PValueType ValueType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull
        {
            get
            {
                return this.type == PValueType.Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValue
        {
            get
            {
                return this.type == PValueType.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.type == PValueType.Empty;
            }
        }

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
                    this.sql = SqlByte.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                    this.sql = SqlByte.Null;
                }
                else
                {
                    this.sql = (SqlByte) value;
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly PByte Null = new PByte( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PByte Default = new PByte( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PByte Empty = new PByte( PValueType.Empty );
    }
}