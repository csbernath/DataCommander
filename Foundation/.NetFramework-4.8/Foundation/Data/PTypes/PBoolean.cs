﻿using System;
using System.Data.SqlTypes;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PBoolean : INullable
    {
        private SqlBoolean _sql;

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
            _sql = value;
            ValueType = PValueType.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PBoolean"/> structure using the supplied
        /// <see cref="System.Data.SqlTypes.SqlBoolean"/> value.
        /// </summary>
        public PBoolean( SqlBoolean value )
        {
            _sql = value;
            ValueType = value.IsNull ? PValueType.Null : PValueType.Value;
        }

        private PBoolean( PValueType type )
        {
            ValueType = type;
            _sql = SqlBoolean.Null;
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
            return (bool) value._sql;
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
            var equals = y is PBoolean;

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
            var hashCode = _sql.GetHashCode();
            return hashCode;
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
                    _sql = SqlBoolean.Null;
                }
                else if (value == DBNull.Value)
                {
                    ValueType = PValueType.Null;
                    _sql = SqlBoolean.Null;
                }
                else
                {
                    _sql = (SqlBoolean) value;
                    ValueType = _sql.IsNull ? PValueType.Null : PValueType.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTrue => _sql.IsTrue;

        /// <summary>
        /// 
        /// </summary>
        public bool IsFalse => _sql.IsFalse;

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