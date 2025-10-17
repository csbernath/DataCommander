using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class TypeCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    public SchemaNode SchemaNode { get; } = schemaNode;

    string? ITreeNode.Name => "Types";

    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $@"select
	t.typname
from pg_type t
join pg_class c
	on t.typrelid = c.oid
where
    t.typnamespace = {schemaNode.Oid} and
	c.relkind = 'c'
order by
	t.typname";

        var typeNodes = await Db.ExecuteReaderAsync(
            schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new TypeNode(schemaNode, name);
            },
            cancellationToken);

        return typeNodes;
    }

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}