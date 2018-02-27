using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        private readonly SQLiteConnection _connection;

        public DatabaseCollectionNode(SQLiteConnection connection)
        {
            FoundationContract.Requires<ArgumentException>(connection != null);

            _connection = connection;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"PRAGMA database_list;";
            var executor = DbCommandExecutorFactory.Create(_connection);
            var databaseNodes = executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(1);
                return new DatabaseNode(_connection, name);
            });
            return databaseNodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}