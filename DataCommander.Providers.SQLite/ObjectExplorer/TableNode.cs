namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using DataCommander.Foundation.Data;

    internal sealed class TableNode : ITreeNode
    {
        private DatabaseNode databaseNode;
        private string name;

        public TableNode( DatabaseNode databaseNode, string name )
        {
            this.databaseNode = databaseNode;
            this.name = name;
        }

        public DatabaseNode Database
        {
            get
            {
                return this.databaseNode;
            }
        }

        #region ITreeNode Members

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            ITreeNode[] treeNodes = new ITreeNode[1];
            treeNodes[ 0 ] = new IndexCollectionNode( this );
            return treeNodes;
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
                return string.Format( "select\t*\r\nfrom\t{0}.{1}", databaseNode.Name, name );
            }
        }

        private static string GetScript(
            SQLiteConnection connection,
            string databaseName,
            string name )
        {
            string commandText = string.Format( @"
select  sql
from	{0}.sqlite_master
where	name	= '{1}'", databaseName, name );

            object scalar = connection.ExecuteScalar( null, commandText, CommandType.Text, 0 );
            string script = DataCommander.Foundation.Data.Database.GetValueOrDefault<string>( scalar );
            return script;
        }

        private void Script_Click( object sender, EventArgs e )
        {
            string script = GetScript( databaseNode.Connection, databaseNode.Name, name );
            QueryForm.ShowText( script );
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                System.Windows.Forms.ContextMenuStrip contextMenu = null;

                if (name != "sqlite_master")
                {
                    contextMenu = new System.Windows.Forms.ContextMenuStrip();
                    contextMenu.Items.Add( "Script", null, new EventHandler( Script_Click ) );
                }

                return contextMenu;
            }
        }

        #endregion
    }
}