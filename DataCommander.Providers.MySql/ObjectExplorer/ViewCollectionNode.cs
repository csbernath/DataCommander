namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using global::MySql.Data.MySqlClient;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public ViewCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name
        {
            get
            {
                return "Views";
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
            string commandText = string.Format(@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{0}'
    and TABLE_TYPE = 'SYSTEM VIEW'
order by TABLE_NAME", this.databaseNode.Name);

            using (var connection = new MySqlConnection(this.databaseNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, 0, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        string name = dataReader.GetString(0);
                        yield return new ViewNode(this.databaseNode, name);
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