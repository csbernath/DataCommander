namespace DataCommander.Providers.Tfs
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using DataCommander.Providers;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsObjectExplorer : IObjectExplorer
    {
        private TfsConnection connection;
        private VersionControlServer versionControlServer;

        public static string GetName(Item item)
        {
            Contract.Requires(item != null);
            string name = VersionControlPath.GetFileName(item.ServerItem);
            return name;
        }

        #region IObjectExplorer Members

        #region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, System.Data.IDbConnection connection)
        {
            TfsDbConnection tfsDbConnection = (TfsDbConnection)connection;
            this.connection = tfsDbConnection.Connection;
            var tfsTeamProjectCollection = this.connection.TfsTeamProjectCollection;
            this.versionControlServer = (VersionControlServer)tfsTeamProjectCollection.GetService(typeof(VersionControlServer));
        }

        #endregion

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            // TFS 2005 and 2008 behavior is different:
            //  - TFS 2005 does not return the rootfolder
            //  - TFS 2008 returns the rootfolder
            ItemSet itemSet = this.versionControlServer.GetItems(VersionControlPath.RootFolder, RecursionType.OneLevel);
            Item[] items = itemSet.Items;
            IEnumerable<Item> enumerable;

            if (items.Length > 0)
            {
                Item firstItem = items[0];

                if (firstItem.ServerItem == VersionControlPath.RootFolder)
                {
                    enumerable = items.Skip(1);
                }
                else
                {
                    enumerable = items;
                }
            }
            else
            {
                enumerable = items.Skip(1);
            }

            var f = from item in enumerable select (ITreeNode)ToTfsProject(item);
            return f;
        }

        bool IObjectExplorer.Sortable
        {
            get
            {
                return false;
            }
        }

        private static TfsProject ToTfsProject(Item item)
        {
            return new TfsProject(item);
        }

        #endregion
    }
}