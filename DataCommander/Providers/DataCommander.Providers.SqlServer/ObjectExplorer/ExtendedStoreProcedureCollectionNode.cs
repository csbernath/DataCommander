using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ExtendedStoreProcedureCollectionNode(DatabaseNode database) : ITreeNode
{
    string? ITreeNode.Name => "Extended Stored Procedures";
    bool ITreeNode.IsLeaf => false;

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    IAsyncEnumerable<ITreeNode> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var commandText = @"select
    s.name,
    o.name
from sys.all_objects o
join sys.schemas s
    on o.schema_id = s.schema_id
where o.type = 'X'
order by 1,2";
        var request = new ExecuteReaderRequest(commandText);
        return Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            request,
            dataRecord =>
            {
                var schema = dataRecord.GetString(0);
                var name = dataRecord.GetString(1);
                return new ExtendedStoreProcedureNode(database, schema, name);
            },
            cancellationToken);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];    
}