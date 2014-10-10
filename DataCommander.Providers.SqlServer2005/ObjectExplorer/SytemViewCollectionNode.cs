namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class SystemViewCollectionNode : ITreeNode
    {
        public SystemViewCollectionNode( DatabaseNode database )
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "System Views";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            string commandText = @"select  name,schema_id
from    {0}.sys.schemas

select  name,schema_id
from    {0}.sys.system_views";
            commandText = string.Format( commandText, database.Name );
            List<ViewNode> treeNodes = new List<ViewNode>();
            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection( connectionString ))
            {
                connection.Open();
                using (var context = connection.ExecuteReader( null, commandText, CommandType.Text, 0, CommandBehavior.Default ))
                {
                    var dataReader = context.DataReader;
                    Dictionary<int, string> schemas = new Dictionary<int, string>();

                    while (dataReader.Read())
                    {
                        string name = dataReader.GetString( 0 );
                        int id = dataReader.GetInt32( 1 );
                        schemas.Add( id, name );
                    }

                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            string name = dataReader.GetString( 0 );
                            int schemaId = dataReader.GetInt32( 1 );
                            string schemaName = schemas[ schemaId ];
                            ViewNode viewNode = new ViewNode( this.database, schemaName, name );
                            treeNodes.Add( viewNode );
                        }
                    }
                }
            }

            return treeNodes.OrderBy( node => node.Name ).Cast<ITreeNode>();
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                return null;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        private DatabaseNode database;
    }
}