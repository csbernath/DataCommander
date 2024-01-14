using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ObjectExplorer : IObjectExplorer
{
    public string ConnectionString { get; private set; }
    public SecureString? Password { get; private set; }
    
    void IObjectExplorer.SetConnection(string connectionString, SecureString? password, IDbConnection connection)
    {
        ConnectionString = connectionString;
        Password = password;
    }

    Task<IEnumerable<ITreeNode>> IObjectExplorer.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ServerNode(ConnectionString, Password).ItemToArray());

    bool IObjectExplorer.Sortable => false;
}