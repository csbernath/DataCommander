using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlCommandExtensions
{
    public static string ToLogString(this SqlCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        StringBuilder sb = new StringBuilder();
        switch (command.CommandType)
        {
            case CommandType.StoredProcedure:
                sb.Append("exec ");
                sb.AppendLine(command.CommandText);
                sb.Append(command.Parameters.ToLogString());
                break;

            case CommandType.Text:
                SqlParameterCollection parameters = command.Parameters;
                if (parameters.Count > 0)
                {
                    string parametersString = GetSpExecuteSqlParameters(parameters);
                    sb.AppendFormat(
                        "exec sp_executesql {0},{1}",
                        command.CommandText.ToNullableNVarChar(),
                        parametersString.ToNullableNVarChar());

                    sb.Append(',');
                    sb.Append(command.Parameters.ToLogString());
                }
                else
                {
                    throw new NotImplementedException();
                }
                break;
        }

        return sb.ToString();
    }

    private static string GetSpExecuteSqlParameters(SqlParameterCollection parameters)
    {
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (SqlParameter parameter in parameters)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(',');
            }

            string dataTypeName = parameter.GetDataTypeName();
            sb.AppendFormat("{0} {1}", parameter.ParameterName, dataTypeName);
        }
        return sb.ToString();
    }
}