using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerCollectionNode : ITreeNode
{
    public LinkedServerCollectionNode(ServerNode serverNode)
    {
        ArgumentNullException.ThrowIfNull(serverNode);
        Server = serverNode;
    }

    public ServerNode Server { get; }

    string? ITreeNode.Name => "Linked Servers";

    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private static string CreateCommandText()
    {
        const string commandText = @"select  s.name
from    sys.servers s (nolock)
where   s.is_linked = 1
order by s.name";
        return commandText;
    }

    private LinkedServerNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new LinkedServerNode(this, name);
    }

    bool ITreeNode.Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}