using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SchemaNode : ITreeNode
{
    public SchemaNode(SchemaCollectionNode schemaCollectionNode, string name)
    {
        SchemaCollectionNode = schemaCollectionNode;
        Name = name;
    }

    public SchemaCollectionNode SchemaCollectionNode { get; }

    public string Name { get; }

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new SequenceCollectionNode(this), 
            new TableCollectionNode(this),
            new ViewCollectionNode(this),
        };
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}