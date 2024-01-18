using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
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

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var list = new List<ITreeNode>();
        list.Add(new SystemDatabaseCollectionNode(this));
        list.Add(new DatabaseSnapshotCollectionNode(this));

        var commandText = CreateCommandText();
        var databaseNodes = await Db.ExecuteReaderAsync(
            Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
        list.AddRange(databaseNodes);
        return list;
    }

    private static string CreateCommandText()
    {
        const string commandText = @"select
    d.name,
    d.state
from sys.databases d (nolock)
where
    source_database_id is null
    and name not in('master','model','msdb','tempdb')
order by d.name";
        return commandText;
    }

    private DatabaseNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        var state = dataRecord.GetByte(1);
        return new DatabaseNode(this, name, state);
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}