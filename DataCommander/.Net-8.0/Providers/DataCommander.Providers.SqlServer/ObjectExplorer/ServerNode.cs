using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Core;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ServerNode : ITreeNode
{
    public ServerNode(string connectionString, SecureString? password)
    {
        Assert.IsTrue(!connectionString.IsNullOrWhiteSpace());

        ConnectionString = connectionString;
        Password = password;
    }

    public string ConnectionString { get; }
    public SecureString? Password { get; }

    #region ITreeNode Members

    string ITreeNode.Name => ConnectionNameProvider.GetConnectionName(ConnectionString, Password);

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var node = new DatabaseCollectionNode(this);
        var securityNode = new SecurityNode(this);
        var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
        var jobCollectionNode = new JobCollectionNode(this);
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[] { node, securityNode, serverObjectCollectionNode, jobCollectionNode });
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}