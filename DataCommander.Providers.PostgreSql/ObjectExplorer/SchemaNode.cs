namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class SchemaNode : ITreeNode
    {
        private readonly SchemaCollectionNode schemaCollectionNode;
        private readonly string name;

        public SchemaNode(SchemaCollectionNode schemaCollectionNode, string name)
        {
            this.schemaCollectionNode = schemaCollectionNode;
            this.name = name;
        }

        public SchemaCollectionNode SchemaCollectionNode
        {
            get
            {
                return this.schemaCollectionNode;
            }
        }

        public string Name
        {
            get
            {
                return name;
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
                new SequenceCollectionNode(this), 
                new TableCollectionNode(this),
                new ViewCollectionNode(this),
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

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}