using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TriggerCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    public string? Name => "Triggers";
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
        SqlCommandBuilder cb = new SqlCommandBuilder();
        string databaseName = cb.QuoteIdentifier(databaseNode.Name);

        string commandText = $@"select
    name,
    object_id
from {databaseName}.sys.triggers
where object_id = {id.ToSqlConstant()}
order by name";
        return commandText;
    }

    private TriggerNode ReadRecord(IDataRecord dataRecord)
    {
        string name = dataRecord.GetString(0);
        int id = dataRecord.GetInt32(1);
        return new TriggerNode(databaseNode, id, name);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}