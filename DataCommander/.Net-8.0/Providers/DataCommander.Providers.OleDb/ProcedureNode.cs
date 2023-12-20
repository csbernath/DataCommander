using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureNode : ITreeNode
{
    private readonly string name;

    public ProcedureNode(string name)
    {
        this.name = name;
    }

    public string Name
    {
        get
        {
            var name = this.name;

            if (name == null)
                name = "[No procedures found]";

            return name;
        }
    }

    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return null;
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            string query;

            if (name != null)
                query = "exec " + name;
            else
                query = null;

            return query;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}