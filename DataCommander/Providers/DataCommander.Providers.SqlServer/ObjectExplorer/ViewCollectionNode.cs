using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Views";
    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async IAsyncEnumerable<ITreeNode> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        yield return new SystemViewCollectionNode(database);

        var databaseName = new SqlCommandBuilder().QuoteIdentifier(database.Name);
        var commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
        var items = Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            dataReader =>
            {
                var schema = dataReader.GetString(0);
                var name = dataReader.GetString(1);
                var id = dataReader.GetInt32(2);
                return new ViewNode(database, id, schema, name);
            },
            cancellationToken);

        await foreach (var item in items)
            yield return item;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}