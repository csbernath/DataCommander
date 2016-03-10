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

        public SQLiteConnection Connection => this.connection;

        #region ITreeNode Members
        public string Name => this.name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new TableCollectionNode(this)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion

        readonly SQLiteConnection connection;
        readonly string name;
    }
}