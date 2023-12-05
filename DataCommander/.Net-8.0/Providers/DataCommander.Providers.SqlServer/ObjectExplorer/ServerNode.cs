using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Core;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ServerNode : ITreeNode
{
    public ServerNode(string connectionString)
    {
        Assert.IsTrue(!connectionString.IsNullOrWhiteSpace());

        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    #region ITreeNode Members

    string ITreeNode.Name => ConnectionNameProvider.GetConnectionName(ConnectionString);

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var node = new DatabaseCollectionNode(this);
        var securityNode = new SecurityNode(this);
        var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
        var jobCollectionNode = new JobCollectionNode(this);
        return new ITreeNode[] { node, securityNode, serverObjectCollectionNode, jobCollectionNode };
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}