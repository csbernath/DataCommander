namespace DataCommander.Providers.OleDb
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for CatalogsNode.
    /// </summary>
    sealed class TableNode : ITreeNode
    {
        private readonly SchemaNode schema;
        private readonly string name;

        public TableNode(SchemaNode schema, string name)
        {
            this.schema = schema;
            this.name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                string name = this.name;

                if (name == null)
                    name = "[No tables found]";

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
                string query;

                if (this.name != null)
                {
                    string name2;

                    if (this.name.IndexOf(' ') >= 0)
                    {
                        name2 = "[" + this.name + "]";
                    }
                    else
                    {
                        name2 = this.name;
                    }

                    query = "select * from " + name2;
                }
                else
                    query = null;

                return query;
            }
        }

        void Columns_Click(object sender, EventArgs e)
        {
            object[] restrictions = new object[] {this.schema.Catalog.Name, this.schema.Name, this.name };
            DataTable dataTable = this.schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);

            QueryForm queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.ShowDataSet(dataSet);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Columns", null, this.Columns_Click);
                contextMenu.Items.Add(menuItem);
                return contextMenu;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}