using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class IndexNode(TableNode tableNode, string? name) : ITreeNode
{
    #region ITreeNode Members

    string? ITreeNode.Name => name;

    bool ITreeNode.IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    async Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken)
    {
        var commandText = $@"select sql
from main.sqlite_master
where
    type = 'index'
    and name = '{name}'";
        var scalar = await Db.ExecuteScalarAsync(
            () => ConnectionFactory.CreateConnection(tableNode.DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
            new CreateCommandRequest(commandText),
            cancellationToken);
        var sql = (string?)scalar;
        return sql;
    }

    public ContextMenu? GetContextMenu() => throw new NotImplementedException();

    #endregion
}