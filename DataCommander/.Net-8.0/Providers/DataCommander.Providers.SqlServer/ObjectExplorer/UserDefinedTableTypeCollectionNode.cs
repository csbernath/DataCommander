using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserDefinedTableTypeCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public UserDefinedTableTypeCollectionNode(DatabaseNode database) => _database = database;

    string ITreeNode.Name => "User-Defined Table Types";
    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await SqlClientFactory.Instance.ExecuteReaderAsync(
            _database.Databases.Server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            129,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        var commandText = $@"select
    s.name,
    t.name,
    type_table_object_id
from [{_database.Name}].sys.schemas s (nolock)
join [{_database.Name}].sys.table_types t (nolock)
    on s.schema_id = t.schema_id
order by 1,2";
        return commandText;
    }

    private UserDefinedTableTypeNode ReadRecord(IDataRecord dataRecord)
    {
        var schema = dataRecord.GetString(0);
        var name = dataRecord.GetString(1);
        var id = dataRecord.GetInt32(2);
        return new UserDefinedTableTypeNode(_database, id, schema, name);
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}