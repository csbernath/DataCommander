using System;
using System.Globalization;
using Foundation.Core;

namespace Foundation.Data.SqlClient
{
    public static class SqlStatementExtensions
    {
        private const byte True = 1;
        private const byte False = 0;

        public static byte ToTSqlBit(this bool source) => source ? True : False;

        public static string ToTSqlBit(this bool? source) =>
            source != null
                ? source.Value.ToTSqlBit().ToString()
                : SqlNull.NullString;

        public static string ToTSqlDate(this SmallDate source)
        {
            var dateTime = source.ToDateTime();
            const string format = "yyyyMMdd";
            return $"'{dateTime.ToString(format, CultureInfo.InvariantCulture)}'";
        }

        public static string ToTSqlDate(this DateTime? source) =>
            source != null
                ? ToTSqlDateTime(source.Value)
                : SqlNull.NullString;

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

        public static string ToTSqlDecimal(this decimal source) => source.ToString(NumberFormatInfo.InvariantInfo);

        public static string ToTSqlFloat(this double? source) => source != null
            ? source.Value.ToString(CultureInfo.InvariantCulture)
            : SqlNull.NullString;

        public static string ToTSqlInt(this int? source) => source != null
            ? source.ToString()
            : SqlNull.NullString;

        public static string ToTSqlUniqueIdentifier(this Guid source) => $"'{source}'";

        public static string ToTSqlVarChar(this string source)
        {
            string target;
            if (source != null)
                target = "'" + source.Replace("'", "''") + "'";
            else
                target = SqlNull.NullString;

            return target;
        }

        public static string ToTSqlNVarChar(this string source)
        {
            string target;
            if (source != null)
                target = "N'" + source.Replace("'", "''") + "'";
            else
                target = SqlNull.NullString;

            return target;
        }
    }
}