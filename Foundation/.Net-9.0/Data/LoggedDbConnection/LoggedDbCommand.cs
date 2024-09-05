using System;
using System.Data;
using System.Threading;

namespace Foundation.Data.LoggedDbConnection;

internal sealed class LoggedDbCommand : IDbCommand
{
    private static int _commandIdCounter;
    private readonly int _commandId;
    private readonly IDbCommand _command;
    private readonly EventHandler<BeforeExecuteCommandEventArgs> _beforeExecuteCommand;
    private readonly EventHandler<AfterExecuteCommandEventArgs> _afterExecuteCommand;
    private readonly EventHandler<AfterReadEventArgs> _afterRead;

    public LoggedDbCommand(
        IDbCommand command,
        EventHandler<BeforeExecuteCommandEventArgs> beforeExecuteCommand,
        EventHandler<AfterExecuteCommandEventArgs> afterExecuteCommand,
        EventHandler<AfterReadEventArgs> afterRead)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(beforeExecuteCommand);
        ArgumentNullException.ThrowIfNull(afterExecuteCommand);
        ArgumentNullException.ThrowIfNull(afterRead);

        _commandId = Interlocked.Increment(ref _commandIdCounter);
        _command = command;
        _beforeExecuteCommand = beforeExecuteCommand;
        _afterExecuteCommand = afterExecuteCommand;
        _afterRead = afterRead;
    }

    void IDbCommand.Cancel()
    {
        _command.Cancel();
    }

    string IDbCommand.CommandText
    {
        get => _command.CommandText;

        set => _command.CommandText = value;
    }

    int IDbCommand.CommandTimeout
    {
        get => _command.CommandTimeout;

        set => _command.CommandTimeout = value;
    }

    CommandType IDbCommand.CommandType
    {
        get => _command.CommandType;

        set => _command.CommandType = value;
    }

    IDbConnection IDbCommand.Connection
    {
        get => _command.Connection;

        set => _command.Connection = value;
    }

    IDbDataParameter IDbCommand.CreateParameter()
    {
        return _command.CreateParameter();
    }

    int IDbCommand.ExecuteNonQuery()
    {
        var commandInfo = new Lazy<LoggedDbCommandInfo>(() => CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.NonQuery));

        if (_beforeExecuteCommand != null)
        {
            var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
            _beforeExecuteCommand(this, eventArgs);
        }

        int rowCount;

        if (_afterExecuteCommand != null)
        {
            Exception exception = null;
            try
            {
                rowCount = _command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                _afterExecuteCommand(this, eventArgs);
            }
        }
        else
        {
            rowCount = _command.ExecuteNonQuery();
        }

        return rowCount;
    }

    IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
    {
        var commandInfo = new Lazy<LoggedDbCommandInfo>(() => CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Reader));

        if (_beforeExecuteCommand != null)
        {
            var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
            _beforeExecuteCommand(this, eventArgs);
        }

        IDataReader dataReader;

        if (_afterExecuteCommand != null)
        {
            Exception exception = null;
            try
            {
                dataReader = _command.ExecuteReader();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var eventArgs = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                _afterExecuteCommand(this, eventArgs);
            }
        }
        else
        {
            dataReader = _command.ExecuteReader();
        }

        return new LoggedDataReader(dataReader, _afterRead);
    }

    IDataReader IDbCommand.ExecuteReader()
    {
        var dbCommand = (IDbCommand)this;
        return dbCommand.ExecuteReader(CommandBehavior.Default);
    }

    object IDbCommand.ExecuteScalar()
    {
        var commandInfo = new Lazy<LoggedDbCommandInfo>(() => CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType.Scalar));
        if (_beforeExecuteCommand != null)
        {
            var eventArgs = new BeforeExecuteCommandEventArgs(commandInfo.Value);
            _beforeExecuteCommand(this, eventArgs);
        }

        object scalar;

        if (_afterExecuteCommand != null)
        {
            Exception exception = null;
            try
            {
                scalar = _command.ExecuteScalar();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var args = new AfterExecuteCommandEventArgs(commandInfo.Value, exception);
                _afterExecuteCommand(this, args);
            }
        }
        else
        {
            scalar = _command.ExecuteScalar();
        }

        return scalar;
    }

    IDataParameterCollection IDbCommand.Parameters => _command.Parameters;

    void IDbCommand.Prepare()
    {
        _command.Prepare();
    }

    IDbTransaction IDbCommand.Transaction
    {
        get => _command.Transaction;

        set => _command.Transaction = value;
    }

    UpdateRowSource IDbCommand.UpdatedRowSource
    {
        get => _command.UpdatedRowSource;

        set => _command.UpdatedRowSource = value;
    }

    void IDisposable.Dispose()
    {
        _command.Dispose();
    }

    private LoggedDbCommandInfo CreateLoggedDbCommandInfo(LoggedDbCommandExecutionType executionType)
    {
        var connection = _command.Connection;

        return new LoggedDbCommandInfo(
            _commandId,
            connection.State,
            connection.Database,
            executionType,
            _command.CommandType,
            _command.CommandTimeout,
            _command.CommandText,
            _command.Parameters.ToLogString());
    }
}