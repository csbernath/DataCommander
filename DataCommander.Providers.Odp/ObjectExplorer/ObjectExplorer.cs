namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using Oracle.ManagedDataAccess.Client;

    /// <summary>
    /// Summary description for ObjectBrowser.
    /// </summary>
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private OracleConnection connection;
        private SchemaCollectionNode schemasNode;

        public OracleConnection OracleConnection
        {
            get
            {
                return connection;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return new ITreeNode[] { schemasNode };
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public SchemaCollectionNode SchemasNode
        {
            get
            {
                return schemasNode;
            }
        }

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection( string connectionString, IDbConnection connection )
        {
            this.connection = (OracleConnection) connection;
            schemasNode = new SchemaCollectionNode( this.connection );
        }

        #endregion
    }
}