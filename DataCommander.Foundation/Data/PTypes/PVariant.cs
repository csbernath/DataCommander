namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public struct PVariant : INullable
    {
        private object sql;
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
        public PVariant( object value )
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
        public static implicit operator PVariant( string s )
        {
            return new PVariant( s );
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