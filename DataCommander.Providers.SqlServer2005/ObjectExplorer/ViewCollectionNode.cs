namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        public ViewCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name => "Views";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = @"select
     s.name as SchemaName
    ,v.name as ViewName
from [{0}].sys.views v (nolock)
join [{0}].sys.schemas s (nolock)
    on v.schema_id = s.schema_id
order by s.name,v.name";
            commandText = string.Format(commandText, this.database.Name);
            var connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            var dataRows = dataTable.Rows;
            var count = dataRows.Count;
            var treeNodes = new List<ITreeNode>();
            treeNodes.Add(new SystemViewCollectionNode(this.database));

            for (var i = 0; i < count; i++)
            {
                var row = dataRows[i];
                var schema = (string)row[0];
                var name = (string)row[1];
                treeNodes.Add(new ViewNode(this.database, schema, name));
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        private readonly DatabaseNode database;
    }
}