namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Windows.Forms;

    class CatalogNode : ITreeNode
    {
        public CatalogNode(
            OleDbConnection connection,
            string          name)
        {
            this.connection = connection;
            this.name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                string name = this.name;

                if (name == null)
                    name = "[No catalogs found]";
                else if (name.Length == 0)
                    name = "[Catalog name is empty]";
          
                return name;
            }
        }
    
        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes;

            try
            {
                object [] restrictions = new object[] {this.name};
                DataTable dataTable = this.connection.GetOleDbSchemaTable(OleDbSchemaGuid.Schemata,restrictions);
                int count = dataTable.Rows.Count;
                DataColumn nameColumn = dataTable.Columns["SCHEMA_NAME"];        
                treeNodes = new ITreeNode[count];

                for (int i=0;i<count;i++)
                {
                    string schemaName = (string)dataTable.Rows[i][nameColumn];
                    treeNodes[i] = new SchemaNode(this,schemaName);
                }
            }
            catch
            {
                treeNodes = new ITreeNode[1];
                treeNodes[0] = new SchemaNode(this,null);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public OleDbConnection Connection => this.connection;

        public string Name => this.name;

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }

        readonly OleDbConnection connection;
        readonly string          name;
    }
}