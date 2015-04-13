using System.Text;

namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class IndexNode : ITreeNode
    {
        private readonly TableNode tableNode;
        private string schema;
        private readonly string name;
        private readonly byte type;
        private readonly bool isUnique;

        public IndexNode(TableNode tableNode, string name, byte type, bool isUnique)
        {
            this.tableNode = tableNode;
            this.name = name;
            this.type = type;
            this.isUnique = isUnique;
        }

        public string Name
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(this.name);
                sb.Append('(');

                sb.Append(this.isUnique
                    ? "Unique"
                    : "Non-Unique");

                sb.Append(',');

                string typeString;
                switch (this.type)
                {
                    case 1:
                        typeString = "Clustered";
                        break;

                    case 2:
                        typeString = "Non-Clustered";
                        break;

                    default:
                        typeString = "???";
                        break;
                }

                sb.Append(typeString);
                sb.Append(')');

                return sb.ToString();
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
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
                return null;
            }
        }

        //private void menuItemScriptObject_Click(object sender, EventArgs e)
        //{
        //    string connectionString = this.tableNode.
        //        .database.Databases.Server.ConnectionString;
        //    string text;
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        text = SqlDatabase.GetSysComments(connection, this.database.Name, "dbo", this.name);
        //    }
        //    QueryForm.ShowText(text);
        //}

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
                //ToolStripMenuItem menuItemScriptObject = new ToolStripMenuItem("Script Object", null, new EventHandler(this.menuItemScriptObject_Click));
                //ContextMenuStrip contextMenu = new ContextMenuStrip();
                //contextMenu.Items.Add(menuItemScriptObject);
                //return contextMenu;
            }
        }
    }
}