using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class SchemaNode(CatalogNode catalog, string name) : ITreeNode
{
    string? ITreeNode.Name
    {
        get
        {
            var name = Name;

            if (name == null)
                name = "[No schemas found]";

            return name;
        }
    }

    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async IAsyncEnumerable<ITreeNode> ITreeNode.GetChildren(
        IReadOnlyCollection<FilterCriterion> filterCriteria, 
        bool refresh, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new TableCollectionNode(this);
        yield return new ProcedureCollectionNode(this);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public CatalogNode Catalog { get; } = catalog;
    public string Name { get; } = name;

    public ContextMenu? GetContextMenu() => throw new NotImplementedException();
}