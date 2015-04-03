namespace DataCommander.Providers.SqlServerCe
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Windows.Forms;
    using Application = DataCommander.Providers.Application;

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
            string commandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
            DataTable dataTable = this.connection.ExecuteDataTable(commandText);
            List<ITreeNode> nodes = new List<ITreeNode>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                string tableName = (string)dataRow["TABLE_NAME"];
                TableNode tableNode = new TableNode(tableName);
                nodes.Add(tableNode);
            }

            return nodes;
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

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var contextMenu = new ContextMenuStrip();
                var menuItem = new ToolStripMenuItem("Shrink database", null, this.ShrinkDatabase);
                contextMenu.Items.Add(menuItem);
                menuItem = new ToolStripMenuItem("Compact database", null, this.CompactDatabase);
                contextMenu.Items.Add(menuItem);
                return contextMenu;
            }
        }

        #endregion

        private void ShrinkDatabase(object sender, EventArgs e)
        {
            string connectionString = this.objectExplorer.ConnectionString;
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.Shrink();
        }

        private void CompactDatabase(object sender, EventArgs e)
        {
            var form = Application.Instance.MainForm.ActiveMdiChild;
            var cursor = form.Cursor;
            try
            {
                form.Cursor = Cursors.WaitCursor;
                this.connection.Close();
                string connectionString = this.objectExplorer.ConnectionString;
                SqlCeEngine engine = new SqlCeEngine(connectionString);
                engine.Compact(null);
                this.connection.Open();
            }
            finally
            {
                form.Cursor = cursor;
            }
        }
    }
}