using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ParameterNode(string name, SysType sysType) : ITreeNode
{
    public string? Name => name;

    public bool IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) => throw new System.NotImplementedException();

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    public Task<string?> GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}