namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;

    internal sealed class SqlCeObjectExplorer : IObjectExplorer
    {
        private SqlCeConnection connection;

        public string ConnectionString { get; private set; }

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.ConnectionString = connectionString;
            this.connection = (SqlCeConnection)connection;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            yield return new TableCollectionNode(this, this.connection);
        }

        bool IObjectExplorer.Sortable => false;

        #endregion
    }
}