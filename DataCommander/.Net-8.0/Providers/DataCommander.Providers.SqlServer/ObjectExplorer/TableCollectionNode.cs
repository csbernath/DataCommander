using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TableCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    public DatabaseNode DatabaseNode { get; } = databaseNode;
    public string Name => "Tables";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var tableNodes = await GetTableNodes(cancellationToken);
        var childNodes = new ITreeNode[] { new SystemTableCollectionNode(DatabaseNode) }
            .Concat(tableNodes);
        return childNodes;
    }

    private async Task<ReadOnlySegmentLinkedList<TableNode>> GetTableNodes(CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        var connectionString = DatabaseNode.Databases.Server.ConnectionString;
        var tableNodes = await SqlClientFactory.Instance.ExecuteReaderAsync(
            connectionString,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
        return tableNodes;
    }

    private string CreateCommandText()
    {
        var commandText = $@"select
    s.name,
    tbl.name,
    tbl.object_id,
    tbl.temporal_type
from [{DatabaseNode.Name}].sys.tables tbl (nolock)
join [{DatabaseNode.Name}].sys.schemas s (nolock)
    on tbl.schema_id = s.schema_id
where
(CAST(
 case 
    when tbl.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            [{DatabaseNode.Name}].sys.extended_properties 
        where 
            major_id = tbl.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)=0) and
    tbl.temporal_type in(0,2)
order by 1,2";
        return commandText;
    }

    private TableNode ReadRecord(IDataRecord dataRecord)
    {
        var schema = dataRecord.GetString(0);
        var name = dataRecord.GetString(1);
        var objectId = dataRecord.GetInt32(2);
        var temporalType = (TemporalType)dataRecord.GetByte(3);
        return new TableNode(DatabaseNode, schema, name, objectId, temporalType);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}