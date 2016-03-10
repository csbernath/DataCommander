namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System.Collections.Generic;

    internal sealed class DatabaseNode : ITreeNode
    {
        private readonly ObjectExplorer objectExplorer;
        private readonly string name;

        public DatabaseNode(ObjectExplorer objectExplorer, string name)
        {
            this.objectExplorer = objectExplorer;
            this.name = name;
        }

        public ObjectExplorer ObjectExplorer => this.objectExplorer;

        public string Name => this.name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new TableCollectionNode(this),
                new ViewCollectionNode(this), 
                new StoredProcedureCollectionNode(this), 
                new FunctionCollectionNode(this), 
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}