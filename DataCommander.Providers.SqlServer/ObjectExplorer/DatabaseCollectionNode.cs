namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Foundation.Data;
    using Foundation.Data.SqlClient;
    using Foundation.Threading.Tasks;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        public DatabaseCollectionNode(ServerNode server)
        {
#if CONTRACTS_FULL
            Contract.Requires(server != null);
#endif
            this.Server = server;
        }

        public ServerNode Server { get; }

        #region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var list = new List<ITreeNode>();
            list.Add(new SystemDatabaseCollectionNode(this));
            var executor = new SqlCommandExecutor(this.Server.ConnectionString);

            const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";

            var executeReaderRequest = new ExecuteReaderRequest(commandText);

            var response = executor.ExecuteReader(executeReaderRequest, dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new DatabaseNode(this, name);
            });

            list.AddRange(response.Rows);

            return list;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}