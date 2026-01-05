using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SequenceCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    string? ITreeNode.Name => "Sequences";

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        using var connection = schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection();
        connection.Open();
        var executor = connection.CreateCommandExecutor();
        var commandText = $@"select sequence_name
from information_schema.sequences
where sequence_schema = '{schemaNode.Name}'
order by sequence_name";
        return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
        {
            var name = dataReader.GetString(0);
            return new SequenceNode(this, name);
        }));
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}