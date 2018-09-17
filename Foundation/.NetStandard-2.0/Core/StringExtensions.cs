using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Core
{
    public static class StringExtensions
    {
        public const string IndentString = "    ";

        public static IList<char> AsList(this string source) => new StringAsList(source);
        public static string Format(this string format, params object[] args) => string.Format(format, args);
        public static string Format(this string format, IFormatProvider provider, params object[] args) => string.Format(provider, format, args);
        public static string Indent(this string source, int indentCount) => source.Indent(IndentString, indentCount);

        [Pure]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

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

        public static DateTime? ParseToNullableDateTime(this string source) => string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source);

        public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source, provider);

        public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider, DateTimeStyles styles) =>
            string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.Parse(source, provider, styles);

        public static DateTime? ParseExactToNullableDateTime(this string source, string format, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (DateTime?) null : DateTime.ParseExact(source, format, provider);

        public static decimal? ParseToNullableDecimal(this string source) => string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source);

        public static decimal? ParseToNullableDecimal(this string source, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source, provider);

        public static decimal? ParseToNullableDecimal(this string source, NumberStyles style, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (decimal?) null : decimal.Parse(source, style, provider);

        public static int? ParseToNullableInt32(this string source) => string.IsNullOrEmpty(source) ? (int?) null : int.Parse(source);

        public static string Right(this string value, int length)
        {
            Assert.IsNotNull(value);
            Assert.IsInRange(value.Length >= length);

            var startIndex = value.Length - length;
            return value.Substring(startIndex);
        }

        private static string Indent(this string source, string indentString, int indentCount)
        {
            indentString = string.Join(string.Empty, Enumerable.Repeat(indentString, indentCount));
            var stringBuyBuilder = new StringBuilder();

            using (var stringReader = new StringReader(source))
            {
                var sequence = new Sequence();
                while (true)
                {
                    var line = stringReader.ReadLine();
                    if (line == null)
                        break;

                    if (sequence.Next() > 0)
                        stringBuyBuilder.AppendLine();

                    if (line.Length > 0)
                    {
                        stringBuyBuilder.Append(indentString);
                        stringBuyBuilder.Append(line);
                    }
                }
            }

            return stringBuyBuilder.ToString();
        }

        private sealed class StringAsList : IList<char>
        {
            private readonly string _source;

            public StringAsList(string source)
            {
                _source = source;
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
                get => _source[index];
                set => throw new NotImplementedException();
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

            int ICollection<char>.Count => _source.Length;

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