using System;
using System.Collections.Generic;
using DataCommander.Providers2;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LoginNode : ITreeNode
{
    private readonly string _name;

    public LoginNode(string name) => _name = name;

    string ITreeNode.Name => _name;
    bool ITreeNode.IsLeaf => true;
    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotImplementedException();
    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu() => null;
}