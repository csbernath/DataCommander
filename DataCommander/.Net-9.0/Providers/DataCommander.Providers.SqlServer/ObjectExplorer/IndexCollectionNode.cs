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
        SqlCommandBuilder cb = new SqlCommandBuilder();

        string commandText = string.Format(@"select
    i.name,
    i.index_id,
    i.type,
    i.is_unique
from {0}.sys.schemas s (nolock)
join {0}.sys.objects o (nolock)
    on s.schema_id = o.schema_id
join {0}.sys.indexes i (nolock)
    on o.object_id = i.object_id
where
    o.object_id = @object_id and
    i.type > 0
order by i.name",
            cb.QuoteIdentifier(databaseNode.Name));

        SqlParameterCollectionBuilder parameters = new SqlParameterCollectionBuilder();
        parameters.Add("object_id", id);
        ExecuteReaderRequest request = new ExecuteReaderRequest(commandText, parameters.ToReadOnlyCollection());
        SqlCommandExecutor executor = new SqlCommandExecutor(databaseNode.Databases.Server.CreateConnection);
        return await executor.ExecuteReaderAsync(
            request,
            128,
            dataRecord =>
            {
                string name = dataRecord.GetStringOrDefault(0);
                int indexId = dataRecord.GetInt32(1);
                byte type = dataRecord.GetByte(2);
                bool isUnique = dataRecord.GetBoolean(3);
                return new IndexNode(databaseNode, id, indexId, name, type, isUnique);
            },
            cancellationToken);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}