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

    public static IList<char> AsList(this string source) => new StringAsList(source);
    public static string Format(this string format, params object[] args) => string.Format(format, args);
    public static string Format(this string format, IFormatProvider provider, params object[] args) => string.Format(provider, format, args);
    public static string Indent(this string source, int indentCount) => source.Indent(IndentString, indentCount);

    public static IEnumerable<string> GetLines(this string source)
    {
        using (var stringReader = new StringReader(source))
        {
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                    break;

                yield return line;
            }
        }
    }

    public static string IncreaseLineIndent(this string line, int indentSize)
    {
        ArgumentNullException.ThrowIfNull(line);
        Assert.IsInRange(indentSize > 0);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(new string(' ', indentSize));
        stringBuilder.Append(line);
        return stringBuilder.ToString();
    }

    public static string DecreaseLineIndent(this string line, int indentSize)
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

    public static int IndexOf(this string source, Func<char, bool> predicate)
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

    [Pure]
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

    [Pure]
    public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

    public static DateTime? ParseToNullableDateTime(this string source) => string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source);

    public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider) =>
        string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source, provider);

    public static DateTime? ParseToNullableDateTime(this string source, IFormatProvider provider, DateTimeStyles styles) =>
        string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.Parse(source, provider, styles);

    public static DateTime? ParseExactToNullableDateTime(this string source, string format, IFormatProvider provider) =>
        string.IsNullOrEmpty(source) ? (DateTime?)null : DateTime.ParseExact(source, format, provider);

    public static decimal? ParseToNullableDecimal(this string source) => string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source);

    public static decimal? ParseToNullableDecimal(this string source, IFormatProvider provider) =>
        string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source, provider);

    public static decimal? ParseToNullableDecimal(this string source, NumberStyles style, IFormatProvider provider) =>
        string.IsNullOrEmpty(source) ? (decimal?)null : decimal.Parse(source, style, provider);

    public static int? ParseToNullableInt32(this string source) => string.IsNullOrEmpty(source) ? (int?)null : int.Parse(source);

    public static string Right(this string value, int length)
    {
        ArgumentNullException.ThrowIfNull(value);
        Assert.IsInRange(value.Length >= length);

        var startIndex = value.Length - length;
        return value[startIndex..];
    }

    private static string Indent(this string source, string indentString, int indentCount)
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