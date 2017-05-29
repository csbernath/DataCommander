namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for CatalogsNode.
    /// </summary>
    sealed class SchemaNode : ITreeNode
    {
        public SchemaNode(CatalogNode catalog,string name)
        {
            this.Catalog = catalog;
            this.Name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                var name = this.Name;
        
                if (name == null)
                    name = "[No schemas found]";

                return name;
            }
        }

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var treeNodes = new ITreeNode[2];
            treeNodes[0] = new TableCollectionNode(this);
            treeNodes[1] = new ProcedureCollectionNode(this);

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public CatalogNode Catalog { get; }

        public string Name { get; }

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }
    }
}