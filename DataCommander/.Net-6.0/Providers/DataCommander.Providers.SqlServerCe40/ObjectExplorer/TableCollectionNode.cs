using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly SqlCeObjectExplorer _objectExplorer;
        private readonly SqlCeConnection _connection;

        public TableCollectionNode(SqlCeObjectExplorer objectExplorer, SqlCeConnection connection)
        {
            _objectExplorer = objectExplorer;
            _connection = connection;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var executor = _connection.CreateCommandExecutor();
            var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var nodes = new List<ITreeNode>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var tableName = (string)dataRow["TABLE_NAME"];
                var tableNode = new TableNode(tableName);
                nodes.Add(tableNode);
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        public ContextMenu GetContextMenu()
        {
            var menuItem = new MenuItem("Shrink database", ShrinkDatabase, EmptyReadOnlyCollection<MenuItem>.Value);
            var compactDatabase = new MenuItem("Compact database", CompactDatabase, EmptyReadOnlyCollection<MenuItem>.Value);
            var items = new[] { menuItem, compactDatabase }.ToReadOnlyCollection();
            var contextMenu = new ContextMenu(items);
            return contextMenu;
        }

        #endregion

        private void ShrinkDatabase(object sender, EventArgs e)
        {
            var connectionString = _objectExplorer.ConnectionString;
            var engine = new SqlCeEngine(connectionString);
            engine.Shrink();
        }

        private void CompactDatabase(object sender, EventArgs e)
        {
            var form = DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            var cursor = form.Cursor;
            try
            {
                form.Cursor = Cursors.WaitCursor;
                _connection.Close();
                var connectionString = _objectExplorer.ConnectionString;
                var engine = new SqlCeEngine(connectionString);
                engine.Compact(null);
                _connection.Open();
            }
            finally
            {
                form.Cursor = cursor;
            }
        }
    }
}