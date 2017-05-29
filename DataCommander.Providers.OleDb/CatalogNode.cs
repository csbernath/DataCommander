namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Windows.Forms;

    class CatalogNode : ITreeNode
    {
        public CatalogNode(
            OleDbConnection connection,
            string          name)
        {
            this.Connection = connection;
            this.Name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                var name = this.Name;

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
                var restrictions = new object[] {this.Name};
                var dataTable = this.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Schemata,restrictions);
                var count = dataTable.Rows.Count;
                var nameColumn = dataTable.Columns["SCHEMA_NAME"];        
                treeNodes = new ITreeNode[count];

                for (var i=0;i<count;i++)
                {
                    var schemaName = (string)dataTable.Rows[i][nameColumn];
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

        public OleDbConnection Connection { get; }

        public string Name { get; }

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }
    }
}