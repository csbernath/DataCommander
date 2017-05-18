namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Foundation.Data;
    using Foundation.Data.SqlClient;
    using Foundation.Threading.Tasks;

    internal sealed class LoginCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public LoginCollectionNode(ServerNode server)
        {
#if CONTRACTS_FULL
            Contract.Requires(server != null);
#endif
            this.server = server;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Logins";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"select  name
from sys.server_principals sp (nolock)
where   sp.type in('S','U','G')
order by name";
            var request = new ExecuteReaderRequest(commandText);

            var dbContext = new SqlConnectionStringDbContext(this.server.ConnectionString);
            var loginNodes = TaskSyncRunner.Run(() => dbContext.ExecuteReaderAsync(request, dataRecord => new LoginNode(dataRecord.GetString(0))));

            return loginNodes.Rows;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}