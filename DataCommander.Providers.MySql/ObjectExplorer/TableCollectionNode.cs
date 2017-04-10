namespace DataCommander.Providers.MySql.ObjectExplorer
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

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText =
                $@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{this.databaseNode.Name
                    }'
    and TABLE_TYPE = 'BASE TABLE'
order by TABLE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                this.databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new TableNode(this.databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}