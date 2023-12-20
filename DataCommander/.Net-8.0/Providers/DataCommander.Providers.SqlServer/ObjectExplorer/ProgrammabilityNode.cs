using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ProgrammabilityNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public ProgrammabilityNode(DatabaseNode database) => _database = database;

    string ITreeNode.Name => "Programmability";
    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var childNodes = new List<ITreeNode>();
        childNodes.Add(new StoredProcedureCollectionNode(_database, false));
        childNodes.Add(new FunctionCollectionNode(_database));

        if (_database.Name == "master")
            childNodes.Add(new ExtendedStoreProcedureCollectionNode(_database));

        childNodes.Add(new UserDefinedTableTypeCollectionNode(_database));

        return Task.FromResult<IEnumerable<ITreeNode>>(childNodes);
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}