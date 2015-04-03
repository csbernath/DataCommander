namespace DataCommander.Providers.SqlServer2005
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

        string ITreeNode.Name
        {
            get
            {
                return string.Format("{0}.{1}", this.schema, this.name);
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
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