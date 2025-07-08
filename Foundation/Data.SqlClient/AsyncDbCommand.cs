using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Foundation.Data.SqlClient;

public class AsyncDbCommand : IDbCommand
{
    private readonly AsyncDbConnection _connection;
    private readonly IDbCommand _command;

    internal AsyncDbCommand(AsyncDbConnection connection, IDbCommand command)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(command);

        _connection = connection;
        _command = command;
    }

    public void Cancel()
    {
        // TODO:  Add AsyncDbCommand.Cancel implementation
    }

    public void Prepare()
    {
        // TODO:  Add AsyncDbCommand.Prepare implementation
    }

    public CommandType CommandType
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_command);
            return _command!.CommandType;
        }

        set => _command.CommandType = value;
    }

    public IDataReader ExecuteReader(CommandBehavior behavior) => throw new NotImplementedException();

    IDataReader IDbCommand.ExecuteReader() => throw new NotImplementedException();

    public object ExecuteScalar() => throw new NotImplementedException();

    public int ExecuteNonQuery()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        return _connection.ExecuteNonQuery(this);
    }

    public int CommandTimeout
    {
        get => 0;

        set
        {
            // TODO:  Add AsyncDbCommand.CommandTimeout setter implementation
        }
    }

    public IDbDataParameter CreateParameter() => throw new NotImplementedException();

    public IDbConnection? Connection
    {
        get => null;

        set
        {
            // TODO:  Add AsyncDbCommand.Connection setter implementation
        }
    }

    public UpdateRowSource UpdatedRowSource
    {
        get => new();

        set
        {
            // TODO:  Add AsyncDbCommand.UpdatedRowSource setter implementation
        }
    }

    [AllowNull]
    public string CommandText
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_command);
            return _command.CommandText;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(_command);
            _command.CommandText = value;
        }
    }

    public IDataParameterCollection Parameters
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_command);
            return _command.Parameters;
        }
    }

    public IDbTransaction? Transaction
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_command);
            return _command.Transaction;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(_command);
            _command.Transaction = value;
        }
    }

    public void Dispose()
    {
        // TODO:  Add AsyncDbCommand.Dispose implementation
    }
}