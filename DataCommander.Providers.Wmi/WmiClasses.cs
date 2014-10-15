using System.Linq;

namespace DataCommander.Providers.Wmi
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Management;
    using System.Windows.Forms;

    internal sealed class WmiClasses : ITreeNode
    {
        private ManagementScope scope;

        public WmiClasses(ManagementScope scope)
        {
            this.scope = scope;
        }

        public string Name
        {
            get
            {
                return "Classes";
            }
        }
    
        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var manClass = new ManagementClass(scope.Path);
            ManagementObjectCollection objects = manClass.GetSubclasses();
            var list = new List<ITreeNode>();

            foreach (ManagementClass subClass in objects)
            {
                ITreeNode treeNode = new WmiClass(subClass);
                list.Add(treeNode);
            }

            return list.OrderBy(n => n.Name);
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
                return null;
            }
        }
    
        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }
    }
}