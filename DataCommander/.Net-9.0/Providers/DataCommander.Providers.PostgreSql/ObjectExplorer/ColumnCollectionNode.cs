using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ColumnCollectionNode(TableNode tableNode) : ITreeNode
{
    private readonly TableNode _tableNode = tableNode;

    string? ITreeNode.Name => "Columns";

    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var schemaNode = _tableNode.TableCollectionNode.SchemaNode;

        return await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.ObjectExplorer.CreateConnection,
            new ExecuteReaderRequest($@"select
     c.column_name
    ,c.is_nullable
    ,c.data_type
    ,c.character_maximum_length
    ,c.numeric_precision
    ,c.numeric_scale
from information_schema.columns c
where
    c.table_schema = '{_tableNode.TableCollectionNode.SchemaNode.Name}'
    and c.table_name = '{_tableNode.Name}'
order by c.ordinal_position"),
            128,
            ReadRecord,
            cancellationToken);
    }

    private ColumnNode ReadRecord(IDataRecord dataRecord)
    {
        var columnName = dataRecord.GetString(0);
        var dataType = dataRecord.GetString(2);
        return new ColumnNode(this, columnName, dataType);
    }

    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}