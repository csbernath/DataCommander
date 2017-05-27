namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ProgrammabilityNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public ProgrammabilityNode(DatabaseNode database)
        {
            _database = database;
        }

        string ITreeNode.Name => "Programmability";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new StoredProcedureCollectionNode(_database, false),
                new FunctionCollectionNode(_database),
                new UserDefinedTableTypeCollectionNode(_database)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}