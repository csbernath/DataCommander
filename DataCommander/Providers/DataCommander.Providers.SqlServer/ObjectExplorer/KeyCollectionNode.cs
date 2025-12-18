using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal class KeyCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    public string? Name => "Keys";
    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            databaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        var sqlCommandBuilder = new SqlCommandBuilder();
        var database = sqlCommandBuilder.QuoteIdentifier(databaseNode.Name);

        var commandText = @$"select name
from {database}.sys.objects o
where
    o.type in('PK','F','UQ') and
    o.parent_object_id = {id}
order by
    case o.type
        when 'PK' then 0
        when 'F' then 1
        when 'UQ' then 2
    end";

        return commandText;
    }

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    private static KeyNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new KeyNode(name);
    }
}