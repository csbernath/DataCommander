using System.Collections.Generic;
using System.Data;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public string ConnectionString { get; private set; }

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            ConnectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            return new[] {new SchemaCollectionNode(this)};
        }

        bool IObjectExplorer.Sortable => false;
    }
}