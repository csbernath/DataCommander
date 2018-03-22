using Foundation.Data;

namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Threading;
    using System.Windows.Forms;

    internal sealed class TableCollectionNode : ITreeNode
    {
        private readonly SqlCeObjectExplorer objectExplorer;
        private readonly SqlCeConnection connection;

        public TableCollectionNode(SqlCeObjectExplorer objectExplorer, SqlCeConnection connection)
        {
            this.objectExplorer = objectExplorer;
            this.connection = connection;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var executor = connection.CreateCommandExecutor();
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

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var contextMenu = new ContextMenuStrip();
                var menuItem = new ToolStripMenuItem("Shrink database", null, ShrinkDatabase);
                contextMenu.Items.Add(menuItem);
                menuItem = new ToolStripMenuItem("Compact database", null, CompactDatabase);
                contextMenu.Items.Add(menuItem);
                return contextMenu;
            }
        }

        #endregion

        private void ShrinkDatabase(object sender, EventArgs e)
        {
            var connectionString = objectExplorer.ConnectionString;
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
                connection.Close();
                var connectionString = objectExplorer.ConnectionString;
                var engine = new SqlCeEngine(connectionString);
                engine.Compact(null);
                connection.Open();
            }
            finally
            {
                form.Cursor = cursor;
            }
        }
    }
}