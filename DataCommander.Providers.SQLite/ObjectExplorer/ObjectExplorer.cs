namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private SQLiteConnection connection;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.connection = (SQLiteConnection)connection;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new DatabaseCollectionNode(this.connection)
            };
        }

        bool IObjectExplorer.Sortable => false;

        #endregion
    }
}