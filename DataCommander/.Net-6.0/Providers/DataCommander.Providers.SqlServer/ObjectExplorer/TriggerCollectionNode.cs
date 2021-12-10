using System.Collections.Generic;
using DataCommander.Providers2;
using Microsoft.Data.SqlClient;
using Foundation.Data;
using Foundation.Data.SqlClient;
using SqlCommandExecutor = Foundation.Data.SqlClient2.SqlCommandExecutor;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerCollectionNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly int _id;

    public TriggerCollectionNode(DatabaseNode databaseNode, int id)
    {
        _databaseNode = databaseNode;
        _id = id;
    }

    public string Name => "Triggers";
    public bool IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var cb = new SqlCommandBuilder();
        var databaseName = cb.QuoteIdentifier(_databaseNode.Name);

        var commandText = $@"select
    name,
    object_id
from {databaseName}.sys.triggers
where object_id = {_id.ToSqlConstant()}
order by name";

        var connectionString = _databaseNode.Databases.Server.ConnectionString;
        var executor = new SqlCommandExecutor(connectionString);
        return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var name = dataRecord.GetString(0);
            var id = dataRecord.GetInt32(1);
            return new TriggerNode(_databaseNode, id, name);
        });
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}