using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ColumnCollectionNode(TableNode tableNode) : ITreeNode
{
    public readonly TableNode TableNode = tableNode;

    string? ITreeNode.Name => "Columns";

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        var schemaNode = TableNode.SchemaNode;

        var commandText = $@"select
    attname,
    atttypid, 
	attnotnull
from pg_attribute
where
    attrelid  = {TableNode.Oid} and
	attnum >= 1
order by attnum";

        return await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    private ColumnNode ReadRecord(IDataRecord dataRecord)
    {
        var columnName = dataRecord.GetString(0);
        var typeOid = dataRecord.GetUInt32(1);
        var notNull = dataRecord.GetBoolean(2);

        var typeRepository = TableNode.SchemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository;
        typeRepository.TryGetByOid(typeOid, out var type);

        return new ColumnNode(this, columnName, type!, notNull);
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}