﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Foundation.Assertions;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsProject : ITreeNode
    {
        private readonly Item _item;

        public TfsProject(Item item) => _item = item;

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                var name = VersionControlPath.GetFileName(_item.ServerItem);
                return name;
            }
        }

        bool ITreeNode.IsLeaf => false;

        internal static IEnumerable<ITreeNode> GetChildren( Item item )
		{
            Assert.IsNotNull(item);

			var itemSet = item.VersionControlServer.GetItems( item.ServerItem, RecursionType.OneLevel );
			var folders = new List<ITreeNode>();
			var files = new List<ITreeNode>();

			foreach (var current in itemSet.Items.Skip( 1 ))
			{
				switch (current.ItemType)
				{
					case ItemType.File:
						files.Add( new TfsFile( current ) );
						break;

					case ItemType.Folder:
						folders.Add( new TfsFolder( current ) );
						break;

					default:
						throw new NotImplementedException();
				}
			}

			return folders.Concat( files );
		}

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
			//ItemSet itemSet = this.item.VersionControlServer.GetItems(item.ServerItem, RecursionType.OneLevel);
			//var e = from current in itemSet.Items.Skip(1) select ToTreeNode(current);
			//return e;

			return GetChildren( _item );
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => _item.ServerItem;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion

        private static ITreeNode ToTreeNode(Item item)
        {
            ITreeNode treeNode;

            switch (item.ItemType)
            {
                case ItemType.File:
                    treeNode = new TfsFile(item);
                    break;

                case ItemType.Folder:
                    treeNode = new TfsFolder(item);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return treeNode;
        }

        //private void GetLatestVersion_Click(object sender, EventArgs e)
        //{
        //    GetLatestversion(this.item);
        //}

        //    public static void GetLatestversion(Item item)
        //    {
        //        Assert.IsNotNull(item, "item");
        //        //string tempPath = Path.GetTempPath();       
        //        //string path = Path.Combine(tempPath, string.Format("GetLatestVersion-{0}", DateTime.Now.ToString("yyyyMMdd HHmmss.fff")));
        //        string path = string.Format(@"C:\GLV {0}", DateTime.Now.ToString("yyyyMMdd HHmmss.fff"));
        //        DownloadFolder(item, path);
        //        QueryForm queryForm = (QueryForm)DataCommander.Providers.Application.Instance.MainForm.ActiveMdiChild;
        //        queryForm.AppendMessageText(string.Format("{0} downloaded to {1} successfully.", item.ServerItem, path));
        //    }

        //    private static void DownloadFolder(Item item, string localPath)
        //    {
        //        QueryForm queryForm = (QueryForm)DataCommander.Providers.Application.Instance.MainForm.ActiveMdiChild;
        //        queryForm.Invoke(new QueryForm.AppendText(queryForm.AppendMessageText), string.Format("Downloading {0}...", item.ServerItem));

        //        if (!Directory.Exists(localPath))
        //        {
        //            Directory.CreateDirectory(localPath);
        //        }

        //        using (new CurrentDirectoryChanger(localPath))
        //        {
        //            ItemSet itemSet = item.VersionControlServer.GetItems(item.ServerItem, RecursionType.OneLevel);

        //            foreach (Item childItem in itemSet.Items.Skip(1))
        //            {
        //                switch (childItem.ItemType)
        //                {
        //                    case ItemType.File:
        //                        string childItemName = VersionControlPath.GetFileName(childItem.ServerItem);
        //                        childItem.DownloadFile(childItemName);
        //                        DateTime checkingDate = childItem.CheckinDate;
        //                        FileInfo fileInfo = new FileInfo(childItemName);
        //                        fileInfo.LastWriteTime = checkingDate;
        //                        fileInfo.Attributes = FileAttributes.ReadOnly;
        //                        break;

        //                    case ItemType.Folder:
        //                        string childName = VersionControlPath.GetFileName(childItem.ServerItem);
        //                        DownloadFolder(childItem, childName);
        //                        break;

        //                    default:
        //                        throw new NotImplementedException();
        //                }
        //            }
        //        }

        //        Directory.SetLastWriteTime(localPath, item.CheckinDate);
        //    } 
    }
}