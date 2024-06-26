﻿using System;
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

    /// <summary>
    /// 
    /// </summary>
    public void Cancel()
    {
        // TODO:  Add AsyncDbCommand.Cancel implementation
    }

    /// <summary>
    /// 
    /// </summary>
    public void Prepare()
    {
        // TODO:  Add AsyncDbCommand.Prepare implementation
    }

    /// <summary>
    /// 
    /// </summary>
    public CommandType CommandType
    {
        get
        {
            Assert.IsTrue(_command != null);
            return _command.CommandType;
        }

        set => _command.CommandType = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="behavior"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object ExecuteScalar()
    {
        // TODO:  Add AsyncDbCommand.ExecuteScalar implementation
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int ExecuteNonQuery()
    {
        Assert.IsTrue(_connection != null);
        return _connection.ExecuteNonQuery(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public int CommandTimeout
    {
        get => 0;

        set
        {
            // TODO:  Add AsyncDbCommand.CommandTimeout setter implementation
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IDbDataParameter CreateParameter()
    {
        // TODO:  Add AsyncDbCommand.CreateParameter implementation
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    public IDbConnection Connection
    {
        get => null;

        set
        {
            // TODO:  Add AsyncDbCommand.Connection setter implementation
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public UpdateRowSource UpdatedRowSource
    {
        get => new();

        set
        {
            // TODO:  Add AsyncDbCommand.UpdatedRowSource setter implementation
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public IDataParameterCollection Parameters
    {
        get
        {
            Assert.IsTrue(_command != null);
            return _command.Parameters;
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        // TODO:  Add AsyncDbCommand.Dispose implementation
    }
}