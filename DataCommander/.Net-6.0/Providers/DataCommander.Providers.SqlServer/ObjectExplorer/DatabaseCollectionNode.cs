using System.Collections.Generic;
using DataCommander.Providers2;
using Microsoft.Data.SqlClient;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseCollectionNode : ITreeNode
{
    public DatabaseCollectionNode(ServerNode server)
    {
        Assert.IsTrue(server != null);

        Server = server;
    }

    public ServerNode Server { get; }

    #region ITreeNode Members

    string ITreeNode.Name => "Databases";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var list = new List<ITreeNode>();
        list.Add(new SystemDatabaseCollectionNode(this));
        list.Add(new DatabaseSnapshotCollectionNode(this));

        const string commandText = @"select
    d.name,
    d.state
from sys.databases d (nolock)
where
    source_database_id is null
    and name not in('master','model','msdb','tempdb')
order by d.name";

        var rows = SqlClientFactory.Instance.ExecuteReader(Server.ConnectionString, new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var name = dataRecord.GetString(0);
            var state = dataRecord.GetByte(1);
            return new DatabaseNode(this, name, state);
        });
        list.AddRange(rows);
        return list;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}