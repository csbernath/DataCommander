using Foundation.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class IndexCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    public string? Name => "Indexes";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var cb = new SqlCommandBuilder();

        var database = cb.QuoteIdentifier(databaseNode.Name);
        var commandText = $@"select
    i.name,
    i.index_id,
    i.type,
    i.is_unique
from {database}.sys.schemas s (nolock)
join {database}.sys.objects o (nolock)
    on s.schema_id = o.schema_id
join {database}.sys.indexes i (nolock)
    on o.object_id = i.object_id
where
    o.object_id = @object_id and
    i.type > 0
order by i.name";

        var parameters = new SqlParameterCollectionBuilder();
        parameters.Add("object_id", id);
        var request = new ExecuteReaderRequest(commandText, parameters.ToReadOnlyCollection());
        var executor = new SqlCommandExecutor(databaseNode.Databases.Server.CreateConnection);
        return await executor.ExecuteReaderAsync(
            request,
            128,
            dataRecord =>
            {
                var name = dataRecord.GetStringOrDefault(0);
                var indexId = dataRecord.GetInt32(1);
                var type = dataRecord.GetByte(2);
                var isUnique = dataRecord.GetBoolean(3);
                return new IndexNode(databaseNode, id, indexId, name, type, isUnique);
            },
            cancellationToken);
    }

    public bool Sortable => false;
    public string? Query => null;

    public ContextMenu? GetContextMenu() => null;
}