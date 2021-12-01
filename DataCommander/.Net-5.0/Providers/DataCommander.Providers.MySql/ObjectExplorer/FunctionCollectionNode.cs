using Foundation.Data;
using Foundation.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class FunctionCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;

        public FunctionCollectionNode(DatabaseNode databaseNode)
        {
            _databaseNode = databaseNode;
        }

        string ITreeNode.Name => "Functions";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select r.ROUTINE_NAME
from information_schema.ROUTINES r
where
    r.ROUTINE_SCHEMA = {_databaseNode.Name.ToNullableVarChar()}
    and r.ROUTINE_TYPE = 'FUNCTION'
order by r.ROUTINE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                _databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                128,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new FunctionNode(_databaseNode, name);
                });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu => null;
        public ContextMenu GetContextMenu() => null;
    }
}