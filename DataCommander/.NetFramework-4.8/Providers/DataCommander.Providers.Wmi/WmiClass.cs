using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Windows.Forms;

namespace DataCommander.Providers.Wmi
{
    class WmiClass : ITreeNode
    {
        public WmiClass(ManagementClass manClass) => _manClass = manClass;

        public string Name => _manClass.ClassPath.ClassName;

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var objects = _manClass.GetSubclasses();
            var list = new ArrayList();

            foreach (ManagementClass subClass in objects)
            {
                ITreeNode treeNode = new WmiClass(subClass);
                list.Add(treeNode);
            }

            var array = new ITreeNode[list.Count];
            list.CopyTo(array);

            return array;
        }
    
        public bool Sortable => false;

        public string Query => $"select * from {Name}";

        public ContextMenuStrip ContextMenu => null;

        private readonly ManagementClass _manClass;
    }
}