using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Foundation.Core;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqlLogCommand : ISqlLogItem
{
    private readonly int _applicationId;
    private readonly IDictionary<string, SqLoglCommandExecution> _commands;
    private readonly int _connectionNo;
    private readonly string _database;
    private readonly CommandType _commandType;
    private string _commandText;
    private string _parameters;
    private readonly DateTime _startDate;
    private readonly long _duration;
    private readonly Exception _exception;

    public SqlLogCommand(
        int applicationId,
        IDictionary<string, SqLoglCommandExecution> commands,
        int connectionNo,
        IDbCommand command,
        DateTime startDate,
        long duration,
        Exception exception)
    {
        _applicationId = applicationId;
        _commands = commands;
        _connectionNo = connectionNo;
        _database = command.Connection.Database;
        _commandType = command.CommandType;
        _commandText = command.CommandText;
        var parameters = (SqlParameterCollection)command.Parameters;

        if (parameters.Count > 0)
        {
            _parameters = parameters.ToLogString();
        }

        _startDate = startDate;
        _duration = duration;
        _exception = exception;
    }

    string ISqlLogItem.CommandText
    {
        get
        {
            switch (_commandType)
            {
                case CommandType.Text:
                    var index = _commandText.IndexOf(' ');

                    if (index >= 0)
                    {
                        var word = _commandText[..index];
                        word = word.ToLower();

                        switch (word)
                        {
                            case "exec":
                            case "execute":
                                var startIndex = word.Length + 1;
                                var endIndex = _commandText.IndexOf(' ', startIndex);

                                if (endIndex >= 0)
                                {
                                    word = _commandText[..endIndex];
                                    _parameters = _commandText[(endIndex + 1)..];
                                    _commandText = word;
                                }

                                break;
                        }
                    }

                    break;

                default:
                    break;
            }

            var command = GetCommandExecution(_database, _commandText, out var isNew);
            var sb = new StringBuilder();

            if (isNew)
            {
                sb.AppendFormat("exec LogCommand {0},{1},{2},{3}\r\n", _applicationId, command.CommandNo, _database.ToNullableVarChar(),
                    _commandText.ToNullableVarChar());
            }

            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "exec LogCommandExecute {0},{1},{2},{3},",
                _applicationId,
                _connectionNo,
                command.CommandNo,
                command.ExecutionNo);

            sb.Append(_parameters.ToNullableVarChar());
            sb.Append(',');
            sb.Append(_startDate.ToSqlConstant());

            var microseconds = StopwatchTimeSpan.ToInt32(_duration, 1000000);
            sb.AppendFormat(",{0}\r\n", microseconds);

            if (_exception != null)
            {
                var error = new SqlLogError(_applicationId, _connectionNo, command.CommandNo, command.ExecutionNo, _exception);
                sb.Append(error.CommandText);
            }

            return sb.ToString();
        }
    }

    private SqLoglCommandExecution GetCommandExecution(
        string database,
        string commandText,
        out bool isNew)
    {
        SqLoglCommandExecution command;
        isNew = false;
        var key = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", database, commandText);

        lock (_commands)
        {
            if (_commands.TryGetValue(key, out command))
            {
                command!.ExecutionNo++;
            }
            else
            {
                var commandNo = _commands.Count + 1;
                command = new SqLoglCommandExecution(commandNo);
                _commands.Add(key, command);
                isNew = true;
            }
        }

        return command;
    }
}