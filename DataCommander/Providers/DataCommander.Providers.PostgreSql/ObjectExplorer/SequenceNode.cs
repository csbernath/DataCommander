using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SequenceNode(SequenceCollectionNode sequenceCollectionNode, string? name) : ITreeNode
{
    private readonly SequenceCollectionNode _sequenceCollectionNode = sequenceCollectionNode;
    private readonly string? _name = name;

    string? ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}