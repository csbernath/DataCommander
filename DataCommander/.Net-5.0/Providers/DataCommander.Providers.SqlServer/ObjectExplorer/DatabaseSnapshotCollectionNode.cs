using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class DatabaseSnapshotCollectionNode : ITreeNode
    {
        private readonly DatabaseCollectionNode _databaseCollectionNode;

        public DatabaseSnapshotCollectionNode(DatabaseCollectionNode databaseCollectionNode)
        {
            _databaseCollectionNode = databaseCollectionNode;
        }

        string ITreeNode.Name => "Database Snapshots";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var connectionString = _databaseCollectionNode.Server.ConnectionString;
            const string commandText = @"select name
from sys.databases d
where
	d.source_database_id is not null
order by 1";
            var executeReaderRequest = new ExecuteReaderRequest(commandText);
            return SqlClientFactory.Instance.ExecuteReader(connectionString, executeReaderRequest, 128, ReadDatabaseNode);
        }

        private DatabaseNode ReadDatabaseNode(IDataRecord dataRecord)
        {
            var name = dataRecord.GetString(0);
            return new DatabaseNode(_databaseCollectionNode, name, 0);
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
        public ContextMenu GetContextMenu() => null;
    }
}