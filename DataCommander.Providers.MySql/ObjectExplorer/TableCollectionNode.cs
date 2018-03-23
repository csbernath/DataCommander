using System.Collections.Generic;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
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
            var commandText = $@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{databaseNode.Name}'
    and TABLE_TYPE = 'BASE TABLE'
order by TABLE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new TableNode(databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}