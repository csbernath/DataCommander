namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class FunctionNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string owner;
        private readonly string name;
        private readonly string xtype;

        public FunctionNode(
            DatabaseNode database,
            string owner,
            string name,
            string xtype )
        {
            this.database = database;
            this.owner = owner;
            this.name = name;
            this.xtype = xtype;
        }

        public string Name
        {
            get
            {
                return $"{this.owner}.{this.name}";
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
                //string query = string.Format("select {0}.{1}.[{2}]()",database.Name,owner,name);
                string query;

                switch (this.xtype)
                {
                    case "FN": //Scalar function
                        query = $"select {this.database.Name}.{this.owner}.[{this.name}]()";
                        break;

                    case "TF": //Table function
                    case "IF": //Inlined table-function
                        query = $@"select	*
from	{this.database.Name}.{this.owner}.[{this.name}]()";
                        break;

                    default:
                        query = null;
                        break;
                }

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
                text = SqlDatabase.GetSysComments( connection, this.database.Name, this.owner, this.name );
            }
            QueryForm.ShowText( text );
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