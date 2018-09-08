using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    /// <summary>
    /// Summary description for ObjectBrowser.
    /// </summary>
    public sealed class ObjectExplorer : IObjectExplorer
    {
        private IDbConnection connection;
        private SchemaCollectionNode schemasNode;

        public IDbConnection Connection => connection;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return new ITreeNode[] { schemasNode };
        }

        public bool Sortable => false;

        public SchemaCollectionNode SchemasNode => schemasNode;

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection( string connectionString, IDbConnection connection )
        {
            this.connection = connection;
            schemasNode = new SchemaCollectionNode( this.connection );
        }

        #endregion
    }
}