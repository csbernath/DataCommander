using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Foundation.Assertions;
using Foundation.Data.SqlClient;

namespace Foundation.Data;

public static class IDataParameterCollectionExtensions
{
    public static void AddRange(this IDataParameterCollection dataParameterCollection, IEnumerable<object> parameters)
    {
        foreach (var parameter in parameters)
            dataParameterCollection.Add(parameter);
    }

    public static string ToLogString(this IDataParameterCollection parameters)
    {
        Assert.IsNotNull(parameters);

        var stringBuilder = new StringBuilder();
        var first = true;

        foreach (IDataParameter parameter in parameters)
        {
            if (parameter.Direction != ParameterDirection.ReturnValue)
            {
                var value = parameter.Value;

                if (value != null)
                {
                    string valueString;

                    if (value == DBNull.Value)
                    {
                        valueString = SqlNull.NullString;
                    }
                    else
                    {
                        var dbType = parameter.DbType;

                        switch (dbType)
                        {
                            case DbType.DateTime:
                                var dateTime = (DateTime)value;
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

        var s = stringBuilder.ToString();

        return s;
    }
}