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
    public PostgresSqlNamespaceRepository? NamespaceRepository;
    public PostgresSqlTypeRepository? TypeRepository;
    
    public DatabaseCollectionNode DatabaseCollectionNode { get; } = databaseCollectionNode;

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        await using (var connection = CreateConnection())
        {
            await connection.OpenAsync(cancellationToken);
            var executor = connection.CreateCommandAsyncExecutor();
            await InitializeNamespaces(cancellationToken, executor);
            await InitializeTypes(executor, cancellationToken);
        }

        return
        [
            new SchemaCollectionNode(this)
        ];
    }

    private async Task InitializeTypes(IDbCommandAsyncExecutor executor, CancellationToken cancellationToken)
    {
        var commandText = @"select
    oid,
    typname,
    typnamespace,
    typcategory, 
    typelem
from pg_type";
        var types = await executor.ExecuteReaderAsync(
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var typname = dataRecord.GetString(1);
                var typnamespace = dataRecord.GetUInt32(2);
                NamespaceRepository!.TryGetByOid(typnamespace, out var @namespace);
                var typcategory = dataRecord.GetChar(3);
                var category = ToTypeCategory(typcategory);
                var typelem = dataRecord.GetUInt32(4);
                return new
                {
                    PostgresSqlType = new PostgresSqlType(oid, typname, @namespace, category, null),
                    typelem
                };
            },
            cancellationToken);

        TypeRepository = new PostgresSqlTypeRepository(
            types.Select(i => i.PostgresSqlType)
                .ToDictionary(t => t.Oid));
            
        foreach (var t in types.Where(i => i.typelem != 0))
        {
            TypeRepository.TryGetByOid(t.typelem, out var elementType);
            t.PostgresSqlType.ElementType = elementType;
        }
    }

    private async Task InitializeNamespaces(CancellationToken cancellationToken, IDbCommandAsyncExecutor executor)
    {
        var commandText = @"select
	oid,
	nspname
from pg_namespace";
        var namespaces = await executor.ExecuteReaderAsync(
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var oid = dataRecord.GetUInt32(0);
                var name = dataRecord.GetString(1);
                return new PostgresSqlNamespace(oid, name);
            },
            cancellationToken);

        NamespaceRepository = new PostgresSqlNamespaceRepository(
            namespaces.ToDictionary(n => n.Oid));
    }

    private static TypeCategory ToTypeCategory(char typcategory)
    {
        return typcategory switch
        {
            'A' => TypeCategory.ArrayTypes,
            'B' => TypeCategory.BooleanTypes,
            'C' => TypeCategory.CompositeTypes,
            'U' => TypeCategory.UserDefinedTypes,
            _ => TypeCategory.NotImplemented
        };
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    public NpgsqlConnection CreateConnection() => DatabaseCollectionNode.ObjectExplorer.CreateConnection(name);
}