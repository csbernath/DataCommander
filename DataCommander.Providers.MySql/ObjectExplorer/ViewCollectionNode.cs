using Foundation.Data;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System.Collections.Generic;
    using global::MySql.Data.MySqlClient;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public ViewCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name => "Views";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{databaseNode.Name}'
    and TABLE_TYPE = 'SYSTEM VIEW'
order by TABLE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new ViewNode(databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}