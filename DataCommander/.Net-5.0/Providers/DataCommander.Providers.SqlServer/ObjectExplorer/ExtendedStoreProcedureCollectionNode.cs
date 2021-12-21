using System.Collections.Generic;
using Foundation.Data;
using Foundation.Data.SqlClient2;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ExtendedStoreProcedureCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public ExtendedStoreProcedureCollectionNode(DatabaseNode database) => _database = database;

    string ITreeNode.Name => "Extended Stored Procedures";
    bool ITreeNode.IsLeaf => false;
    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu GetContextMenu() => null;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var executor = new SqlCommandExecutor(_database.Databases.Server.ConnectionString);
        var commandText = @"select
    s.name,
    o.name
from sys.all_objects o
join sys.schemas s
    on o.schema_id = s.schema_id
where o.type = 'X'
order by 1,2";
        var request = new ExecuteReaderRequest(commandText);
        var childNodes = executor.ExecuteReader(request, 128, dataRecord =>
        {
            var schema = dataRecord.GetString(0);
            var name = dataRecord.GetString(1);
            return new ExtendedStoreProcedureNode(_database, schema, name);
        });
        return childNodes;
    }
}