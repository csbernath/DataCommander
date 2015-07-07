namespace DataCommander.Providers.MySql
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

        public ObjectExplorer ObjectExplorer
        {
            get
            {
                return this.objectExplorer;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

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

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}