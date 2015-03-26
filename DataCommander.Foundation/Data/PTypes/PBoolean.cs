namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PBoolean : INullable
    {
        private SqlBoolean sql;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBoolean Null = new PBoolean( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBoolean Default = new PBoolean( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBoolean Empty = new PBoolean( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBoolean True = new PBoolean( true );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PBoolean False = new PBoolean( false );

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="PBoolean"/>
        /// structure using the supplied boolean value.
        /// </summary>
        /// <param name="value"></param>
        public PBoolean( bool value )
        {
            this.sql = value;
            this.type = PValueType.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PBoolean"/> structure using the supplied
        /// <see cref="System.Data.SqlTypes.SqlBoolean"/> value.
        /// </summary>
        public PBoolean( SqlBoolean value )
        {
            this.sql = value;
            this.type = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PBoolean( PValueType type )
        {
            this.type = type;
            this.sql = SqlBoolean.Null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PBoolean( bool value )
        {
            return new PBoolean( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PBoolean( bool? value )
        {
            PBoolean target;
            if (value == null)
            {
                target = Null;
            }
            else if (value.Value)
            {
                target = True;
            }
            else
            {
                target = False;
            }

            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PBoolean( SqlBoolean value )
        {
            return new PBoolean( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator bool( PBoolean value )
        {
            return (bool) value.sql;
        }

        /// <summary>
        /// Compares two instances of <see cref="PBoolean"/> for equality.
        /// </summary>
        /// <param name="x">An <see cref="PBoolean"/></param>
        /// <param name="y">An <see cref="PBoolean"/></param>
        /// <returns>
        /// A <see cref="bool"/> that is <c>true</c> if the two instances's <see cref="ValueType"/> are equal
        /// and values are equal.
        /// </returns>
        public static bool operator ==( PBoolean x, PBoolean y )
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
        public static bool operator !=( PBoolean x, PBoolean y )
        {
            return !(x == y);
        }

        /// <summary>
        /// Converts the specified <see cref="System.String"/> representation of a logical value
        /// to its <see cref="PBoolean"/> equivalent.
        /// </summary>
        /// <param name="s">
        /// The <see cref="System.String"/> to be converted. 
        /// </param>
        /// <param name="type"></param>
        /// <returns>
        /// An <see cref="PBoolean"/> structure containing the parsed value.
        /// </returns>
        public static PBoolean Parse( string s, PValueType type )
        {
            PBoolean sp;

            if (string.IsNullOrEmpty(s))
            {
                sp = new PBoolean( type );
            }
            else
            {
                sp = SqlBoolean.Parse( s );
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
            bool equals = y is PBoolean;

            if (equals)
            {
                equals = this == (PBoolean) y;
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
                    this.sql = SqlBoolean.Null;
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                    this.sql = SqlBoolean.Null;
                }
                else
                {
                    this.sql = (SqlBoolean) value;
                    this.type = this.sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTrue
        {
            get
            {
                return this.sql.IsTrue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFalse
        {
            get
            {
                return this.sql.IsFalse;
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