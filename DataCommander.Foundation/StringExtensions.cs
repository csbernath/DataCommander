namespace DataCommander.Foundation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        private sealed class StringAsList : IList<Char>
        {
            private readonly String source;

            public StringAsList( String source )
            {
                this.source = source;
            }

            #region IList<Char> Members

            Int32 IList<Char>.IndexOf( Char item )
            {
                throw new NotImplementedException();
            }

            void IList<Char>.Insert( Int32 index, Char item )
            {
                throw new NotImplementedException();
            }

            void IList<Char>.RemoveAt( Int32 index )
            {
                throw new NotImplementedException();
            }

            Char IList<Char>.this[ Int32 index ]
            {
                get
                {
                    return this.source[ index ];
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            #endregion

            #region ICollection<Char> Members

            void ICollection<Char>.Add( Char item )
            {
                throw new NotImplementedException();
            }

            void ICollection<Char>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<Char>.Contains( Char item )
            {
                throw new NotImplementedException();
            }

            void ICollection<Char>.CopyTo( Char[] array, Int32 arrayIndex )
            {
                throw new NotImplementedException();
            }

            Int32 ICollection<Char>.Count
            {
                get
                {
                    return this.source.Length;
                }
            }

            bool ICollection<Char>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection<Char>.Remove( Char item )
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<Char> Members

            IEnumerator<Char> IEnumerable<Char>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<Char> AsList( this String source )
        {
            return new StringAsList( source );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String Format( this String format, params Object[] args )
        {
            return String.Format( format, args );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String Format( this String format, IFormatProvider provider, params Object[] args )
        {
            return String.Format( provider, format, args );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean IsNullOrEmpty(this String value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static bool IsNullOrWhiteSpace(this String value)
        {
#if FOUNDATION_3_5
            Boolean isNullOrWhiteSpace = true;
            if (value != null)
            {
                for (Int32 i = 0; i < value.Length; i++)
                {
                    if (!Char.IsWhiteSpace( value[ i ] ))
                    {
                        isNullOrWhiteSpace = false;
                        break;
                    }
                }
            }
            return isNullOrWhiteSpace;
#else
            return String.IsNullOrWhiteSpace(value);
#endif
        }

#if FOUNDATION_3_5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static String Join( String separator, IEnumerable<String> values )
        {
            var sb = new StringBuilder();
            Boolean first = true;

            foreach (String value in values)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append( separator );
                }

                sb.Append( value );
            }

            return sb.ToString();
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime( this String source )
        {
            return String.IsNullOrEmpty( source ) ? (DateTime?)null : DateTime.Parse( source );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime( this String source, IFormatProvider provider )
        {
            return String.IsNullOrEmpty( source ) ? (DateTime?)null : DateTime.Parse( source, provider );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime( this String source, IFormatProvider provider, DateTimeStyles styles )
        {
            return String.IsNullOrEmpty( source ) ? (DateTime?)null : DateTime.Parse( source, provider, styles );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DateTime? ParseExactToNullableDateTime( this String source, String format, IFormatProvider provider )
        {
            return String.IsNullOrEmpty( source ) ? (DateTime?)null : DateTime.ParseExact( source, format, provider );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Decimal? ParseToNullableDecimal( this String source )
        {
            return String.IsNullOrEmpty( source ) ? (Decimal?)null : Decimal.Parse( source );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Decimal? ParseToNullableDecimal( this String source, IFormatProvider provider )
        {
            return String.IsNullOrEmpty( source ) ? (Decimal?)null : Decimal.Parse( source, provider );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Decimal? ParseToNullableDecimal( this String source, NumberStyles style, IFormatProvider provider )
        {
            return String.IsNullOrEmpty( source ) ? (Decimal?)null : Decimal.Parse( source, style, provider );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Int32? ParseToNullableInt32( this String source )
        {
            return String.IsNullOrEmpty( source ) ? (Int32?)null : Int32.Parse( source );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static String Right( this String value, Int32 length )
        {
            Contract.Requires<ArgumentNullException>( value != null );
            Contract.Requires<ArgumentOutOfRangeException>( value.Length >= length );

            Int32 startIndex = value.Length - length;
            return value.Substring( startIndex );
        }
    }
}