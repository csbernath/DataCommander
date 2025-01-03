using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ColumnNode : ITreeNode
{
    private readonly ColumnCollectionNode _columnCollectionNode;
    private readonly string _name;
    private readonly string _dataType;

    public ColumnNode(ColumnCollectionNode columnCollectionNode, string name, string dataType)
    {
        _columnCollectionNode = columnCollectionNode;
        _name = name;
        _dataType = dataType;
    }

    string ITreeNode.Name => $"{_name} {_dataType}";

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return null;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}