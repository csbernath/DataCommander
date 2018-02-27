namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for CatalogsNode.
    /// </summary>
    class CatalogsNode : ITreeNode
    {
        public CatalogsNode(OleDbConnection connection)
        {
            this.connection = connection;
        }

        public string Name => "Catalogs";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes;

            try
            {
                var dataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs, null);
                var count = dataTable.Rows.Count;
                var nameColumn = dataTable.Columns["CATALOG_NAME"];
                treeNodes = new ITreeNode[count];

                for (var i = 0; i < count; i++)
                {
                    var name = (string)dataTable.Rows[i][nameColumn];
                    treeNodes[i] = new CatalogNode(connection, name);
                }
            }
            catch
            {
                treeNodes = new ITreeNode[1];
                treeNodes[0] = new CatalogNode(connection, null);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }

        readonly OleDbConnection connection;
    }
}