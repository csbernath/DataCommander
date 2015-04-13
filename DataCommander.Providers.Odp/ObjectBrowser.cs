namespace SqlUtil.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Oracle.DataAccess.Client;

    /// <summary>
    /// Summary description for ObjectBrowser.
    /// </summary>
    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public IDbConnection Connection
        {
            set
            {
                this.connection = (OracleConnection)value;
                schemasNode = new SchemaCollectionNode(connection);
            }
        }

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

        OracleConnection connection;
        SchemaCollectionNode schemasNode;
    }
}