using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TableValuedFunctionCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Table-valued Functions";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        var commandText = @"select
    s.name	as SchemaName,
	o.name	as Name,
	o.type
from [{0}].sys.schemas s (nolock)
join [{0}].sys.objects o (nolock)
on	s.schema_id = o.schema_id
where o.type in('IF','TF')
order by 1,2";
        commandText = string.Format(commandText, database.Name);
        return commandText;
    }

    private FunctionNode ReadRecord(IDataRecord dataRecord)
    {
        var owner = dataRecord.GetString(0);
        var name = dataRecord.GetString(1);
        var xtype = dataRecord.GetString(2);
        return new FunctionNode(database, owner, name, xtype);
    }

    public bool Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}