using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    public sealed class ObjectExplorer : IObjectExplorer
    {
        private IDbConnection _connection;
        private SchemaCollectionNode _schemasNode;

        public IDbConnection Connection => _connection;
        public IEnumerable<ITreeNode> GetChildren(bool refresh) => new ITreeNode[] {_schemasNode};
        public bool Sortable => false;
        public SchemaCollectionNode SchemasNode => _schemasNode;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            _connection = connection;
            _schemasNode = new SchemaCollectionNode(_connection);
        }

        #endregion
    }
}