namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Diagnostics.Contracts;
    using System.Text;
    using DataCommander.Foundation.Text;

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
        public static String ToString( Byte? value )
        {
            String s;

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
        public static String ToString( DBNull value )
        {
            return SqlNull.NullString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToString( SqlDateTime value )
        {
            String s;

            if (value.IsNull)
            {
                s = SqlNull.NullString;
            }
            else
            {
                String format;

                if (value.TimeTicks == 0)
                {
                    format = "yyyyMMdd";
                }
                else
                {
                    format = "yyyyMMdd HH:mm:ss.fff";
                }

                s = value.Value.ToString( format );
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static String ToString( Object value, SqlDbType sqlDbType )
        {
            var sb = new StringBuilder();

            if (value == null)
            {
                sb.Append( SqlNull.NullString );
            }
            else
            {
                switch (sqlDbType)
                {
                    case SqlDbType.Bit:
                        Boolean b = (Boolean) value;
                        Int32 i = b ? 1 : 0;
                        sb.Append( i );
                        break;

                    case SqlDbType.Binary:
                        Byte[] bytes = (Byte[]) value;
                        Char[] chars = Hex.Encode( bytes, true );
                        sb.Append( "0x" );
                        sb.Append( chars );
                        break;

                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                        sb.Append( '\'' );
                        String s = value.ToString();

                        if (s.IndexOf( '\'' ) >= 0)
                        {
                            sb.Append( s.Replace( "'", "''" ) );
                        }
                        else
                        {
                            sb.Append( s );
                        }

                        sb.Append( '\'' );
                        break;

                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                        sb.Append( "N'" );
                        s = value.ToString();

                        if (s.IndexOf( '\'' ) >= 0)
                        {
                            sb.Append( s.Replace( "'", "''" ) );
                        }
                        else
                        {
                            sb.Append( s );
                        }

                        sb.Append( '\'' );
                        break;

                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                        DateTime dateTime = (DateTime) value;
                        String dateTimeStr = ToString( dateTime );
                        sb.Append( '\'' );
                        sb.Append( dateTimeStr );
                        sb.Append( '\'' );
                        break;

                    case SqlDbType.Decimal:
                        Decimal d = (Decimal) value;
                        sb.Append( d.ToTSqlDecimal() );
                        break;

                    default:
                        sb.Append( value );
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
            String value )
        {
            Contract.Requires( commandText != null );
            string s = value.ToTSqlNVarChar();
            commandText.Append( s );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            Object value,
            SqlDbType sqlDbType )
        {
            Contract.Requires( commandText != null );

            String s = ToString( value, sqlDbType );
            commandText.Append( s );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlBoolean"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlBoolean sqlBoolean )
        {
            Object obj;

            if (sqlBoolean.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = sqlBoolean.Value;
            }

            String s = ToString( obj, SqlDbType.Bit );
            commandText.Append( s );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlInt16 value )
        {
            Object obj;

            if (value.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = value.Value;
            }

            String s = ToString( obj, SqlDbType.SmallInt );
            commandText.Append( s );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlDateTime"></param>
        public static void AppendToCommandText(
            StringBuilder commandText,
            SqlDateTime sqlDateTime )
        {
            Object obj;

            if (sqlDateTime.IsNull)
            {
                obj = null;
            }
            else
            {
                obj = sqlDateTime.Value;
            }

            String s = ToString( obj, SqlDbType.DateTime );
            commandText.Append( s );
        }
    }
}