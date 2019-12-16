using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class ProgrammabilityNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public ProgrammabilityNode(DatabaseNode database) => _database = database;

        string ITreeNode.Name => "Programmability";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var childNodes = new List<ITreeNode>();
            childNodes.Add(new StoredProcedureCollectionNode(_database, false));
            childNodes.Add(new FunctionCollectionNode(_database));

            if (_database.Name == "master")
                childNodes.Add(new ExtendedStoreProcedureCollectionNode(_database));

            childNodes.Add(new UserDefinedTableTypeCollectionNode(_database));

            return childNodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}