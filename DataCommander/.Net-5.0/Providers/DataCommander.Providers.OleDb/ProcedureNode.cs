using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.OleDb
{
    internal sealed class ProcedureNode : ITreeNode
    {
        private readonly string name;

        public ProcedureNode(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                var name = this.name;

                if (name == null)
                    name = "[No procedures found]";

                return name;
            }
        }

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                string query;

                if (name != null)
                    query = "exec " + name;
                else
                    query = null;

                return query;
            }
        }

        public ContextMenuStrip ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}