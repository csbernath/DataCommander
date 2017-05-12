namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class UserDefinedTableTypeNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly int id;
        private readonly string schema;
        private readonly string name;

        public UserDefinedTableTypeNode(DatabaseNode database, int id, string schema, string name)
        {
            this.database = database;
            this.id = id;
            this.schema = schema;
            this.name = name;
        }

        string ITreeNode.Name => $"{this.schema}.{this.name}";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(this.database, id)
            };
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}