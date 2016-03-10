namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class FunctionCollectionNode : ITreeNode
    {
        public FunctionCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name => "Functions";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new TableValuedFunctionCollectionNode(this.database),
                new ScalarValuedFunctionCollectionNode(this.database)
            };
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        readonly DatabaseNode database;
    }
}