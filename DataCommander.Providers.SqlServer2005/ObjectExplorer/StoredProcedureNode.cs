namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class StoredProcedureNode : ITreeNode
    {
        private DatabaseNode database;
        private string owner;
        private string name;

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
                return owner + '.' + name;
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
                string query = "exec " + name;
                return query;
            }
        }

        void menuItemScriptObject_Click( object sender, EventArgs e )
        {
            string connectionString = this.database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection( connectionString ))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments( connection, database.Name, owner, name );
            }

            QueryForm.ShowText( text );
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ToolStripMenuItem menuItemScriptObject = new ToolStripMenuItem( "Script Object", null, new EventHandler( menuItemScriptObject_Click ) );
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add( menuItemScriptObject );
                return contextMenu;
            }
        }
    }
}