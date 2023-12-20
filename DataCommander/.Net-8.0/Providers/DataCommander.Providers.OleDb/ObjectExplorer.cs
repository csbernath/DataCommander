using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ObjectExplorer : IObjectExplorer
{
    public bool Sortable => false;

    private OleDbConnection _connection;

    #region IObjectExplorer Members

    void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
    {
        _connection = (OleDbConnection)connection;
    }

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new ITreeNode[]
        {
            new CatalogsNode(_connection)
        };
        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    #endregion
}