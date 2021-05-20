using Foundation.Linq;
using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public string ConnectionString { get; private set; }
        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection) => ConnectionString = connectionString;
        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh) => new ServerNode(ConnectionString).ItemToArray();
        bool IObjectExplorer.Sortable => false;
    }
}