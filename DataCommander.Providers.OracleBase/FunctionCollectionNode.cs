namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    public sealed class FunctionCollectionNode : ITreeNode
	{
		private readonly SchemaNode schemaNode;

		public FunctionCollectionNode( SchemaNode schemaNode )
		{
			this.schemaNode = schemaNode;
		}

		#region ITreeNode Members

		string ITreeNode.Name
		{
			get
			{
				return "Functions";
			}
		}

		bool ITreeNode.IsLeaf
		{
			get
			{
				return false;
			}
		}

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText = string.Format(@"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{0}'
	and OBJECT_TYPE	= 'FUNCTION'
order by OBJECT_NAME", schemaNode.Name);
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);

            using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
            {
                return dataReader.Read(dataRecord =>
                {
                    string procedureName = dataRecord.GetString(0);
                    return new FunctionNode(schemaNode, null, procedureName);
                });
            }
        }

        bool ITreeNode.Sortable
		{
			get
			{
				return false;
			}
		}

		string ITreeNode.Query
		{
			get
			{
				return null;
			}
		}

		ContextMenuStrip ITreeNode.ContextMenu
		{
			get
			{
				return null;
			}
		}

		#endregion
	}
}