namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;
    using Foundation.Data;

    internal sealed class TriggerCollectionNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string schema;
        private readonly string objectName;

        public TriggerCollectionNode(DatabaseNode database, string owner, string objectName)
        {
            this.database = database;
            this.schema = owner;
            this.objectName = objectName;
        }

        public string Name
        {
            get
            {
                return "Triggers";
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
            var cb = new SqlCommandBuilder();
            var tableName = new DatabaseObjectMultipartName(null, this.database.Name, this.schema, this.objectName);
            //string commandText = "select o1.name,o2.name from {0}..sysobjects o1 left join {0}..sysobjects o2 on o1.parent_obj = o2.id where o1.type = 'TR' and o2.name = '{1}'";
            string commandText = string.Format(@"select  tr.name
from    {0}.sys.schemas s
join    {0}.sys.objects o
on      s.schema_id = o.schema_id
join    {0}.sys.triggers tr
on      o.object_id = tr.parent_id
where   s.name = {1}
        and o.name = {2}
order by 1",
                cb.QuoteIdentifier(this.database.Name),
                tableName.Schema.ToTSqlNVarChar(),
                tableName.Name.ToTSqlNVarChar());

            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
                        return new TriggerNode(this.database, this.schema, this.objectName, name);
                    });
                }
            }
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