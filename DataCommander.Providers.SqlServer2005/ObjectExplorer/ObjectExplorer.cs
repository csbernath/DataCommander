namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Linq;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public string ConnectionString { get; private set; }

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            this.ConnectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new ServerNode(this.ConnectionString).ItemToArray();
        }

        bool IObjectExplorer.Sortable => false;
    }
}