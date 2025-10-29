using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SchemaNode(SchemaCollectionNode schemaCollectionNode, uint oid, string? name) : ITreeNode
{
    public readonly uint Oid = oid;
    
    public SchemaCollectionNode SchemaCollectionNode { get; } = schemaCollectionNode;

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new FunctionCollectionNode(this),
            new ProcedureCollectionNode(this),
            new SequenceCollectionNode(this),
            new TableCollectionNode(this),
            new TypeCollectionNode(this),
            new ViewCollectionNode(this)
        ]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}