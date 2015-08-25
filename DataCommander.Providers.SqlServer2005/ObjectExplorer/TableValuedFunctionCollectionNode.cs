namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class TableValuedFunctionCollectionNode : ITreeNode
    {
        public TableValuedFunctionCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Table-valued Functions";
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
            string commandText = @"select
    s.name	as SchemaName,
	o.name	as Name,
	o.type
from [{0}].sys.schemas s (nolock)
join [{0}].sys.objects o (nolock)
on	s.schema_id = o.schema_id
where o.type in('IF','TF')
order by 1,2";
            commandText = string.Format(commandText, this.database.Name);
            string connectionString = this.database.Databases.Server.ConnectionString;

            return SqlClientFactory.Instance.ExecuteReader2(
                this.database.Databases.Server.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    string owner = dataRecord.GetString(0);
                    string name = dataRecord.GetString(1);
                    string xtype = dataRecord.GetString(2);
                    return new FunctionNode(this.database, owner, name, xtype);
                });
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

        readonly DatabaseNode database;
    }
}