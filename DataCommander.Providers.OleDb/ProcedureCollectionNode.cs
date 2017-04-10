namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Windows.Forms;

    internal sealed class ProcedureCollectionNode : ITreeNode
    {
        private readonly SchemaNode schema;

        public ProcedureCollectionNode(SchemaNode schema)
        {
            this.schema = schema;
        }

        public string Name => "Procedures";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes;

            try
            {
                var restrictions = new object[] {this.schema.Catalog.Name, this.schema.Name};
                var dataTable = this.schema.Catalog.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures,
                    restrictions);
                var count = dataTable.Rows.Count;
                var procedureName = dataTable.Columns["PROCEDURE_NAME"];
                treeNodes = new ITreeNode[count];

                for (var i = 0; i < count; i++)
                {
                    var name = (string) dataTable.Rows[i][procedureName];
                    treeNodes[i] = new ProcedureNode(name);
                }
            }
            catch
            {
                treeNodes = new ITreeNode[] {new ProcedureNode(null)};
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
        {
        }
    }
}