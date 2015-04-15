namespace DataCommander.Providers.OleDb
{
    using System.Collections.Generic;
    using System.Windows.Forms;

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
                string name = this.name;

                if (name == null)
                    name = "[No procedures found]";

                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                string query;

                if (this.name != null)
                    query = "exec " + this.name;
                else
                    query = null;

                return query;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}