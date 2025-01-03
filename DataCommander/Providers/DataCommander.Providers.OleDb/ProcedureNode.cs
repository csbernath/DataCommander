using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class ProcedureNode(string name) : ITreeNode
{
    public string Name
    {
        get
        {
            var name1 = name;

            if (name1 == null)
                name1 = "[No procedures found]";

            return name1;
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