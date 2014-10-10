namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PVariant : INullable
    {
        private Object sql;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PVariant Null = new PVariant( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PVariant Default = new PVariant( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PVariant Empty = new PVariant( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private PVariant( PValueType type )
        {
            this.type = type;
            this.sql = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PVariant( Object value )
        {
            if (value == null)
            {
                this.type = PValueType.Default;
            }
            else if (value == DBNull.Value)
            {
                this.type = PValueType.Null;
            }
            else
            {
                this.type = PValueType.Value;
            }

            this.sql = value;
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
        public Boolean IsNull
        {
            get
            {
                return this.type == PValueType.Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsValue
        {
            get
            {
                return this.type == PValueType.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsEmpty
        {
            get
            {
                return this.type == PValueType.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Object Value
        {
            get
            {
                Object value;

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
                }
                else if (value == DBNull.Value)
                {
                    this.type = PValueType.Null;
                }
                else
                {
                    this.type = PValueType.Value;
                }

                this.sql = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static implicit operator PVariant( String s )
        {
            return new PVariant( s );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.sql.ToString();
        }
    }
}