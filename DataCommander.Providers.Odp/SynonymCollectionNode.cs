namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Foundation.Data;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class SynonymCollectionNode : ITreeNode
    {
        public SynonymCollectionNode(SchemaNode schema)
        {
            this.schema = schema;
        }

        public string Name
        {
            get
            {
                return "Synonyms";
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
            string commandText = @"select	s.SYNONYM_NAME
from	SYS.ALL_SYNONYMS s
where	s.OWNER	= '{0}'
order by s.SYNONYM_NAME";

            commandText = string.Format(commandText, schema.Name);
            var transactionScope = new DbTransactionScope(this.Schema.SchemasNode.Connection, null);
            DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText});
            int count = dataTable.Rows.Count;
            var treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][0];
                treeNodes[i] = new SynonymNode(schema, name);
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

        public SchemaNode Schema
        {
            get
            {
                return schema;
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