using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class SchemaNode : ITreeNode
{
    private readonly SchemaCollectionNode _schemasNode;
    private readonly string _name;

    public SchemaNode(
        SchemaCollectionNode schemasNode,
        string name)
    {
        _schemasNode = schemasNode;
        _name = name;
    }

    public string Name => _name;

    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var treeNodes = new ITreeNode[]
        {
            new TableCollectionNode(this),
            new ViewCollectionNode(this),
            new SequenceCollectionNode(this),
            new ProcedureCollectionNode(this),
            new FunctionCollectionNode(this),
            new PackageCollectionNode(this),
            new SynonymCollectionNode(this)
        };

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    public SchemaCollectionNode SchemasNode => _schemasNode;
}