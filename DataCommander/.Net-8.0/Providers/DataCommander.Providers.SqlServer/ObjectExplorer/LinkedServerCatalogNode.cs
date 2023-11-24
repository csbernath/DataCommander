using System;
using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerCatalogNode : ITreeNode
{
    private readonly string _name;

    public LinkedServerCatalogNode(LinkedServerNode linkedServer, string name)
    {
        ArgumentNullException.ThrowIfNull(linkedServer);
        _name = name;
    }

    #region ITreeNode Members

    string ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        throw new NotImplementedException();
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}