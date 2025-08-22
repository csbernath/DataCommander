using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Core;

public static class StringExtensions
{
    public const string IndentString = "    ";

    extension(string source)
    {
        public IList<char> AsList() => new StringAsList(source);

        public string Indent(int indentCount) => source.Indent(IndentString, indentCount);

        public IEnumerable<string> GetLines()
        {
            using var stringReader = new StringReader(source);
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                    break;

                yield return line;
            }
        }

        public int IndexOf(Func<char, bool> predicate)
        {
            var result = -1;
            for (var index = 0; index < source.Length; ++index)
            {
                if (predicate(source[index]))
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public DateTime? ParseToNullableDateTime() => string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source);

        public DateTime? ParseToNullableDateTime(IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source, provider);

        public DateTime? ParseToNullableDateTime(IFormatProvider provider, DateTimeStyles styles) =>
            string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source, provider, styles);

        public DateTime? ParseExactToNullableDateTime(string format, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.ParseExact(source, format, provider);

        public decimal? ParseToNullableDecimal() => string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source);

        public decimal? ParseToNullableDecimal(IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source, provider);

        public decimal? ParseToNullableDecimal(NumberStyles style, IFormatProvider provider) =>
            string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source, style, provider);

        public int? ParseToNullableInt32() => string.IsNullOrEmpty(source) ? (int?)null : int.Parse(source);

        private string Indent(string indentString, int indentCount)
        {
            indentString = string.Join(string.Empty, Enumerable.Repeat(indentString, indentCount));
            var stringBuilder = new StringBuilder();

            using (var stringReader = new StringReader(source))
            {
                var sequence = new Sequence();
                while (true)
                {
                    var line = stringReader.ReadLine();
                    if (line == null)
                        break;

                    if (sequence.Next() > 0)
                        stringBuilder.AppendLine();

                    if (line.Length > 0)
                    {
                        stringBuilder.Append(indentString);
                        stringBuilder.Append(line);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }

    extension(string format)
    {
        public string Format(params object[] args) => string.Format(format, args);
        public string Format(IFormatProvider provider, params object[] args) => string.Format(provider, format, args);
    }

    extension(string line)
    {
        public string IncreaseLineIndent(int indentSize)
        {
            ArgumentNullException.ThrowIfNull(line);
            Assert.IsInRange(indentSize > 0);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(new string(' ', indentSize));
            stringBuilder.Append(line);
            return stringBuilder.ToString();
        }

        public string DecreaseLineIndent(int indentSize)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(line));
            Assert.IsInRange(indentSize > 0);
            var index = line.IndexOf(c => !char.IsWhiteSpace(c));
            string decreasedLine;
            if (index > 0)
            {
                index = Math.Min(index, indentSize);
                decreasedLine = line[index..];
            }
            else
                decreasedLine = line;

            return decreasedLine;
        }
    }

    extension(string value)
    {
        [Pure]
        public bool IsNullOrEmpty() => string.IsNullOrEmpty(value);

        [Pure]
        public bool IsNullOrWhiteSpace() => string.IsNullOrWhiteSpace(value);

        public string Right(int length)
        {
            ArgumentNullException.ThrowIfNull(value);
            Assert.IsInRange(value.Length >= length);

            var startIndex = value.Length - length;
            return value[startIndex..];
        }
    }

    private sealed class StringAsList(string source) : IList<char>
    {
        int IList<char>.IndexOf(char item) => throw new NotImplementedException();

        void IList<char>.Insert(int index, char item) => throw new NotImplementedException();

        void IList<char>.RemoveAt(int index) => throw new NotImplementedException();

        char IList<char>.this[int index]
        {
            get => source[index];
            set => throw new NotImplementedException();
        }

        void ICollection<char>.Add(char item) => throw new NotImplementedException();

        void ICollection<char>.Clear() => throw new NotImplementedException();

        bool ICollection<char>.Contains(char item) => throw new NotImplementedException();

        void ICollection<char>.CopyTo(char[] array, int arrayIndex) => throw new NotImplementedException();

        int ICollection<char>.Count => source.Length;

        bool ICollection<char>.IsReadOnly => true;

        bool ICollection<char>.Remove(char item) => throw new NotImplementedException();

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}