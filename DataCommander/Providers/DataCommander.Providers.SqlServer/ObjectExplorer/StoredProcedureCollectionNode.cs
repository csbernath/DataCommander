using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StoredProcedureCollectionNode(DatabaseNode database, bool isMsShipped) : ITreeNode
{
    private IReadOnlyCollection<FilterCriterion> _filterCriteria = [];
        
    public string? Name => isMsShipped
        ? "System Stored Procedures"
        : "Stored Procedures";

    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [FilterableProperty.Name, FilterableProperty.Schema];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        List<ITreeNode> treeNodes = [];
        if (!isMsShipped)
            treeNodes.Add(new StoredProcedureCollectionNode(database, true));

        string? schemaContains = null;
        string? nameContains = null;
        var filterCriterion = filterCriteria.FirstOrDefault(c => c.Property == FilterableProperty.Schema);
        if (filterCriterion != null)
            schemaContains = filterCriterion.Value;
        filterCriterion = filterCriteria.FirstOrDefault(c => c.Property == FilterableProperty.Name);
        if (filterCriterion != null)
            nameContains = filterCriterion.Value;

        var commandText = GetCommandText(schemaContains, nameContains);
        var rows = await Db.ExecuteReaderAsync(
            database.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var objectId = dataRecord.GetInt32(0);
                var owner = dataRecord.GetString(1);
                var name = dataRecord.GetString(2);
                return new StoredProcedureNode(database, objectId, owner, name);
            },
            cancellationToken);
        treeNodes.AddRange(rows);

        _filterCriteria = filterCriteria;

        return treeNodes;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => _filterCriteria;

    private string GetCommandText(string? schemaContains, string? nameContains)
    {
        var sb = new StringBuilder();
        sb.Append($@"select
    o.object_id as ObjectId,
    s.name as Owner,
    o.name as Name
from    [{database.Name}].sys.all_objects o (readpast)
join    [{database.Name}].sys.schemas s (readpast)
on      o.schema_id = s.schema_id
left join [{database.Name}].sys.extended_properties p
on      o.object_id = p.major_id and p.minor_id = 0 and p.class = 1 and p.name = 'microsoft_database_tools_support'
where
    o.type = 'P' and
    o.is_ms_shipped = {(isMsShipped
        ? 1
        : 0)} and
    p.major_id is null");

        if (schemaContains != null)
        {
            var schemaLike = $"%{schemaContains}%".ToNVarChar();
            sb.Append($@" and
    s.name like {schemaLike}");
        }

        if (nameContains != null)
        {
            var nameLike = $"%{nameContains}%".ToNVarChar();
            sb.Append($@" and
    o.name like {nameLike}");
        }

        sb.Append(@"
order by
    s.name,o.name");

        return sb.ToString();
    }

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}