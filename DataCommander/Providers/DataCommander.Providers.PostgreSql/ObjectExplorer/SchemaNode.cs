using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async IAsyncEnumerable<ITreeNode> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new FunctionCollectionNode(this);
        yield return new ProcedureCollectionNode(this);
        yield return new SequenceCollectionNode(this);
        yield return new TableCollectionNode(this);
        yield return new TypeCollectionNode(this);
        yield return new ViewCollectionNode(this);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}