using System.Collections.Generic;
using System.Data;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SystemDatabaseCollectionNode : ITreeNode
{
    private readonly DatabaseCollectionNode _databaseCollectionNode;

    public SystemDatabaseCollectionNode(DatabaseCollectionNode databaseCollectionNode)
    {
        Assert.IsNotNull(databaseCollectionNode);
        _databaseCollectionNode = databaseCollectionNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "System Databases";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var connectionString = _databaseCollectionNode.Server.ConnectionString;
        DataTable dataTable;
        using (var connection = new SqlConnection(connectionString))
        {
            var executor = connection.CreateCommandExecutor();
            const string commandText = @"select d.name
from sys.databases d (nolock)
where name in('master','model','msdb','tempdb')
order by d.name";
            dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
        }

        var list = new List<ITreeNode>();
        foreach (DataRow dataRow in dataTable.Rows)
        {
            var name = (string) dataRow[0];
            var node = new DatabaseNode(_databaseCollectionNode, name, 0);
            list.Add(node);
        }

        return list;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu() => null;

    #endregion
}