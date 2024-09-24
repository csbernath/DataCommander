using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.OleDb;

internal sealed class ObjectExplorer(IProvider provider) : IObjectExplorer
{
    private readonly IProvider _provider = provider;

    public bool Sortable => false;

    private OleDbConnection _connection;

    void IObjectExplorer.SetConnection(ConnectionStringAndCredential connectionStringAndCredential) => _connection = (OleDbConnection)_provider.CreateConnection(connectionStringAndCredential).Connection;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] treeNodes = new ITreeNode[]
        {
            new CatalogsNode(_connection)
        };
        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }
}