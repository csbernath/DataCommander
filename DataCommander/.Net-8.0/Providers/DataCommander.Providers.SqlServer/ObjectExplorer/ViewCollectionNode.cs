using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ViewCollectionNode(DatabaseNode database) : ITreeNode
{
    public string Name => "Views";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new List<ITreeNode>();
        treeNodes.Add(new SystemViewCollectionNode(database));

        var databaseName = new SqlCommandBuilder().QuoteIdentifier(database.Name);
        var commandText = $@"select
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
            async dataReader =>
            {
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    var schema = dataReader.GetString(0);
                    var name = dataReader.GetString(1);
                    var id = dataReader.GetInt32(2);
                    treeNodes.Add(new ViewNode(database, id, schema, name));
                }
            });
        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}