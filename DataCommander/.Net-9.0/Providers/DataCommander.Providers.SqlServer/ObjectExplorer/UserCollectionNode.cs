using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Users";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        string commandText = $"select name from {database.Name}..sysusers where islogin = 1 order by name";
        DataTable dataTable;
        using (Microsoft.Data.SqlClient.SqlConnection connection = database.Databases.Server.CreateConnection())
        {
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText), CancellationToken.None);
        }

        DataRowCollection dataRows = dataTable.Rows;
        int count = dataRows.Count;
        ITreeNode[] treeNodes = new ITreeNode[count];

        for (int i = 0; i < count; ++i)
        {
            string name = (string) dataRows[i][0];
            treeNodes[i] = new UserNode(database, name);
        }

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}