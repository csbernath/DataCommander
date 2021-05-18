﻿using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    sealed class DatabaseNode : ITreeNode
    {
        public DatabaseNode(SQLiteConnection connection, string name)
        {
            Connection = connection;
            Name = name;
        }

        public SQLiteConnection Connection { get; }

        #region ITreeNode Members
        public string Name { get; }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new TableCollectionNode(this)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}