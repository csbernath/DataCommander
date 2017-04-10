namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class UserDefinedTableTypeCollectionNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public UserDefinedTableTypeCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        string ITreeNode.Name => "User-Defined Table Types";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = string.Format(@"select
     s.name
    ,t.name
from [{0}].sys.schemas s (nolock)
join [{0}].sys.table_types t (nolock)
    on s.schema_id = t.schema_id
order by 1,2", this.database.Name);

            var tableTypeNodes = new List<UserDefinedTableTypeNode>();
            var connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var reader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    reader.Read(dataRecord =>
                    {
                        var schema = dataRecord.GetString(0);
                        var name = dataRecord.GetString(1);
                        var tableTypeNode = new UserDefinedTableTypeNode(this.database, schema, name);
                        tableTypeNodes.Add(tableTypeNode);
                    });
                }
            }

            return tableTypeNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}