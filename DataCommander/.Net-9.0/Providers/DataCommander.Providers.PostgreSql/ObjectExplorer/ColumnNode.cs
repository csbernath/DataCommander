using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ColumnNode(ColumnCollectionNode columnCollectionNode, string name, string dataType) : ITreeNode
{
    private readonly ColumnCollectionNode _columnCollectionNode = columnCollectionNode;
    private readonly string _name = name;
    private readonly string _dataType = dataType;

    string? ITreeNode.Name => $"{_name} {_dataType}";

    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}