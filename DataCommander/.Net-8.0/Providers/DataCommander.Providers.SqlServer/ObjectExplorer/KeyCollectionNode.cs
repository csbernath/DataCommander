using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await SqlClientFactory.Instance.ExecuteReaderAsync(
            _databaseNode.Databases.Server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        var sqlCommandBuilder = new SqlCommandBuilder();
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
        return commandText;
    }

    private KeyNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new KeyNode(_databaseNode, _id, name);
    }

    public bool Sortable => false;
    public string Query => null;
    public ContextMenu? GetContextMenu() => null;
}