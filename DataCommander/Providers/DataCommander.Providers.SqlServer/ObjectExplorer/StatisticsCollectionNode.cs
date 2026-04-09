using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StatisticsCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    public string? Name => "Statistics";
    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        var cb = new SqlCommandBuilder();
        var database = cb.QuoteIdentifier(databaseNode.Name);        
        var commandText = $@"select s.name
from {database}.sys.stats s (nolock)
where s.object_id = @object_id
order by s.name";

        var parameters = new SqlParameterCollectionBuilder();
        parameters.Add("object_id", id);
        var request = new ExecuteReaderRequest(commandText, parameters.ToArray());
        var executor = new SqlCommandExecutor(databaseNode.Databases.Server.CreateConnection);
        return await executor.ExecuteReaderAsync(
            request,
            128,
            dataRecord =>
            {
                var name = dataRecord.GetStringOrDefault(0);
                return new StatisticsNode(databaseNode, name);
            },
            cancellationToken);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}