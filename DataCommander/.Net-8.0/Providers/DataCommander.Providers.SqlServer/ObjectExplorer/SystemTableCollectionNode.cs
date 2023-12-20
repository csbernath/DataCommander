using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SystemTableCollectionNode : ITreeNode
{
    public SystemTableCollectionNode(DatabaseNode databaseNode)
    {
        DatabaseNode = databaseNode;
    }

    public DatabaseNode DatabaseNode { get; }

    string ITreeNode.Name => "System Tables";

    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        var tableNodes = await SqlClientFactory.Instance.ExecuteReaderAsync(
            DatabaseNode.Databases.Server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var schema = dataRecord.GetString(0);
                var name = dataRecord.GetString(1);
                var id = dataRecord.GetInt32(2);
                var temporalType = (TemporalType)dataRecord.GetByte(3);
                return new TableNode(DatabaseNode, schema, name, id, temporalType);
            },
            cancellationToken);
        return tableNodes;
    }

    private string CreateCommandText()
    {
        var commandText = $@"select
    s.name,
    tbl.name,
    tbl.object_id,
    tlb.temporal_type
from [{DatabaseNode.Name}].sys.tables tbl
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
             AS bit)=1)
order by 1,2";
        return commandText;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}