using System.Collections.Generic;
using DataCommander.Providers2;
using Foundation.Assertions;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SecurityNode : ITreeNode
{
    private readonly ServerNode _server;

    public SecurityNode(ServerNode serverNode)
    {
        Assert.IsNotNull(serverNode);
        _server = serverNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "Security";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new LoginCollectionNode(_server).ItemToArray();
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}