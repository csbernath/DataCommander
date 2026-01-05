using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ParameterCollectionNode(DatabaseNode databaseNode, int objectId) : ITreeNode
{
    private readonly DatabaseNode _databaseNode = databaseNode;

    public string? Name => "Parameters";

    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        var commandText = @$"select
    t.name,
    t.user_type_id
from [{_databaseNode.Name}].sys.types t

select
    p.name,
    p.user_type_id,
    p.max_length,
    p.precision,
    p.scale,
    p.is_output,
    p.has_default_value
from [{_databaseNode.Name}].sys.parameters p
where
    p.object_id = {objectId}
order by
    p.parameter_id";
        ReadOnlySegmentLinkedList<ParameterNode>? parameterNodes = null;
        await Db.ExecuteReaderAsync(
            databaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            async (dataReader, _) =>
            {
                var sysTypes = await dataReader.ReadResultAsync(128, dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    var systemTypeId = dataRecord.GetInt32(1);
                    return new SysType(name, systemTypeId);
                }, cancellationToken);
                var sysTypesByUserTypeId = sysTypes.ToDictionary(t => t.UserTypeId);

                await dataReader.NextResultAsync(cancellationToken);

                parameterNodes = await dataReader.ReadResultAsync(128, dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    var userTypeId = dataRecord.GetInt32(1);
                    var sysType = sysTypesByUserTypeId[userTypeId];
                    var maxLength = dataRecord.GetInt16(2);
                    var isOutput = dataRecord.GetBoolean(5);
                    return new ParameterNode(name, sysType, maxLength, isOutput);
                }, cancellationToken);
            }, cancellationToken);
        return parameterNodes;
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    public Task<string?> GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}