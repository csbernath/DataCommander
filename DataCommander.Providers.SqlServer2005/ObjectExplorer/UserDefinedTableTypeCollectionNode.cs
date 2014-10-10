namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Data.SqlClient;
    using DataCommander.Foundation.Data;

    internal sealed class UserDefinedTableTypeCollectionNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public UserDefinedTableTypeCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        string ITreeNode.Name
        {
            get
            {
                return "User-Defined Table Types";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText = string.Format(@"select
     s.name
    ,t.name
from [{0}].sys.schemas s (nolock)
join [{0}].sys.table_types t (nolock)
    on s.schema_id = t.schema_id
order by 1,2", this.database.Name);

            var tableTypeNodes = new List<UserDefinedTableTypeNode>();
            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, null, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        string schema = dataReader.GetString(0);
                        string name = dataReader.GetString(1);
                        var tableTypeNode = new UserDefinedTableTypeNode(this.database, schema, name);
                        tableTypeNodes.Add(tableTypeNode);
                    }
                }
            }

            return tableTypeNodes;
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}