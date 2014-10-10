using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlParameterCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parameterName"></param>
        /// <param name="sqlDataRecords"></param>
        public static void AddStructured(this SqlParameterCollection parameters, String parameterName, IEnumerable<SqlDataRecord> sqlDataRecords)
        {
            Contract.Requires(parameters != null);

            var parameter = new SqlParameter(parameterName, SqlDbType.Structured);

            if (sqlDataRecords.Any())
            {
                parameter.Value = sqlDataRecords;
            }

            parameters.Add(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static String ToLogString( this SqlParameterCollection parameters )
        {
            Contract.Requires( parameters != null );

            StringBuilder sb = new StringBuilder();
            Boolean first = true;
            String s;
            var numberFormatInfo = NumberFormatInfo.InvariantInfo;

            foreach (SqlParameter parameter in parameters)
            {
                Object value = parameter.Value;

                if (value != null)
                {
                    switch (parameter.Direction)
                    {
                        case ParameterDirection.ReturnValue:
                            break;

                        default:
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                sb.AppendLine( "," );
                            }

                            if (value == DBNull.Value)
                            {
                                s = SqlNull.NullString;
                            }
                            else
                            {
                                Type type = value.GetType();
                                INullable nullable = value as INullable;

                                if (nullable != null)
                                {
                                    if (nullable.IsNull)
                                    {
                                        s = SqlNull.NullString;
                                    }
                                    else
                                    {
                                        switch (parameter.SqlDbType)
                                        {
                                            case SqlDbType.Bit:
                                                SqlBoolean sqlBoolean = (SqlBoolean) value;
                                                s = sqlBoolean.ByteValue.ToString();
                                                break;

                                            case SqlDbType.Char:
                                            case SqlDbType.VarChar:
                                            case SqlDbType.Text:
                                            case SqlDbType.NChar:
                                            case SqlDbType.NVarChar:
                                            case SqlDbType.NText:
                                                s = value.ToString();
                                                s = s.Replace( "\'", "''" );
                                                s = String.Format( "'{0}'", s );
                                                break;

                                            case SqlDbType.DateTime:
                                                SqlDateTime sqlDateTime = (SqlDateTime) value;
                                                DateTime dateTime = sqlDateTime.Value;
                                                s = dateTime.ToTSqlDateTime();
                                                break;

                                            case SqlDbType.Float:
                                                SqlDouble sqlDouble = (SqlDouble) value;
                                                Double d = sqlDouble.Value;
                                                Int64 i = (Int64) d;

                                                if (i == d)
                                                    s = i.ToString( numberFormatInfo );
                                                else
                                                    s = d.ToString( numberFormatInfo );

                                                break;

                                            case SqlDbType.Real:
                                                SqlSingle sqlSingle = (SqlSingle) value;
                                                s = sqlSingle.ToString();
                                                break;

                                            case SqlDbType.Decimal:
                                                SqlDecimal sqlDecimal = (SqlDecimal) value;
                                                s = sqlDecimal.ToString();
                                                break;

                                            case SqlDbType.Money:
                                                SqlMoney sqlMoney = (SqlMoney) value;
                                                Decimal dec = sqlMoney.Value;
                                                i = (Int64) dec;

                                                if (i == dec)
                                                    s = i.ToString( numberFormatInfo );
                                                else
                                                    s = dec.ToString( numberFormatInfo );

                                                break;

                                            case SqlDbType.SmallDateTime:
                                                sqlDateTime = (SqlDateTime) value;
                                                dateTime = sqlDateTime.Value;
                                                s = dateTime.ToTSqlDateTime();
                                                break;

                                            default:
                                                s = value.ToString();
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (type.IsArray)
                                    {
                                        Type elementType = type.GetElementType();
                                        TypeCode elementTypeCode = Type.GetTypeCode( elementType );

                                        switch (elementTypeCode)
                                        {
                                            case TypeCode.Byte:
                                                Byte[] bytes = (Byte[]) value;
                                                s = "0x" + Hex.GetString( bytes, true );
                                                break;

                                            default:
                                                s = value.ToString();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        TypeCode typeCode = Type.GetTypeCode( type );

                                        switch (typeCode)
                                        {
                                            case TypeCode.Boolean:
                                                Boolean b = (Boolean) value;
                                                s = b ? "1" : "0";
                                                break;

                                            case TypeCode.String:
                                                s = (String) value;
                                                s = s.Replace( "\'", "''" );
                                                s = String.Format( "'{0}'", s );
                                                break;

                                            case TypeCode.DateTime:
                                                DateTime dateTime = (DateTime) value;
                                                s = dateTime.ToTSqlDateTime();
                                                break;

                                            case TypeCode.Decimal:
                                                decimal decimalValue = (decimal) value;
                                                s = decimalValue.ToString( numberFormatInfo );
                                                break;

                                            default:
                                                s = value.ToString();
                                                break;
                                        }
                                    }
                                }
                            }

                            sb.AppendFormat( "{0} = {1}", parameter.ParameterName, s );
                            break;
                    }
                }
            }

            s = sb.ToString();

            if (s.Length == 0)
            {
                s = null;
            }

            return s;
        }
    }
}