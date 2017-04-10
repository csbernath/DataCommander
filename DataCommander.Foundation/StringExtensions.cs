namespace DataCommander.Foundation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<char> AsList(this string source)
        {
            return new StringAsList(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static bool IsNullOrWhiteSpace(this string value)
        {
#if FOUNDATION_3_5
            bool isNullOrWhiteSpace = true;
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
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
            return string.IsNullOrWhiteSpace(value);
#endif
        }

#if FOUNDATION_3_5
    /// <summary>
    /// 
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="values"></param>
    /// <returns></returns>
        public static string Join( string separator, IEnumerable<string> values )
        {
            var sb = new StringBuilder();
            bool first = true;

            foreach (string value in values)
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
        public static DateTime? ParseToNullableDateTime(this string source)
        {
            return string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider)
        {
            return string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source, provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider,
            DateTimeStyles styles)
        {
            return string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source, provider, styles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DateTime? ParseExactToNullableDateTime(this string source, string format, IFormatProvider provider)
        {
            return string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.ParseExact(source, format, provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? ParseToNullableDecimal(this string source)
        {
            return string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static decimal? ParseToNullableDecimal(this string source, IFormatProvider provider)
        {
            return string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source, provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static decimal? ParseToNullableDecimal(this string source, NumberStyles style, IFormatProvider provider)
        {
            return string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source, style, provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int? ParseToNullableInt32(this string source)
        {
            return string.IsNullOrEmpty(source) ? (int?) null : int.Parse(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string value, int length)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Requires<ArgumentOutOfRangeException>(value.Length >= length);
#endif

            var startIndex = value.Length - length;
            return value.Substring(startIndex);
        }

        private sealed class StringAsList : IList<char>
        {
            private readonly string source;

            public StringAsList(string source)
            {
                this.source = source;
            }

#region IList<Char> Members

            int IList<char>.IndexOf(char item)
            {
                throw new NotImplementedException();
            }

            void IList<char>.Insert(int index, char item)
            {
                throw new NotImplementedException();
            }

            void IList<char>.RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            char IList<char>.this[int index]
            {
                get
                {
                    return this.source[index];
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

#endregion

#region ICollection<Char> Members

            void ICollection<char>.Add(char item)
            {
                throw new NotImplementedException();
            }

            void ICollection<char>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<char>.Contains(char item)
            {
                throw new NotImplementedException();
            }

            void ICollection<char>.CopyTo(char[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            int ICollection<char>.Count => this.source.Length;

            bool ICollection<char>.IsReadOnly => true;

            bool ICollection<char>.Remove(char item)
            {
                throw new NotImplementedException();
            }

#endregion

#region IEnumerable<Char> Members

            IEnumerator<char> IEnumerable<char>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

#endregion

#region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

#endregion
        }
    }
}