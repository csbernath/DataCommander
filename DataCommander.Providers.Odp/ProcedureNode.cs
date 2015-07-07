namespace DataCommander.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;

    internal sealed class ProcedureNode : ITreeNode
    {
		private SchemaNode schemaNode;
		private PackageNode packageNode;
		private string name;

        public ProcedureNode(
            SchemaNode schemaNode,
            PackageNode packageNode,
            string name)
        {
            this.schemaNode = schemaNode;
            this.packageNode = packageNode;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
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
                string query = "EXEC " + schemaNode.Name + '.';

				if (packageNode != null)
				{
					query += packageNode.Name + '.';
				}

                query += name;
                return query;
            }
        }

        void ScriptObject_Click(object sender, EventArgs e)
        {
            string commandText = string.Format(@"select	text
from	all_source
where	owner = '{0}'
	and name = '{1}'
	and type = 'PROCEDURE'
order by line", schemaNode.Name, name);
            StringBuilder sb = new StringBuilder();
            string text;

            using (IDataReader dataReader = schemaNode.SchemasNode.Connection.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    text = dataReader.GetString(0);
                    sb.Append(text);
                }
            }

            text = sb.ToString();
            QueryForm.ShowText(text);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu;

                if (this.packageNode != null)
                {
                    contextMenu = null;
                }
                else
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem("Script Object", null, ScriptObject_Click);
                    contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add(menuItem);
                }

                return contextMenu;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}