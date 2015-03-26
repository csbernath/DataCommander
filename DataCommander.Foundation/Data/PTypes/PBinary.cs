namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PBinary : INullable
    {
        private SqlBinary sql;
        private PValueType type;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PBinary( Byte[] value )
        {
            this.sql = new SqlBinary( value );
            this.type = PValueType.Value;
        }

        private PBinary( PValueType type )
        {
            this.type = type;
            this.sql = SqlBinary.Null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PBinary( Byte[] value )
        {
            return new PBinary( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Byte[]( PBinary value )
        {
            return (Byte[]) value.sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==( PBinary x, PBinary y )
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
            bool equals = y is PBinary;

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
                    this.sql = SqlBinary.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                    this.sql = SqlBinary.Null;
                }
                else
                {
                    this.type = PValueType.Value;
                    this.sql = (Byte[]) value;
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
            return this.sql.ToString();
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