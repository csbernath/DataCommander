using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ParameterNode(string name, SysType sysType, short maxLength, bool isOutput) : ITreeNode
{
    public string? Name
    {
        get
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append(" (");

            stringBuilder.Append(sysType.Name);

            switch (sysType.UserTypeId)
            {
                case UserTypeId.VarChar:
                    stringBuilder.Append('(');
                    stringBuilder.Append(maxLength);
                    stringBuilder.Append(')');
                    break;
                
                case UserTypeId.NVarChar:
                    stringBuilder.Append('(');
                    stringBuilder.Append(maxLength / 2);
                    stringBuilder.Append(')');
                    break;
            }

            stringBuilder.Append(", ");
            stringBuilder.Append(isOutput ? "out" : "Input");
            stringBuilder.Append(')');

            return stringBuilder.ToString();
        }
    }

    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    public Task<string?> GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}