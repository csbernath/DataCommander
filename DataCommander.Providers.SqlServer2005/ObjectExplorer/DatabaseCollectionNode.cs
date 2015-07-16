namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public DatabaseCollectionNode(ServerNode server)
        {
            Contract.Requires(server != null);
            this.server = server;
        }

        public ServerNode Server
        {
            get
            {
                return this.server;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Databases";
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
            var list = new List<ITreeNode>();
            list.Add(new SystemDatabaseCollectionNode(this));

            string connectionString = this.server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";

                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);

                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
                        var node = new DatabaseNode(this, name);
                        list.Add(node);
                    });
                }
            }

            return list;
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