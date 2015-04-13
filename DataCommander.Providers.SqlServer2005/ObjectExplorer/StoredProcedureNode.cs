using System.Diagnostics;
using System.Drawing;
using DataCommander.Foundation.Diagnostics;

namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class StoredProcedureNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string owner;
        private readonly string name;

        public StoredProcedureNode(
            DatabaseNode database,
            string owner,
            string name )
        {
            this.database = database;
            this.owner = owner;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.owner + '.' + this.name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return null;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                string query = "exec " + this.name;
                return query;
            }
        }

        private void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();
            string connectionString = this.database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments(connection, this.database.Name, this.owner, this.name);
            }

            Clipboard.SetText(text);

            var queryForm = (QueryForm)Providers.DataCommanderApplication.Instance.MainForm.ActiveMdiChild;

            queryForm.SetStatusbarPanelText(
                string.Format(
                    "Copying stored prcoedure script to clipboard finished in {0} seconds.",
                    StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)),
                SystemColors.ControlText);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ToolStripMenuItem menuItemScriptObject = new ToolStripMenuItem( "Script Object", null, new EventHandler(this.menuItemScriptObject_Click ) );
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add( menuItemScriptObject );
                return contextMenu;
            }
        }
    }
}