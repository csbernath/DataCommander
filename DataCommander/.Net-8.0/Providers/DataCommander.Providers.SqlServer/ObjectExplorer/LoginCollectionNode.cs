using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LoginCollectionNode : ITreeNode
{
    private readonly ServerNode _server;

    public LoginCollectionNode(ServerNode server)
    {
        ArgumentNullException.ThrowIfNull(server);
        _server = server;
    }

    string ITreeNode.Name => "Logins";
    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await SqlClientFactory.Instance.ExecuteReaderAsync(
            _server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private static string CreateCommandText()
    {
        const string commandText = @"select name
from sys.server_principals sp (nolock)
where   sp.type in('S','U','G')
order by name";
        return commandText;
    }

    private static LoginNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new LoginNode(name);
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}