using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class ExtendedStoreProcedureNode : ITreeNode
    {
        private readonly DatabaseNode _database;
        private readonly string _schema;
        private readonly string _name;

        public ExtendedStoreProcedureNode(DatabaseNode database, string schema, string name)
        {
            _database = database;
            _schema = schema;
            _name = name;
        }

        string ITreeNode.Name => $"{_schema}.{_name}";
        bool ITreeNode.IsLeaf => true;
        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotSupportedException();
    }
}