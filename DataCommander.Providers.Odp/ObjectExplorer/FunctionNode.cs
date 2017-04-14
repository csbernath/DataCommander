namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class FunctionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;
        private readonly PackageNode packageNode;
        private readonly string name;

        public FunctionNode(
            SchemaNode schemaNode,
            PackageNode packageNode,
            string name)
        {
            this.schemaNode = schemaNode;
            this.packageNode = packageNode;
            this.name = name;
        }

        public string Name => this.name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var query = "EXEC " + schemaNode.Name + '.';

                if (this.packageNode != null)
                {
                    query += packageNode.Name + '.';
                }

                query += name;
                return query;
            }
        }

        private void ScriptObject_Click(object sender, EventArgs e)
        {
            var commandText =
                $@"select	text
from	all_source
where	owner = '{schemaNode.Name}'
	and name = '{name}'
	and type = 'FUNCTION'
order by line";
            var sb = new StringBuilder();
            string text;
            var transactionScope = new DbTransactionScope(this.schemaNode.SchemasNode.Connection, null);

            transactionScope.ExecuteReader(
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    text = dataRecord.GetString(0);
                    sb.Append(text);
                });

            text = sb.ToString();
            QueryForm.ShowText(text);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu;

                if (packageNode != null)
                {
                    contextMenu = null;
                }
                else
                {
                    var menuItem = new ToolStripMenuItem("Script Object", null, ScriptObject_Click);
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