using System.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        public DatabaseCollectionNode(ServerNode server)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(server != null);
#endif
            Server = server;
        }

        public ServerNode Server { get; }

        #region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var list = new List<ITreeNode>();
            list.Add(new SystemDatabaseCollectionNode(this));

            const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";

            var rows = SqlClientFactory.Instance.ExecuteReader(Server.ConnectionString, new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new DatabaseNode(this, name);
            });
            list.AddRange(rows);
            return list;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}