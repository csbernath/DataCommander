using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsFolder : ITreeNode
    {
        private readonly Item _item;

        public TfsFolder(Item item) => _item = item;

        #region ITreeNode Members

        string ITreeNode.Name => TfsObjectExplorer.GetName(_item);

        bool ITreeNode.IsLeaf
        {
            get
            {
                bool isLeaf;

                switch (_item.ItemType)
                {
                    case ItemType.File:
                        isLeaf = true;
                        break;

                    case ItemType.Folder:
                        isLeaf = false;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return isLeaf;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return TfsProject.GetChildren(_item);
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => _item.ServerItem;
        ContextMenuStrip ITreeNode.ContextMenu => null;
        private static TfsFolder ToTfsItem(Item item) => new TfsFolder(item);

        #endregion
    }
}