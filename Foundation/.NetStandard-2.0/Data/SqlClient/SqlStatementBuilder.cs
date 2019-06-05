using System;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data.SqlClient
{
    public static class SqlStatementBuilder
    {
        public static string ToString(byte? value)
        {
            var s = value != null ? value.Value.ToString() : SqlNull.NullString;
            return s;
        }

        public static string ToString(DBNull value) => SqlNull.NullString;

        public static string ToString(SqlDateTime value)
        {
            string s;

            if (value.IsNull)
                s = SqlNull.NullString;
            else
            {
                var format = value.TimeTicks == 0 ? "yyyyMMdd" : "yyyyMMdd HH:mm:ss.fff";
                s = value.Value.ToString(format);
            }

            return s;
        }

        public static string ToString(object value, SqlDbType sqlDbType)
        {
            var sb = new StringBuilder();

            if (value == null)
                sb.Append(SqlNull.NullString);
            else
            {
                switch (sqlDbType)
                {
                    case SqlDbType.Bit:
                        var b = (bool) value;
                        var i = b ? 1 : 0;
                        sb.Append(i);
                        break;

                    case SqlDbType.Binary:
                        var bytes = (byte[]) value;
                        var chars = Hex.Encode(bytes, true);
                        sb.Append("0x");
                        sb.Append(chars);
                        break;

                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                        sb.Append('\'');
                        var s = value.ToString();

                        if (s.IndexOf('\'') >= 0)
                            sb.Append(s.Replace("'", "''"));
                        else
                            sb.Append(s);

                        sb.Append('\'');
                        break;

                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                        sb.Append("N'");
                        s = value.ToString();
                        sb.Append(s.IndexOf('\'') >= 0 ? s.Replace("'", "''") : s);
                        sb.Append('\'');
                        break;

                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                        var dateTime = (DateTime) value;
                        var dateTimeStr = ToString(dateTime);
                        sb.Append('\'');
                        sb.Append(dateTimeStr);
                        sb.Append('\'');
                        break;

                    case SqlDbType.Decimal:
                        var d = (decimal) value;
                        sb.Append(d.ToSqlConstant());
                        break;

                    default:
                        sb.Append(value);
                        break;
                }
            }

            return sb.ToString();
        }

        public static void AppendToCommandText(StringBuilder commandText, string value)
        {
            Assert.IsNotNull(commandText);

            var s = value.ToNullableNVarChar();
            commandText.Append(s);
        }

        public static void AppendToCommandText(StringBuilder commandText, object value, SqlDbType sqlDbType)
        {
            Assert.IsNotNull(commandText);

            var s = ToString(value, sqlDbType);
            commandText.Append(s);
        }

        public static void AppendToCommandText(StringBuilder commandText, SqlBoolean sqlBoolean)
        {
            var obj = sqlBoolean.IsNull ? (object) null : sqlBoolean.Value;
            var s = ToString(obj, SqlDbType.Bit);
            commandText.Append(s);
        }

        public static void AppendToCommandText(StringBuilder commandText, SqlInt16 value)
        {
            var obj = value.IsNull ? (object) null : value.Value;
            var s = ToString(obj, SqlDbType.SmallInt);
            commandText.Append(s);
        }

        public static void AppendToCommandText(StringBuilder commandText, SqlDateTime sqlDateTime)
        {
            var obj = sqlDateTime.IsNull ? (object) null : sqlDateTime.Value;
            var s = ToString(obj, SqlDbType.DateTime);
            commandText.Append(s);
        }
    }
}