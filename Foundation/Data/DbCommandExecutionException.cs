using System;
using System.Data;
using System.Text;

namespace Foundation.Data;

public class DbCommandExecutionException(string message, Exception innerException, IDbCommand command) : Exception(message, innerException)
{
    private readonly string _database = command.Connection!.Database;
    private readonly string _commandText = command.ToLogString();
    private readonly int _commandTimeout = command.CommandTimeout;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("DbCommandExecutionException: {0}\r\ninnerException:\r\n{1}\r\ndatabase: {2}\r\ncommandTimeout: {3}\r\ncommandText: {4}",
            Message,
            InnerException!.ToLogString(),
            _database,
            _commandTimeout,
            _commandText);
        var s = sb.ToString();
        return s;
    }
}