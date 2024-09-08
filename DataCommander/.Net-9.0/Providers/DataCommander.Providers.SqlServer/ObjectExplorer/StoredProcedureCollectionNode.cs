using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StoredProcedureCollectionNode(DatabaseNode database, bool isMsShipped) : ITreeNode
{
    public string? Name => isMsShipped
        ? "System Stored Procedures"
        : "Stored Procedures";

    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new List<ITreeNode>();
        if (!isMsShipped)
            treeNodes.Add(new StoredProcedureCollectionNode(database, true));

        var commandText = GetCommandText();
        var rows = await Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var owner = dataRecord.GetString(0);
                var name = dataRecord.GetString(1);
                return new StoredProcedureNode(database, owner, name);
            },
            cancellationToken);
        treeNodes.AddRange(rows);

        return treeNodes;
    }

    private string GetCommandText()
    {
        var commandText = string.Format(@"
select  s.name as Owner,
        o.name as Name        
from    [{0}].sys.all_objects o (readpast)
join    [{0}].sys.schemas s (readpast)
on      o.schema_id = s.schema_id
left join [{0}].sys.extended_properties p
on      o.object_id = p.major_id and p.minor_id = 0 and p.class = 1 and p.name = 'microsoft_database_tools_support'
where
    o.type = 'P'
    and o.is_ms_shipped = {1}
    and p.major_id is null
order by s.name,o.name", database.Name, isMsShipped
            ? 1
            : 0);
        return commandText;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}