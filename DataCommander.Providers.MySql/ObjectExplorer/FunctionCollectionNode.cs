namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using Foundation.Data.SqlClient;
    using global::MySql.Data.MySqlClient;

    internal sealed class FunctionCollectionNode : ITreeNode
    {
        private DatabaseNode databaseNode;

        public FunctionCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name
        {
            get
            {
                return "Functions";
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
            string commandText = string.Format(@"select r.ROUTINE_NAME
from information_schema.ROUTINES r
where
    r.ROUTINE_SCHEMA = {0}
    and r.ROUTINE_TYPE = 'FUNCTION'
order by r.ROUTINE_NAME", this.databaseNode.Name.ToTSqlVarChar());

            using (var connection = new MySqlConnection(this.databaseNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();

                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, 0, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        string name = dataReader.GetString(0);
                        yield return new FunctionNode(this.databaseNode, name);
                    }
                }
            }
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