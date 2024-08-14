using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SequenceNode : ITreeNode
{
    private readonly SequenceCollectionNode _sequenceCollectionNode;
    private readonly string _name;

    public SequenceNode(SequenceCollectionNode sequenceCollectionNode, string name)
    {
        _sequenceCollectionNode = sequenceCollectionNode;
        _name = name;
    }

    string ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return null;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}