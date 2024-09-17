using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogError(
    int applicationId,
    int connectionNo,
    int commandNo,
    int executionNo,
    Exception exception)
    : ISqlLogItem
{
    public string CommandText
    {
        get
        {
            var sb = new StringBuilder();

            if (exception is SqlException sqlEx)
            {
                var errors = sqlEx.Errors;
                var count = errors.Count;

                for (var i = 0; i < count; i++)
                {
                    var error = errors[i];
                    var procedure = error.Procedure;

                    if (procedure.Length == 0)
                        procedure = null;

                    AppendError(sb, i + 1, error.Number, error.Class, error.State, procedure, error.LineNumber, error.Message);
                }
            }
            else
            {
                var text = exception.ToString();
                sb.AppendFormat("exec LogException {0},{1},{2},{3},{4}", applicationId, connectionNo, commandNo, executionNo, text.ToNullableVarChar());
            }

            return sb.ToString();
        }
    }

    private void AppendError(StringBuilder sb, int errorNo, int error, byte severity, byte state, string procedure, int line, string message)
    {
        sb.AppendFormat(
            "exec LogError {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
            applicationId,
            connectionNo,
            commandNo,
            executionNo,
            errorNo,
            error,
            severity,
            state,
            procedure.ToNullableVarChar(),
            message.ToNullableVarChar());
    }
}