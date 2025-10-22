using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string? name) : ITreeNode
{
    public Dictionary<uint, PostgresSqlType>? PostgresSqlTypes;    
    
    public DatabaseCollectionNode DatabaseCollectionNode { get; } = databaseCollectionNode;

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = @"select oid,typname
from pg_type";
        var types = await Db.ExecuteReaderAsync(
            CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var typname = dataRecord.GetString(1);
                return new PostgresSqlType(oid, typname);
            },
            cancellationToken);
        PostgresSqlTypes = types.ToDictionary(t => t.Oid);

        return
        [
            new SchemaCollectionNode(this)
        ];
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    public NpgsqlConnection CreateConnection() => DatabaseCollectionNode.ObjectExplorer.CreateConnection(name);
}