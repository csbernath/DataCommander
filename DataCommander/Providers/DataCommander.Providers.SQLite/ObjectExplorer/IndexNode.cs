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

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    bool ITreeNode.Sortable => false;

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