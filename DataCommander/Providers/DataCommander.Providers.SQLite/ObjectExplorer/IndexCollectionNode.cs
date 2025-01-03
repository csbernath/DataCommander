using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

internal sealed class IndexCollectionNode : ITreeNode
{
    private readonly TableNode _tableNode;

    public IndexCollectionNode(TableNode tableNode)
    {
        ArgumentNullException.ThrowIfNull(tableNode);
        _tableNode = tableNode;
    }

    string? ITreeNode.Name => "Indexes";
    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $"PRAGMA index_list({_tableNode.Name});";
        var list = await Db.ExecuteReaderAsync(
            () => ConnectionFactory.CreateConnection(_tableNode.DatabaseNode.DatabaseCollectionNode.ConnectionStringAndCredential),
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new IndexNode(_tableNode, name);
            },
            cancellationToken);
        return list!;
    }

    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => throw new NotImplementedException();
}