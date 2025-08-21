using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ConstraintCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    public string? Name => "Constraints";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var cb = new SqlCommandBuilder();

        var database = cb.QuoteIdentifier(databaseNode.Name);
        var commandText = $@"select cc.name
from {database}.sys.check_constraints cc (nolock)
where cc.parent_object_id = @object_id
union all
select dc.name
from {database}.sys.default_constraints dc (nolock)
where dc.parent_object_id = @object_id
order by 1";

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
                return new ConstraintNode(databaseNode, name);
            },
            cancellationToken);
    }

    public bool Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}