using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class JobCollectionNode : ITreeNode
{
    public JobCollectionNode(ServerNode server)
    {
        ArgumentNullException.ThrowIfNull(server);
        Server = server;
    }

    public ServerNode Server { get; }

    string? ITreeNode.Name => "Jobs";
    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        CancellationToken cancellationToken)
    {
        const string commandText = @"select  j.name
from    msdb.dbo.sysjobs j (nolock)
order by j.name";
        return await Db.ExecuteReaderAsync(
            Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode)new JobNode(this, name);
            },
            cancellationToken);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}