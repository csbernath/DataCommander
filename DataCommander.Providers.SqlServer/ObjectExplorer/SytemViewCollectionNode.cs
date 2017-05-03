namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SystemViewCollectionNode : ITreeNode
    {
        public SystemViewCollectionNode( DatabaseNode database )
        {
            this.database = database;
        }

        public string Name => "System Views";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            var commandText = @"select  name,schema_id
from    {0}.sys.schemas

select  name,schema_id
from    {0}.sys.system_views";
            commandText = string.Format( commandText, this.database.Name );
            var treeNodes = new List<ViewNode>();
            var connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection( connectionString ))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition { CommandText = commandText }, CommandBehavior.Default))
                {
                    var schemas = new Dictionary<int, string>();

                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var id = dataRecord.GetInt32(1);
                        schemas.Add(id, name);
                    });

                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var schemaId = dataRecord.GetInt32(1);
                        var schemaName = schemas[schemaId];
                        var viewNode = new ViewNode(this.database, schemaName, name);
                        treeNodes.Add(viewNode);
                    });
                }
            }

            return treeNodes.OrderBy( node => node.Name ).Cast<ITreeNode>();
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        private readonly DatabaseNode database;
    }
}