namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsFolder : ITreeNode
    {
        private readonly Item item;

        public TfsFolder(Item item)
        {
            this.item = item;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return TfsObjectExplorer.GetName(this.item);
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                bool isLeaf;

                switch (this.item.ItemType)
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
            //ItemSet itemSet = item.VersionControlServer.GetItems(this.item.ServerItem, RecursionType.OneLevel);
            //List<ITreeNode> folders = new List<ITreeNode>();
            //List<ITreeNode> files = new List<ITreeNode>();

            //foreach (Item current in itemSet.Items.Skip(1))
            //{
            //    switch (current.ItemType)
            //    {
            //        case ItemType.File:
            //            files.Add(new TfsFile(current));
            //            break;

            //        case ItemType.Folder:
            //            folders.Add(new TfsFolder(current));
            //            break;

            //        default:
            //            throw new NotImplementedException();
            //    }
            //}

            //return folders.Concat(files);

            return TfsProject.GetChildren(this.item);
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
                return this.item.ServerItem;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        private static TfsFolder ToTfsItem(Item item)
        {
            return new TfsFolder(item);
        }

        #endregion
    }
}