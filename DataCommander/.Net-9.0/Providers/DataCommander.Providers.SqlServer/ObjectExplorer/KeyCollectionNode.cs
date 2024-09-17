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

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        string commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            databaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder();
        string commandText = @$"select name
from {sqlCommandBuilder.QuoteIdentifier(databaseNode.Name)}.sys.objects o
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

    private KeyNode ReadRecord(IDataRecord dataRecord)
    {
        string name = dataRecord.GetString(0);
        return new KeyNode(databaseNode, id, name);
    }

    public bool Sortable => false;
    public string Query => null;
    public ContextMenu? GetContextMenu() => null;
}