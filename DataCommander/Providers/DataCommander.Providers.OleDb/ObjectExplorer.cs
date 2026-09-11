using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.OleDb;

internal sealed class ObjectExplorer(IProvider provider) : IObjectExplorer
{
    private readonly IProvider _provider = provider;

    public bool Sortable => false;

    private ConnectionStringAndCredential _connectionStringAndCredential;

    void IObjectExplorer.SetConnectionStringAndCredential(string? connectionName, ConnectionStringAndCredential? connectionStringAndCredential) =>
        _connectionStringAndCredential = connectionStringAndCredential;

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new ITreeNode[]
        {
            new CatalogsNode(_connectionStringAndCredential)
        };
        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }
}