namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Summary description for ObjectBrowser.
    /// </summary>
    public sealed class ObjectExplorer : IObjectExplorer
    {
        private IDbConnection connection;
        private SchemaCollectionNode schemasNode;

        public IDbConnection Connection
        {
            get
            {
                return this.connection;
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
            this.connection = connection;
            schemasNode = new SchemaCollectionNode( this.connection );
        }

        #endregion
    }
}