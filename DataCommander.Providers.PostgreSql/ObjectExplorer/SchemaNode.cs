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

        public SchemaCollectionNode SchemaCollectionNode => this.schemaCollectionNode;

        public string Name => name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new SequenceCollectionNode(this), 
                new TableCollectionNode(this),
                new ViewCollectionNode(this),
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}