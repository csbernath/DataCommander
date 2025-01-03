using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private ConnectionStringAndCredential _connectionStringAndCredential;

    void IObjectExplorer.SetConnectionStringAndCredential(ConnectionStringAndCredential connectionStringAndCredential) =>
        _connectionStringAndCredential = connectionStringAndCredential;

    Task<IEnumerable<ITreeNode>> IObjectExplorer.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ServerNode(_connectionStringAndCredential).ItemToArray());

    bool IObjectExplorer.Sortable => false;
}