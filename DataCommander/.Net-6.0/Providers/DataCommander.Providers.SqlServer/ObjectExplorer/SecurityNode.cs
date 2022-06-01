using System;
using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SecurityNode : ITreeNode
{
    private readonly ServerNode _server;

    public SecurityNode(ServerNode serverNode)
    {
        ArgumentNullException.ThrowIfNull(serverNode);
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

    public ContextMenu? GetContextMenu() => null;

    #endregion
}