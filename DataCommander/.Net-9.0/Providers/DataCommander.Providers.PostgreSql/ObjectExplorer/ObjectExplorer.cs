using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Assertions;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private ConnectionStringAndCredential? _connectionStringAndCredential;
    
    public NpgsqlConnection CreateConnection() => ConnectionFactory.CreateConnection(_connectionStringAndCredential!);

    public void SetConnection(ConnectionStringAndCredential connectionStringAndCredential) =>
        _connectionStringAndCredential = connectionStringAndCredential;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([new SchemaCollectionNode(this)]);

    bool IObjectExplorer.Sortable => false;
}