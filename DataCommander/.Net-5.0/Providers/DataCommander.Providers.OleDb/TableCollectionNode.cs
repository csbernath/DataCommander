using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DataCommander.Providers.OleDb
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode(SchemaNode schema)
        {
            this.schema = schema;
        }

        public string Name => "Tables";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes;

            try
            {
                object[] restrictions = null;
                var catalog = schema.Catalog.Name;

                if (catalog != null)
                    restrictions = new object[] { catalog, schema.Name };

                DataTable dataTable = dataTable = schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
                var count = dataTable.Rows.Count;
                var nameColumn = dataTable.Columns["TABLE_NAME"];
                treeNodes = new ITreeNode[count];

                for (var i = 0; i < count; i++)
                {
                    var name = (string)dataTable.Rows[i][nameColumn];
                    treeNodes[i] = new TableNode(schema, name);
                }
            }
            catch
            {
                treeNodes = new ITreeNode[] { new TableNode(schema, null) };
            }

            return treeNodes;
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }

        private readonly SchemaNode schema;
    }
}