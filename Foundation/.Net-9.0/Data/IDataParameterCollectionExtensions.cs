using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Foundation.Data.SqlClient;

namespace Foundation.Data;

public static class IDataParameterCollectionExtensions
{
    public static void AddRange(this IDataParameterCollection dataParameterCollection, IEnumerable<object> parameters)
    {
        foreach (object parameter in parameters)
            dataParameterCollection.Add(parameter);
    }

    public static string ToLogString(this IDataParameterCollection parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;

        foreach (IDataParameter parameter in parameters)
        {
            if (parameter.Direction != ParameterDirection.ReturnValue)
            {
                object value = parameter.Value;

                if (value != null)
                {
                    string valueString;

                    if (value == DBNull.Value)
                    {
                        valueString = SqlNull.NullString;
                    }
                    else
                    {
                        DbType dbType = parameter.DbType;

                        switch (dbType)
                        {
                            case DbType.DateTime:
                                DateTime dateTime = (DateTime)value;
                                valueString = dateTime.ToSqlConstant();
                                break;

                            case DbType.Int32:
                                valueString = value.ToString();
                                break;

                            case DbType.String:
                                valueString = "'" + value.ToString().Replace("'", "''") + "'";
                                break;

                            default:
                                valueString = value.ToString();
                                break;
                        }
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringBuilder.AppendLine(",");
                    }

                    stringBuilder.AppendFormat("  {0} = {1}", parameter.ParameterName, valueString);
                }
            }
        }

        string s = stringBuilder.ToString();

        return s;
    }
}