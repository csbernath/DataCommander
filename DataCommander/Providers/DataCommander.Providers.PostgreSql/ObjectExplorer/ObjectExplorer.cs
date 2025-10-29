using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private ConnectionStringAndCredential? _connectionStringAndCredential;
    
    public NpgsqlConnection CreateConnection() => ConnectionFactory.CreateConnection(_connectionStringAndCredential!);

    public NpgsqlConnection CreateConnection(string database)
    {
        ArgumentNullException.ThrowIfNull(_connectionStringAndCredential);
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionStringAndCredential.ConnectionString)
        {
            Database = database
        };
        var connectionString = connectionStringBuilder.ConnectionString;
        var connectionStringAndCredential = new ConnectionStringAndCredential(connectionString, _connectionStringAndCredential.Credential);
        return ConnectionFactory.CreateConnection(connectionStringAndCredential);
    }

    public void SetConnectionStringAndCredential(ConnectionStringAndCredential connectionStringAndCredential) =>
        _connectionStringAndCredential = connectionStringAndCredential;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new DatabaseCollectionNode(this)
        ]);

    bool IObjectExplorer.Sortable => false;
}