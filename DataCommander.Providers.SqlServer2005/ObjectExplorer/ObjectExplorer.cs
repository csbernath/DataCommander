namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Linq;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private string connectionString;

        public string ConnectionString => this.connectionString;

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.connectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new ServerNode(this.connectionString).ItemToArray();
        }

        bool IObjectExplorer.Sortable => false;
    }
}