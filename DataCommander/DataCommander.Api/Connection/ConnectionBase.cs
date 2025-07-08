using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api.Connection;

public abstract class ConnectionBase : IDisposable, IAsyncDisposable
{
    private DbConnection? _connection;

    public DbConnection Connection => _connection!;
    protected void SetConnection(DbConnection connection) => _connection = connection;
    
    public abstract Task OpenAsync(CancellationToken cancellationToken);

    public void Close()
    {
        if (_connection != null)
            Connection.Close();
    }

    public abstract DbCommand CreateCommand();
    public abstract string DataSource { get; }

    public string Database
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Connection);
            var database = Connection.Database;
            return database;
        }
    }

    public abstract string ServerVersion { get; }
    
    public abstract string? ConnectionInformation { get; }

    public ConnectionState State
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Connection);
            return Connection.State;
        }
    }

    public abstract Task<int> GetTransactionCountAsync(CancellationToken cancellationToken);
    protected void InvokeInfoMessage(IReadOnlyCollection<InfoMessage> messages) => InfoMessage?.Invoke(messages);
    public event InfoMessageEventHandler? InfoMessage;
    public event EventHandler<DatabaseChangedEventArgs>? DatabaseChanged;

    public void Dispose() => Connection!.Dispose();

    public async ValueTask DisposeAsync() => await Connection!.DisposeAsync();
}