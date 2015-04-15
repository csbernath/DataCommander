namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for CatalogsNode.
    /// </summary>
    sealed class SchemaNode : ITreeNode
    {
        private readonly CatalogNode catalog;
        private readonly string name;

        public SchemaNode(CatalogNode catalog,string name)
        {
            this.catalog = catalog;
            this.name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                string name = this.name;
        
                if (name == null)
                    name = "[No schemas found]";

                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes = new ITreeNode[2];
            treeNodes[0] = new TableCollectionNode(this);
            treeNodes[1] = new ProcedureCollectionNode(this);

            return treeNodes;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }
    
        public string Query
        {
            get
            {
                return null;
            }
        }

        public CatalogNode Catalog
        {
            get
            {
                return this.catalog;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}