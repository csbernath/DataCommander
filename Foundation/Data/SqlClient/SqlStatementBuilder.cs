using System;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlStatementBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(byte? value)
        {
            string s;

            if (value != null)
            {
                s = value.Value.ToString();
            }
            else
            {
                s = SqlNull.NullString;
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(DBNull value)
        {
            return SqlNull.NullString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(SqlDateTime value)
        {
            string s;

            if (value.IsNull)
            {
                s = SqlNull.NullString;
            }
            else
            {
                string format;

                if (value.TimeTicks == 0)
                {
                    format = "yyyyMMdd";
                }
                else
                {
                    format = "yyyyMMdd HH:mm:ss.fff";
                }

                s = value.Value.ToString(format);
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static string ToString(object value, SqlDbType sqlDbType)
        {
            var sb = new StringBuilder();

            if (value == null)
            {
                sb.Append(SqlNull.NullString);
            }
            else
            {
                switch (sqlDbType)
                {
                    case SqlDbType.Bit:
                        var b = (bool)value;
                        var i = b ? 1 : 0;
                        sb.Append(i);
                        break;

                    case SqlDbType.Binary:
                        var bytes = (byte[])value;
                        var chars = Hex.Encode(bytes, true);
                        sb.Append("0x");
                        sb.Append(chars);
                        break;

                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                        sb.Append('\'');
                        var s = value.ToString();

                        if (s.IndexOf('\'') >= 0)
                        {
                            sb.Append(s.Replace("'", "''"));
                        }
                        else
                        {
                            sb.Append(s);
                        }

                        sb.Append('\'');
                        break;

                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                        sb.Append("N'");
                        s = value.ToString();

                        if (s.IndexOf('\'') >= 0)
                        {
                            sb.Append(s.Replace("'", "''"));
                        }
                        else
                        {
                            sb.Append(s);
                        }

                        sb.Append('\'');
                        break;

                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                        var dateTime = (DateTime)value;
                        var dateTimeStr = ToString(dateTime);
                        sb.Append('\'');
                        sb.Append(dateTimeStr);
                        sb.Append('\'');
                        break;

                    case SqlDbType.Decimal:
                        var d = (decimal)value;
                        sb.Append(d.ToTSqlDecimal());
                        break;

                    default:
                        sb.Append(value);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            string value)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(commandText != null);
#endif
            var s = value.ToTSqlNVarChar();
            commandText.Append(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            object value,
            SqlDbType sqlDbType)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(commandText != null);
#endif

            var s = ToString(value, sqlDbType);
            commandText.Append(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlBoolean"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlBoolean sqlBoolean)
        {
            object obj;

            if (sqlBoolean.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = sqlBoolean.Value;
            }

            var s = ToString(obj, SqlDbType.Bit);
            commandText.Append(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlInt16 value)
        {
            object obj;

            if (value.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = value.Value;
            }

            var s = ToString(obj, SqlDbType.SmallInt);
            commandText.Append(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlDateTime"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlDateTime sqlDateTime)
        {
            object obj;

            if (sqlDateTime.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = sqlDateTime.Value;
            }

            var s = ToString(obj, SqlDbType.DateTime);
            commandText.Append(s);
        }
    }
}