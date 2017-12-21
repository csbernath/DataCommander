namespace DataCommander.Providers.Tfs
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsObjectExplorer : IObjectExplorer
    {
        private TfsConnection connection;
        private VersionControlServer versionControlServer;

        public static string GetName(Item item)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(item != null);
#endif
            var name = VersionControlPath.GetFileName(item.ServerItem);
            return name;
        }

#region IObjectExplorer Members

#region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            var tfsDbConnection = (TfsDbConnection)connection;
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
            var itemSet = this.versionControlServer.GetItems(VersionControlPath.RootFolder, RecursionType.OneLevel);
            var items = itemSet.Items;
            IEnumerable<Item> enumerable;

            if (items.Length > 0)
            {
                var firstItem = items[0];

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

        bool IObjectExplorer.Sortable => false;

        private static TfsProject ToTfsProject(Item item)
        {
            return new TfsProject(item);
        }

#endregion
    }
}