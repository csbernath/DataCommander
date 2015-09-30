namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using global::MySql.Data.MySqlClient;

    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public TableCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name
        {
            get
            {
                return "Tables";
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
    and TABLE_TYPE = 'BASE TABLE'
order by TABLE_NAME", this.databaseNode.Name);

            return MySqlClientFactory.Instance.ExecuteReader(
                this.databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    string name = dataRecord.GetString(0);
                    return new TableNode(this.databaseNode, name);
                });
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