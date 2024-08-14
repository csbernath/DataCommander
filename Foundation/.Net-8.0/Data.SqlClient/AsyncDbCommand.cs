using System;
using System.Data;
using Foundation.Assertions;

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
            Assert.IsTrue(_command != null);
            return _command.CommandType;
        }

        set => _command.CommandType = value;
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        // TODO:  Add AsyncDbCommand.ExecuteReader implementation
        return null;
    }

    IDataReader IDbCommand.ExecuteReader()
    {
        // TODO:  Add AsyncDbCommand.System.Data.IDbCommand.ExecuteReader implementation
        return null;
    }

    public object ExecuteScalar()
    {
        // TODO:  Add AsyncDbCommand.ExecuteScalar implementation
        return null;
    }

    public int ExecuteNonQuery()
    {
        Assert.IsTrue(_connection != null);
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

    public IDbDataParameter CreateParameter()
    {
        // TODO:  Add AsyncDbCommand.CreateParameter implementation
        return null;
    }

    public IDbConnection Connection
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

    public string CommandText
    {
        get
        {
            Assert.IsTrue(_command != null);
            return _command.CommandText;
        }

        set
        {
            Assert.IsTrue(_command != null);
            _command.CommandText = value;
        }
    }

    public IDataParameterCollection Parameters
    {
        get
        {
            Assert.IsTrue(_command != null);
            return _command.Parameters;
        }
    }

    public IDbTransaction Transaction
    {
        get
        {
            Assert.IsTrue(_command != null);

            return _command.Transaction;
        }

        set
        {
            Assert.IsTrue(_command != null);

            _command.Transaction = value;
        }
    }

    public void Dispose()
    {
        // TODO:  Add AsyncDbCommand.Dispose implementation
    }
}