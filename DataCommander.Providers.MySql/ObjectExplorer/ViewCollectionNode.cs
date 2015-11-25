﻿namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using global::MySql.Data.MySqlClient;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public ViewCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name
        {
            get
            {
                return "Views";
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
                $@"select TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where
    TABLE_SCHEMA = '{this.databaseNode.Name
                    }'
    and TABLE_TYPE = 'SYSTEM VIEW'
order by TABLE_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                this.databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    string name = dataRecord.GetString(0);
                    return new ViewNode(this.databaseNode, name);
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