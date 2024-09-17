using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using Foundation.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlParameterCollectionExtensions
{
    public static void Add(this ICollection<SqlParameter> parameters, string parameterName, object value)
    {
        SqlParameter parameter = new SqlParameter(parameterName, value);
        parameters.Add(parameter);
    }

    public static void Add(this ICollection<SqlParameter> parameters, string parameterName, SqlDbType sqlDbType, object value)
    {
        SqlParameter parameter = SqlParameterFactory.Create(parameterName, sqlDbType, value);
        parameters.Add(parameter);
    }

    public static string ToLogString(this SqlParameterCollection parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;
        string s;
        NumberFormatInfo numberFormatInfo = NumberFormatInfo.InvariantInfo;

        foreach (SqlParameter parameter in parameters)
        {
            object value = parameter.Value;

            if (value != null)
            {
                switch (parameter.Direction)
                {
                    case ParameterDirection.ReturnValue:
                        break;

                    default:
                        if (first)
                            first = false;
                        else
                            stringBuilder.AppendLine(",");

                        if (value == DBNull.Value)
                            s = SqlNull.NullString;
                        else
                        {
                            Type type = value.GetType();
                            if (value is INullable nullable)
                            {
                                if (nullable.IsNull)
                                    s = SqlNull.NullString;
                                else
                                {
                                    switch (parameter.SqlDbType)
                                    {
                                        case SqlDbType.Bit:
                                            SqlBoolean sqlBoolean = (SqlBoolean)value;
                                            s = sqlBoolean.ByteValue.ToString();
                                            break;

                                        case SqlDbType.Char:
                                        case SqlDbType.VarChar:
                                        case SqlDbType.Text:
                                        case SqlDbType.NChar:
                                        case SqlDbType.NVarChar:
                                        case SqlDbType.NText:
                                            s = value.ToString();
                                            s = s.Replace("\'", "''");
                                            s = $"'{s}'";
                                            break;

                                        case SqlDbType.DateTime:
                                            SqlDateTime sqlDateTime = (SqlDateTime)value;
                                            DateTime dateTime = sqlDateTime.Value;
                                            s = dateTime.ToSqlConstant();
                                            break;

                                        case SqlDbType.Float:
                                            SqlDouble sqlDouble = (SqlDouble)value;
                                            double d = sqlDouble.Value;
                                            long i = (long)d;

                                            if (i == d)
                                                s = i.ToString(numberFormatInfo);
                                            else
                                                s = d.ToString(numberFormatInfo);

                                            break;

                                        case SqlDbType.Real:
                                            SqlSingle sqlSingle = (SqlSingle)value;
                                            s = sqlSingle.ToString();
                                            break;

                                        case SqlDbType.Decimal:
                                            SqlDecimal sqlDecimal = (SqlDecimal)value;
                                            s = sqlDecimal.ToString();
                                            break;

                                        case SqlDbType.Money:
                                            SqlMoney sqlMoney = (SqlMoney)value;
                                            decimal dec = sqlMoney.Value;
                                            i = (long)dec;
                                            s = i == dec ? i.ToString(numberFormatInfo) : dec.ToString(numberFormatInfo);
                                            break;

                                        case SqlDbType.SmallDateTime:
                                            sqlDateTime = (SqlDateTime)value;
                                            dateTime = sqlDateTime.Value;
                                            s = dateTime.ToSqlConstant();
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
                                    TypeCode elementTypeCode = Type.GetTypeCode(elementType);

                                    switch (elementTypeCode)
                                    {
                                        case TypeCode.Byte:
                                            byte[] bytes = (byte[])value;
                                            s = "0x" + Hex.GetString(bytes, true);
                                            break;

                                        default:
                                            s = value.ToString();
                                            break;
                                    }
                                }
                                else
                                {
                                    TypeCode typeCode = Type.GetTypeCode(type);

                                    switch (typeCode)
                                    {
                                        case TypeCode.Boolean:
                                            bool b = (bool)value;
                                            s = b ? "1" : "0";
                                            break;

                                        case TypeCode.String:
                                            s = (string)value;
                                            s = s.Replace("\'", "''");
                                            s = $"'{s}'";
                                            break;

                                        case TypeCode.DateTime:
                                            DateTime dateTime = (DateTime)value;
                                            s = dateTime.ToSqlConstant();
                                            break;

                                        case TypeCode.Decimal:
                                            decimal decimalValue = (decimal)value;
                                            s = decimalValue.ToString(numberFormatInfo);
                                            break;

                                        default:
                                            s = value.ToString();
                                            break;
                                    }
                                }
                            }
                        }

                        stringBuilder.AppendFormat("{0} = {1}", parameter.ParameterName, s);
                        break;
                }
            }
        }

        s = stringBuilder.ToString();

        if (s.Length == 0)
            s = null;

        return s;
    }
}