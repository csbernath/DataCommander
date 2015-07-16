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
            commandText = string.Format( commandText, this.database.Name );
            var treeNodes = new List<ViewNode>();
            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection( connectionString ))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition { CommandText = commandText }, CommandBehavior.Default))
                {
                    var schemas = new Dictionary<int, string>();

                    dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
                        int id = dataRecord.GetInt32(1);
                        schemas.Add(id, name);
                    });

                    dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
                        int schemaId = dataRecord.GetInt32(1);
                        string schemaName = schemas[schemaId];
                        var viewNode = new ViewNode(this.database, schemaName, name);
                        treeNodes.Add(viewNode);
                    });
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

        private readonly DatabaseNode database;
    }
}