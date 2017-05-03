namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class UserDefinedTableTypeNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string schema;
        private readonly string name;

        public UserDefinedTableTypeNode(DatabaseNode database, string schema, string name)
        {
            this.database = database;
            this.schema = schema;
            this.name = name;
        }

        string ITreeNode.Name => $"{this.schema}.{this.name}";

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}