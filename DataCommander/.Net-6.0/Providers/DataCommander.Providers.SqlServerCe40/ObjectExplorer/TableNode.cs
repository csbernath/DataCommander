using System;
using System.Collections.Generic;

namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer;

internal sealed class TableNode : ITreeNode
{
    readonly string _name;

    public TableNode(string name) => _name = name;

    #region ITreeNode Members

    string ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        throw new NotImplementedException();
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new NotImplementedException();
    }

    #endregion
}