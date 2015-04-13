namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for SchemaNode.
    /// </summary>
    internal sealed class SchemaNode : ITreeNode
    {
		private SchemaCollectionNode schemasNode;
		private string name;

        public SchemaNode(
          SchemaCollectionNode schemasNode,
          string name)
        {
            this.schemasNode = schemasNode;
            this.name = name;
        }

        public string Name
        {
            get
            {
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
            ITreeNode[] treeNodes = new ITreeNode[]
        {
          new TableCollectionNode(this),
          new ViewCollectionNode(this),
		  new SequenceCollectionNode(this),
          new ProcedureCollectionNode(this),
		  new FunctionCollectionNode(this),
          new PackageCollectionNode(this),
          new SynonymCollectionNode(this)
        };

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

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
            schemasNode.SelectedSchema = name;
        }

        public SchemaCollectionNode SchemasNode
        {
            get
            {
                return schemasNode;
            }
        }
    }
}