using Foundation.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;

        public TableCollectionNode(DatabaseNode databaseNode)
        {
            _databaseNode = databaseNode;
        }

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{_databaseNode.Name}'
    and TABLE_TYPE = 'BASE TABLE'
order by TABLE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                _databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                128,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new TableNode(_databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        public ContextMenu GetContextMenu() => null;
    }
}