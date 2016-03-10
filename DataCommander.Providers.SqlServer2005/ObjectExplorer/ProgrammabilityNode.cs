namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ProgrammabilityNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public ProgrammabilityNode(DatabaseNode database)
        {
            this.database = database;
        }

        string ITreeNode.Name => "Programmability";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new StoredProcedureCollectionNode(this.database, false),
                new FunctionCollectionNode(this.database),
                new UserDefinedTableTypeCollectionNode(this.database)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}