using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Data.SqlClient
{
    public static class SqlStatementExtensions
    {
        private const string True = "1";
        private const string False = "0";

        [Pure]
        public static string ToTSqlBit(this bool source) => source ? True : False;

        [Pure]
        public static string ToTSqlDate(this SmallDate source)
        {
            var dateTime = source.ToDateTime();
            const string format = "yyyyMMdd";
            return $"'{dateTime.ToString(format, CultureInfo.InvariantCulture)}'";
        }

        [Pure]
        public static string ToTSqlDateTime(this DateTime source)
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
        public static string ToTSqlDecimal(this decimal source) => source.ToString(NumberFormatInfo.InvariantInfo);

        [Pure]
        public static string ToTSqlInt(this int source) => source.ToString();

        [Pure]
        public static string ToTSqlNullableBit(this bool? source) =>
            source != null
                ? source.Value.ToTSqlBit()
                : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNullableDateTime(this DateTime? source) => source != null
            ? ToTSqlDateTime(source.Value)
            : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNullableFloat(this double? source) => source != null
            ? source.Value.ToString(CultureInfo.InvariantCulture)
            : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNullableInt(this int? source) => source != null
            ? source.Value.ToTSqlInt()
            : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNullableNVarChar(this string source) => source != null
            ? source.ToTSqlNVarChar()
            : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNullableVarChar(this string source) => source != null
            ? source.ToTSqlVarChar()
            : SqlNull.NullString;

        [Pure]
        public static string ToTSqlNVarChar(this string source)
        {
            Assert.IsNotNull(source);
            return "N" + source.ToTSqlVarChar();
        }

        [Pure]
        public static string ToTSqlUniqueIdentifier(this Guid source) => $"'{source}'";

        [Pure]
        public static string ToTSqlVarChar(this string source)
        {
            Assert.IsNotNull(source);
            return "'" + source.Replace("'", "''") + "'";
        }
    }
}