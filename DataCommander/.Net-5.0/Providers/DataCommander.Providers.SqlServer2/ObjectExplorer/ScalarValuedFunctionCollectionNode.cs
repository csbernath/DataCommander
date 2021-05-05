using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class ScalarValuedFunctionCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public ScalarValuedFunctionCollectionNode(DatabaseNode database) => _database = database;

        public string Name => "Scalar-valued Functions";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = @"select
    s.name	as SchemaName,
	o.name	as Name,
	o.type
from [{0}].sys.schemas s (nolock)
join [{0}].sys.objects o (nolock)
on	s.schema_id = o.schema_id
where o.type = 'FN'
order by 1,2";
            commandText = string.Format(commandText, _database.Name);

            return SqlClientFactory.Instance.ExecuteReader(_database.Databases.Server.ConnectionString, new ExecuteReaderRequest(commandText), 128,
                dataRecord =>
                {
                    var owner = dataRecord.GetString(0);
                    var name = dataRecord.GetString(1);
                    var xtype = dataRecord.GetString(2);
                    return new FunctionNode(_database, owner, name, xtype);
                });
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
    }
}