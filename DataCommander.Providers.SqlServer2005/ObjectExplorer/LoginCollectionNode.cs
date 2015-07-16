namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class LoginCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public LoginCollectionNode(ServerNode server)
        {
            Contract.Requires(server != null);
            this.server = server;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Logins";
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
            const string commandText = @"select  name
from sys.server_principals sp (nolock)
where   sp.type in('S','U','G')
order by name";

            using (var connection = new SqlConnection(this.server.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord => new LoginNode(dataRecord.GetString(0)));
                }
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