using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Data.SqlClient
{
    public static class SqlConstantMapper
    {
        private const string True = "1";
        private const string False = "0";

        [Pure]
        public static string ToSqlConstant(this bool source) => source ? True : False;

        [Pure]
        public static string ToSqlConstant(this bool? source) => source != null
            ? source.Value.ToSqlConstant()
            : SqlNull.NullString;

        [Pure]
        public static string ToSqlConstant(this SmallDate source)
        {
            var dateTime = source.ToDateTime();
            const string format = "yyyyMMdd";
            return $"'{dateTime.ToString(format, CultureInfo.InvariantCulture)}'";
        }

        [Pure]
        public static string ToSqlConstant(this DateTime source)
        {
            var timeOfDay = source.TimeOfDay;
            string format;

            if (timeOfDay == TimeSpan.Zero)
                format = "yyyyMMdd";
            else
                format = source.Millisecond == 0
                    ? "yyyyMMdd HH:mm:ss"
                    : "yyyyMMdd HH:mm:ss.fff";

            return $"'{source.ToString(format, CultureInfo.InvariantCulture)}'";
        }

        [Pure]
        public static string ToSqlConstant(this DateTime? source) => source != null
            ? source.Value.ToSqlConstant()
            : SqlNull.NullString;

        [Pure]
        public static string ToSqlConstant(this decimal source) => source.ToString(NumberFormatInfo.InvariantInfo);

        [Pure]
        public static string ToSqlConstant(this int source) => source.ToString();

        [Pure]
        public static string ToSqlConstant(this int? source) => source != null
            ? source.Value.ToSqlConstant()
            : SqlNull.NullString;

        [Pure]
        public static string ToSqlConstant(this double? source) => source != null
            ? source.Value.ToString(CultureInfo.InvariantCulture)
            : SqlNull.NullString;

        [Pure]
        public static string ToSqlConstant(this Guid source) => $"'{source}'";

        [Pure]
        public static string ToNullableNVarChar(this string source) => source != null
            ? source.ToNVarChar()
            : SqlNull.NullString;

        [Pure]
        public static string ToNullableVarChar(this string source) => source != null
            ? source.ToVarChar()
            : SqlNull.NullString;

        [Pure]
        public static string ToNVarChar(this string source)
        {
            Assert.IsNotNull(source);
            return "N" + source.ToVarChar();
        }

        [Pure]
        public static string ToVarChar(this string source)
        {
            Assert.IsNotNull(source);
            return "'" + source.Replace("'", "''") + "'";
        }
    }
}