namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class ProcedureCollectionNode : ITreeNode
    {
        public ProcedureCollectionNode( SchemaNode schemaNode )
        {
            this.schemaNode = schemaNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Procedures";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{schemaNode.Name}'
	and OBJECT_TYPE	= 'PROCEDURE'
order by OBJECT_NAME";
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);

            using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
            {
                return dataReader.Read(dataRecord =>
                {
                    var procedureName = dataRecord.GetString(0);
                    return new ProcedureNode(schemaNode, null, procedureName);
                });
            }
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion

        private readonly SchemaNode schemaNode;
    }
}