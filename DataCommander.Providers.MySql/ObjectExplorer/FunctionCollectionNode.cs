namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using Foundation.Data.SqlClient;
    using global::MySql.Data.MySqlClient;

    internal sealed class FunctionCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public FunctionCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

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
            string commandText =
                $@"select r.ROUTINE_NAME
from information_schema.ROUTINES r
where
    r.ROUTINE_SCHEMA = {this.databaseNode.Name.ToTSqlVarChar()
                    }
    and r.ROUTINE_TYPE = 'FUNCTION'
order by r.ROUTINE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                this.databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    string name = dataRecord.GetString(0);
                    return new FunctionNode(this.databaseNode, name);
                });
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

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}