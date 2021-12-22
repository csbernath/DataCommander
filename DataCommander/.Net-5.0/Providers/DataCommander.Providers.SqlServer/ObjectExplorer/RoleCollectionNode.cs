using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class RoleCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public RoleCollectionNode(DatabaseNode database) => _database = database;

    public string Name => "Roles";
    public bool IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        var commandText = $"select name from {_database.Name}..sysusers where issqlrole = 1 order by name";
        var connectionString = _database.Databases.Server.ConnectionString;
        var executor = new SqlCommandExecutor(connectionString);
        return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
        {
            var name = dataRecord.GetString(0);
            return new RoleNode(_database, name);
        });
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu GetContextMenu() => null;
}