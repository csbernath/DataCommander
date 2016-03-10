namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        private readonly SQLiteConnection connection;

        public DatabaseCollectionNode(SQLiteConnection connection)
        {
            Contract.Requires(connection != null);
            this.connection = connection;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"PRAGMA database_list;";
            var children = new List<ITreeNode>();
            var transactionScope = new DbTransactionScope(this.connection, null);

            using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
            {
                dataReader.Read(dataRecord =>
                {
                    string name = dataRecord.GetString(1);
                    var databaseNode = new DatabaseNode(this.connection, name);
                    children.Add(databaseNode);
                });
            }

            return children;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}