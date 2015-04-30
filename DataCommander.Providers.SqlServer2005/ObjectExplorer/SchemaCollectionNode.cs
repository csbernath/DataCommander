namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class SchemaCollectionNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public SchemaCollectionNode(DatabaseNode  database)
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Schemas";
            }
        }
    
        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText = @"select s.name
from {0}.sys.schemas s (nolock)
order by s.name";

            var sqlCommandBuilder = new SqlCommandBuilder();
            commandText = string.Format(commandText, sqlCommandBuilder.QuoteIdentifier(this.database.Name));
            string connectionString = this.database.Databases.Server.ConnectionString;
            var treeNodes = new List<ITreeNode>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, 0, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        string name = dataReader.GetString(0);
                        var treeNode = new SchemaNode(this.database, name);
                        treeNodes.Add(treeNode);
                    }
                }
            }

            return treeNodes;
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
    }
}