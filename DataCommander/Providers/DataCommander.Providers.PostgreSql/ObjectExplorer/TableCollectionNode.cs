using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class TableCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    public SchemaNode SchemaNode { get; } = schemaNode;

    string? ITreeNode.Name => "Tables";

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $@"select
	oid,
	relname
from pg_class
where
	relnamespace = {SchemaNode.Oid} and
	relkind = 'r'
order by relname";

        var tableNodes = await Db.ExecuteReaderAsync(
            SchemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var name = dataRecord.GetString(1);
                return new TableNode(SchemaNode, oid, name);
            },
            cancellationToken);

        return tableNodes;
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}