using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TableCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    private IReadOnlyCollection<FilterCriterion> _filterCriteria = [];
    
    public DatabaseNode DatabaseNode { get; } = databaseNode;
    public string? Name => "Tables";
    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [FilterableProperty.Name, FilterableProperty.Schema];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        string? schemaContains = null;
        string? nameContains = null;
        var filterCriterion = filterCriteria.FirstOrDefault(c => c.Property == FilterableProperty.Schema);
        if (filterCriterion != null)
            schemaContains = filterCriterion.Value;
        filterCriterion = filterCriteria.FirstOrDefault(c => c.Property == FilterableProperty.Name);
        if (filterCriterion != null)
            nameContains = filterCriterion.Value;

        var tableNodes = await GetTableNodes(schemaContains, nameContains, cancellationToken);
        var childNodes = new ITreeNode[] { new SystemTableCollectionNode(DatabaseNode) }
            .Concat(tableNodes);
        _filterCriteria = filterCriteria;        
        return childNodes;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => _filterCriteria;

    private async Task<ReadOnlySegmentLinkedList<TableNode>> GetTableNodes(string? schemaContains, string? nameContains, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText(schemaContains, nameContains);
        var tableNodes = await Db.ExecuteReaderAsync(
            DatabaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
        return tableNodes;
    }

    private string CreateCommandText(string? schemaContains, string? nameContains)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($@"select
    s.name,
    tbl.name,
    tbl.object_id,
    tbl.temporal_type
from [{DatabaseNode.Name}].sys.tables tbl (nolock)
join [{DatabaseNode.Name}].sys.schemas s (nolock)
    on tbl.schema_id = s.schema_id
where
(CAST(
 case 
    when tbl.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            [{DatabaseNode.Name}].sys.extended_properties 
        where 
            major_id = tbl.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)=0) and
    tbl.temporal_type in(0,2)");
        
        if (schemaContains != null)
        {
            var schemaLike = $"%{schemaContains}%".ToNVarChar();
            stringBuilder.Append($@" and
    s.name like {schemaLike}");
        }

        if (nameContains != null)
        {
            var nameLike = $"%{nameContains}%".ToNVarChar();
            stringBuilder.Append($@" and
    tbl.name like {nameLike}");
        }

        stringBuilder.Append(@"
order by 1,2");
        var commandText = stringBuilder.ToString();
        return commandText;
    }

    private TableNode ReadRecord(IDataRecord dataRecord)
    {
        var schema = dataRecord.GetString(0);
        var name = dataRecord.GetString(1);
        var objectId = dataRecord.GetInt32(2);
        var temporalType = (TemporalType)dataRecord.GetByte(3);
        return new TableNode(DatabaseNode, schema, name, objectId, temporalType);
    }

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}