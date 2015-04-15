namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode(SchemaNode schema)
        {
            this.schema = schema;
        }

        public string Name
        {
            get
            {
                return "Tables";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes;

            try
            {
                object [] restrictions = null;
                string catalog = this.schema.Catalog.Name;

                if (catalog != null)
                    restrictions = new object[] {catalog, this.schema.Name};

                DataTable dataTable =  dataTable = this.schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,restrictions);
                int count = dataTable.Rows.Count;
                DataColumn nameColumn = dataTable.Columns["TABLE_NAME"];        
                treeNodes = new ITreeNode[count];

                for (int i=0;i<count;i++)
                {
                    string name = (string)dataTable.Rows[i][nameColumn];
                    treeNodes[i] = new TableNode(this.schema,name);
                }
            }
            catch
            {
                treeNodes = new ITreeNode[] {new TableNode(this.schema,null)};
            }

            return treeNodes;
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

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
        }

        readonly SchemaNode schema;
    }
}