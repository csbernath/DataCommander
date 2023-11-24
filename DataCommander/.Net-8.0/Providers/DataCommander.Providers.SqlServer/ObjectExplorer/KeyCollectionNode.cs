using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal class KeyCollectionNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly int _id;
    
    public KeyCollectionNode(DatabaseNode databaseNode, int id)
    {
        _databaseNode = databaseNode;
        _id = id;
    }

    public string Name => "Keys";
    public bool IsLeaf => false;

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var sqlCommandBuilder = new SqlCommandBuilder();
        var connectionString = _databaseNode.Databases.Server.ConnectionString;
        var commandText = @$"select name
from {sqlCommandBuilder.QuoteIdentifier(_databaseNode.Name)}.sys.objects o
where
    o.type in('PK','F','UQ') and
    o.parent_object_id = {_id}
order by
    case o.type
        when 'PK' then 0
        when 'F' then 1
        when 'UQ' then 2
    end";
        var executeReaderRequest = new ExecuteReaderRequest(commandText);

        var keyNodes = new List<KeyNode>();
        
        SqlClientFactory.Instance.ExecuteReader(connectionString, executeReaderRequest, dataReader =>
        {
            while (dataReader.Read())
            {
                var name = dataReader.GetString(0);
                var keyNode = new KeyNode(_databaseNode, _id, name);
                keyNodes.Add(keyNode);
            }
        });

        return keyNodes;
    }

    public bool Sortable => false;
    public string Query => null;
    public ContextMenu? GetContextMenu() => null;
}