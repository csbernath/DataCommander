namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Windows.Forms;

    sealed class DatabaseNode : ITreeNode
    {
        public DatabaseNode(SQLiteConnection connection, string name)
        {
            this.connection = connection;
            this.name = name;
        }

        public SQLiteConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        #region ITreeNode Members
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
                new TableCollectionNode(this)
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
        #endregion

        readonly SQLiteConnection connection;
        readonly string name;
    }
}