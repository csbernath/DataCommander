using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public static class SqlCommandExtensions
{
    extension(SqlCommand command)
    {
        public string ToLogString()
        {
            ArgumentNullException.ThrowIfNull(command);

            var stringBuilder = new StringBuilder();
            switch (command.CommandType)
            {
                case CommandType.StoredProcedure:
                    stringBuilder.Append("exec ");
                    stringBuilder.AppendLine(command.CommandText);
                    stringBuilder.Append(command.Parameters.ToLogString());
                    break;

                case CommandType.Text:
                    var parameters = command.Parameters;
                    if (parameters.Count > 0)
                    {
                        var parametersString = GetSpExecuteSqlParameters(parameters);
                        stringBuilder.Append(
                            $"exec sp_executesql {command.CommandText.ToNullableNVarChar()},{parametersString.ToNullableNVarChar()}");

                        stringBuilder.Append(',');
                        stringBuilder.Append(command.Parameters.ToLogString());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;
            }

            return stringBuilder.ToString();
        }
    }

    private static string GetSpExecuteSqlParameters(SqlParameterCollection parameters)
    {
        var sb = new StringBuilder();
        var first = true;
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

            var dataTypeName = parameter.GetDataTypeName();
            sb.Append($"{parameter.ParameterName} {dataTypeName}");
        }
        return sb.ToString();
    }
}