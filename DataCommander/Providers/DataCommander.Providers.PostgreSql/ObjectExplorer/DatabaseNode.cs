using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string name) : ITreeNode
{
    private PostgresSqlNamespaceRepository? _namespaceRepository;
    private PostgresSqlTypeRepository? _typeRepository;

    public PostgresSqlNamespaceRepository NamespaceRepository
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_namespaceRepository);
            return _namespaceRepository;
        }
    }

    public PostgresSqlTypeRepository TypeRepository
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_typeRepository);
            return _typeRepository;
        }
    }

    public DatabaseCollectionNode DatabaseCollectionNode { get; } = databaseCollectionNode;

    public string Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
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

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

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
                _namespaceRepository!.TryGetByOid(typnamespace, out var @namespace);
                var typcategory = dataRecord.GetChar(3);
                var category = ToTypeCategory(typcategory);
                var typelem = dataRecord.GetUInt32(4);
                return new
                {
                    PostgresSqlType = new PostgresSqlType(oid, typname, @namespace!, category, null),
                    typelem
                };
            },
            cancellationToken);

        _typeRepository = new PostgresSqlTypeRepository(
            types.Select(i => i.PostgresSqlType)
                .ToDictionary(t => t.Oid));
            
        foreach (var t in types.Where(i => i.typelem != 0))
        {
            _typeRepository.TryGetByOid(t.typelem, out var elementType);
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

        _namespaceRepository = new PostgresSqlNamespaceRepository(
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

    public NpgsqlConnection CreateConnection() => DatabaseCollectionNode.ObjectExplorer.CreateConnection(Name);
}