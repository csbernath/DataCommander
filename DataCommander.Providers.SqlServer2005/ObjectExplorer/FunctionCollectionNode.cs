namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class FunctionCollectionNode : ITreeNode
    {
        public FunctionCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Functions";
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
where o.type in('FN','IF','TF')
order by 1,2";
            commandText = string.Format(commandText, this.database.Name);
            var list = new List<ITreeNode>();
            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (IDataReader reader = connection.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        string owner = reader.GetString(0);
                        string name = reader.GetString(1);
                        string xtype = reader.GetString(2);
                        FunctionNode node = new FunctionNode(this.database, owner, name, xtype);
                        list.Add(node);
                    }
                }
            }

            ITreeNode[] treeNodes = new ITreeNode[list.Count];
            list.CopyTo(treeNodes);
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

        readonly DatabaseNode database;
    }
}