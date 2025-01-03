using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogError : ISqlLogItem
{
    private readonly int _applicationId;
    private readonly int _connectionNo;
    private readonly int _commandNo;
    private readonly int _executionNo;
    private readonly Exception _exception;

    public SqlLogError(
        int applicationId,
        int connectionNo,
        int commandNo,
        int executionNo,
        Exception exception)
    {
        _applicationId = applicationId;
        _connectionNo = connectionNo;
        _commandNo = commandNo;
        _executionNo = executionNo;
        _exception = exception;
    }

    public string CommandText
    {
        get
        {
            var sb = new StringBuilder();
            var sqlEx = _exception as SqlException;

            if (sqlEx != null)
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
                var text = _exception.ToString();
                sb.AppendFormat("exec LogException {0},{1},{2},{3},{4}", _applicationId, _connectionNo, _commandNo, _executionNo, text.ToNullableVarChar());
            }

            return sb.ToString();
        }
    }

    private void AppendError(StringBuilder sb, int errorNo, int error, byte severity, byte state, string procedure, int line, string message)
    {
        sb.AppendFormat(
            "exec LogError {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
            _applicationId,
            _connectionNo,
            _commandNo,
            _executionNo,
            errorNo,
            error,
            severity,
            state,
            procedure.ToNullableVarChar(),
            message.ToNullableVarChar());
    }
}