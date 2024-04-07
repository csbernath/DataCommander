using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public sealed class LoggedDbConnection : IDbConnection
{
    private readonly IDbConnection _connection;
    private EventHandler<BeforeOpenDbConnectionEventArgs> _beforeOpen;
    private EventHandler<AfterOpenDbConnectionEventArgs> _afterOpen;
    private EventHandler<BeforeExecuteCommandEventArgs> _beforeExecuteCommand;
    private EventHandler<AfterExecuteCommandEventArgs> _afterExecuteCommand;
    private EventHandler<AfterReadEventArgs> _afterRead;

    public LoggedDbConnection(IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        _connection = connection;
    }

    public event EventHandler<BeforeOpenDbConnectionEventArgs> BeforeOpen
    {
        add => _beforeOpen += value;

        remove => _beforeOpen -= value;
    }

    public event EventHandler<AfterOpenDbConnectionEventArgs> AfterOpen
    {
        add => _afterOpen += value;
        remove => _afterOpen -= value;
    }

    public event EventHandler<BeforeExecuteCommandEventArgs> BeforeExecuteReader
    {
        add => _beforeExecuteCommand += value;
        remove => _beforeExecuteCommand -= value;
    }

    public event EventHandler<AfterExecuteCommandEventArgs> AfterExecuteReader
    {
        add => _afterExecuteCommand += value;
        remove => _afterExecuteCommand -= value;
    }

    public event EventHandler<AfterReadEventArgs> AfterRead
    {
        add => _afterRead += value;

        remove => _afterRead -= value;
    }

    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => _connection.BeginTransaction(il);
    IDbTransaction IDbConnection.BeginTransaction() => _connection.BeginTransaction();
    void IDbConnection.ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);
    void IDbConnection.Close() => _connection.Close();

    string IDbConnection.ConnectionString
    {
        get => _connection.ConnectionString;
        set => _connection.ConnectionString = value;
    }

    int IDbConnection.ConnectionTimeout => _connection.ConnectionTimeout;

    IDbCommand IDbConnection.CreateCommand()
    {
        var command = _connection.CreateCommand();
        return new LoggedDbCommand(command, _beforeExecuteCommand, _afterExecuteCommand, _afterRead);
    }

    string IDbConnection.Database => _connection.Database;

    void IDbConnection.Open()
    {
        if (_beforeOpen != null)
        {
            var eventArgs = new BeforeOpenDbConnectionEventArgs(_connection.ConnectionString);
            _beforeOpen(this, eventArgs);
        }

        if (_afterOpen != null)
        {
            Exception exception = null;
            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var eventArgs = new AfterOpenDbConnectionEventArgs(exception);
                _afterOpen(this, eventArgs);
            }
        }
        else
        {
            _connection.Open();
        }
    }

    ConnectionState IDbConnection.State => _connection.State;

    void IDisposable.Dispose() => _connection.Dispose();
}