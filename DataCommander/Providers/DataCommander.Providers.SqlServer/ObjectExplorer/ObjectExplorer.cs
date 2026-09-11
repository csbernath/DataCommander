using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    private string? _connectionName;
    private ConnectionStringAndCredential? _connectionStringAndCredential;

    void IObjectExplorer.SetConnectionStringAndCredential(string? connectionName, ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionName = connectionName;
        _connectionStringAndCredential = connectionStringAndCredential;
    }

    Task<IEnumerable<ITreeNode>> IObjectExplorer.
        GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([new ServerNode(_connectionName, _connectionStringAndCredential!)]);

    bool IObjectExplorer.Sortable => false;
}