using Foundation.Data.SqlClient;
using Foundation.Diagnostics;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using Query;

    internal sealed class StoredProcedureNode : ITreeNode
    {
        private readonly DatabaseNode _database;
        private readonly string _owner;
        private readonly string _name;

        public StoredProcedureNode(
            DatabaseNode database,
            string owner,
            string name )
        {
            _database = database;
            _owner = owner;
            _name = name;
        }

        public string Name => _owner + '.' + _name;

        public bool IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var query = "exec " + _name;
                return query;
            }
        }

        private void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();
            var connectionString = _database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments(connection, _database.Name, _owner, _name);
            }

            Clipboard.SetText(text);

            var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;

            queryForm.SetStatusbarPanelText(
                $"Copying stored prcoedure script to clipboard finished in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.",
                SystemColors.ControlText);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                var menuItemScriptObject = new ToolStripMenuItem( "Script Object", null, menuItemScriptObject_Click );
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add( menuItemScriptObject );
                return contextMenu;
            }
        }
    }
}