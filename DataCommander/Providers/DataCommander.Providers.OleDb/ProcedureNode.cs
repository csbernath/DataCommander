using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureNode(string? name) : ITreeNode
{
    public string? Name
    {
        get
        {
            var name1 = name;

            if (name1 == null)
                name1 = "[No procedures found]";

            return name1;
        }
    }

    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        var query = name != null
            ? "exec " + name
            : null;
        return Task.FromResult(query);
    }

    public ContextMenu? GetContextMenu() => throw new NotImplementedException();
}