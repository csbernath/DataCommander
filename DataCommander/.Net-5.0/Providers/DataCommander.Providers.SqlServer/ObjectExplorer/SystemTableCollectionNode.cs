using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class SystemTableCollectionNode : ITreeNode
    {
        public SystemTableCollectionNode(DatabaseNode databaseNode)
        {
            DatabaseNode = databaseNode;
        }

        public DatabaseNode DatabaseNode { get; }

        string ITreeNode.Name => "System Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $@"select
    s.name,
    tbl.name,
    tbl.object_id
from [{DatabaseNode.Name}].sys.tables AS tbl
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
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
                {
                    var schema = dataReader.GetString(0);
                    var name = dataReader.GetString(1);
                    var id = dataReader.GetInt32(2);
                    return new TableNode(DatabaseNode, schema, name, id);
                });
            }
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}