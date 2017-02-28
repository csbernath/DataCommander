namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data.SqlTypes;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlStatementExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        private const byte True = 1;

        /// <summary>
        /// 
        /// </summary>
        private const byte False = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte ToTSqlBit(this bool source)
        {
            return source ? True : False;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTSqlDateTime(this DateTime source)
        {
            var timeOfDay = source.TimeOfDay;
            string format;

            if (timeOfDay.TotalMilliseconds == 0)
            {
                format = "yyyyMMdd";
            }
            else
            {
                if (source.Millisecond == 0)
                {
                    format = "yyyyMMdd HH:mm:ss";
                }
                else
                {
                    format = "yyyyMMdd HH:mm:ss.fff";
                }
            }

            return $"'{source.ToString(format, CultureInfo.InvariantCulture)}'";
        }

        /// <summary>
        /// Converts a <see cref="System.Decimal"/> value to Microsoft SQL Server decimal string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTSqlDecimal(this decimal source)
        {
            return source.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTSqlInt(this int? source)
        {
            return source != null ? source.ToString() : SqlNull.NullString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTSqlVarChar(this string source)
        {
            string target;
            if (source != null)
            {
                target = "'" + source.Replace("'", "''") + "'";
            }
            else
            {
                target = SqlNull.NullString;
            }
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTSqlNVarChar(this string source)
        {
            string target;
            if (source != null)
            {
                target = "N'" + source.Replace("'", "''") + "'";
            }
            else
            {
                target = SqlNull.NullString;
            }
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlDateTime ToSqlDateTime(this DateTime? value)
        {
            SqlDateTime returnValue;

            if (value != null)
            {
                returnValue = value.Value;
            }
            else
            {
                returnValue = SqlDateTime.Null;
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlInt32 ToSqlInt32(this int? value)
        {
            SqlInt32 returnValue;

            if (value != null)
            {
                returnValue = value.Value;
            }
            else
            {
                returnValue = SqlInt32.Null;
            }

            return returnValue;
        }
    }
}