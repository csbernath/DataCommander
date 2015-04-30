namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class SchemaNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string name;

        public SchemaNode(DatabaseNode database, string name)
        {
            this.database = database;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public bool IsLeaf
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
    }
}