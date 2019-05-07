using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PVariant : INullable
    {
        private object _sql;

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
            ValueType = type;
            _sql = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PVariant( object value )
        {
            if (value == null)
            {
                ValueType = PValueType.Default;
            }
            else if (value == DBNull.Value)
            {
                ValueType = PValueType.Null;
            }
            else
            {
                ValueType = PValueType.Value;
            }

            _sql = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public PValueType ValueType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull => ValueType == PValueType.Null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValue => ValueType == PValueType.Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty => ValueType == PValueType.Empty;

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                object value;

                switch (ValueType)
                {
                    case PValueType.Value:
                    case PValueType.Null:
                        value = _sql;
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
                    ValueType = PValueType.Default;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                }
                else
                {
                    ValueType = PValueType.Value;
                }

                _sql = value;
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
            return _sql.ToString();
        }
    }
}