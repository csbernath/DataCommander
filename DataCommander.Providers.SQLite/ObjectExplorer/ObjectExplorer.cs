namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data.SQLite;
    using DataCommander.Providers;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private SQLiteConnection connection;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, System.Data.IDbConnection connection)
        {
            this.connection = (SQLiteConnection)connection;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new DatabaseCollectionNode(connection)
            };
        }

        bool IObjectExplorer.Sortable
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}