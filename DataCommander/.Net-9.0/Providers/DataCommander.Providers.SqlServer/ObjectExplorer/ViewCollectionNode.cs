using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Views";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        List<ITreeNode> treeNodes =
        [
            new SystemViewCollectionNode(database)
        ];

        string databaseName = new SqlCommandBuilder().QuoteIdentifier(database.Name);
        string commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
        await Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            async (dataReader, _) =>
            {
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    string schema = dataReader.GetString(0);
                    string name = dataReader.GetString(1);
                    int id = dataReader.GetInt32(2);
                    treeNodes.Add(new ViewNode(database, id, schema, name));
                }
            },
            cancellationToken);
        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}