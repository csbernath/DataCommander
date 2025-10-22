using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SchemaCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    public readonly DatabaseNode DatabaseNode = databaseNode;
    
    bool ITreeNode.IsLeaf => false;
    string ITreeNode.Name => "Schemas";

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    bool ITreeNode.Sortable => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        const string commandText = @"select oid,nspname
from pg_namespace
where nspname not in('information_schema','pg_catalog','pg_toast')
order by 2";
        var databaseNodes = await Db.ExecuteReaderAsync(
            databaseNode.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var name = dataRecord.GetString(1);
                return new SchemaNode(this, oid, name);
            },
            cancellationToken);
        return databaseNodes;
    }
}