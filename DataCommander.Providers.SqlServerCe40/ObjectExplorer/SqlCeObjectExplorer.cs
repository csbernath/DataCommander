namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;

    internal sealed class SqlCeObjectExplorer : IObjectExplorer
    {
        private string connectionString;
        private SqlCeConnection connection;

        public string ConnectionString => this.connectionString;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.connectionString = connectionString;
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