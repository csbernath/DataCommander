using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private ConnectionStringAndCredential? _connectionStringAndCredential;

    public void SetConnection(ConnectionStringAndCredential connectionStringAndCredential) => _connectionStringAndCredential = connectionStringAndCredential;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new DatabaseCollectionNode(_connectionStringAndCredential!)
        ]);

    bool IObjectExplorer.Sortable => false;
}