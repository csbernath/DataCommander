using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public UserCollectionNode(DatabaseNode database) => _database = database;

    public string Name => "Users";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $"select name from {_database.Name}..sysusers where islogin = 1 order by name";
        var connectionString = _database.Databases.Server.ConnectionString;
        DataTable dataTable;
        using (var connection = new SqlConnection(connectionString))
        {
            var executor = connection.CreateCommandExecutor();
            dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
        }

        var dataRows = dataTable.Rows;
        var count = dataRows.Count;
        var treeNodes = new ITreeNode[count];

        for (var i = 0; i < count; ++i)
        {
            var name = (string) dataRows[i][0];
            treeNodes[i] = new UserNode(_database, name);
        }

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}