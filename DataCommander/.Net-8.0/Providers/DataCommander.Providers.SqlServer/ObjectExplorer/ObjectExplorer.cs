using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    public string ConnectionString { get; private set; }
    void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection) => ConnectionString = connectionString;

    Task<IEnumerable<ITreeNode>> IObjectExplorer.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ServerNode(ConnectionString).ItemToArray());

    bool IObjectExplorer.Sortable => false;
}