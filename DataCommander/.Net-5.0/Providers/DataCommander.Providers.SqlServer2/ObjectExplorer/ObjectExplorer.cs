using System.Collections.Generic;
using System.Data;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public string ConnectionString { get; private set; }
        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection) => ConnectionString = connectionString;
        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh) => new ServerNode(ConnectionString).ItemToArray();
        bool IObjectExplorer.Sortable => false;
    }
}