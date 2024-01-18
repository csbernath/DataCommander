using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SystemDatabaseCollectionNode : ITreeNode
{
    private readonly DatabaseCollectionNode _databaseCollectionNode;

    public SystemDatabaseCollectionNode(DatabaseCollectionNode databaseCollectionNode)
    {
        ArgumentNullException.ThrowIfNull(databaseCollectionNode);
        _databaseCollectionNode = databaseCollectionNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "System Databases";

    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            _databaseCollectionNode.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private DatabaseNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new DatabaseNode(_databaseCollectionNode, name, 0);
    }
    
    private static string CreateCommandText()
    {
        const string commandText = @"select d.name
from sys.databases d (nolock)
where name in('master','model','msdb','tempdb')
order by d.name";
        return commandText;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}