using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class RoleCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public RoleCollectionNode(DatabaseNode database) => _database = database;

    public string Name => "Roles";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = $"select name from {_database.Name}..sysusers where issqlrole = 1 order by name";
        return await SqlClientFactory.Instance.ExecuteReaderAsync(
            _database.Databases.Server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private RoleNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new RoleNode(_database, name);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}