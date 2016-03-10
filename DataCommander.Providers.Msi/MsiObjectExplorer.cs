namespace DataCommander.Providers.Msi
{
    using System.Collections.Generic;
    using System.Data;

    internal sealed class MsiObjectExplorer : IObjectExplorer
	{
		private MsiConnection connection;

		#region IObjectExplorer Members

        void IObjectExplorer.SetConnection( string connectionString, IDbConnection connection )            
        {
            this.connection = (MsiConnection) connection;
        }

		IEnumerable<ITreeNode> IObjectExplorer.GetChildren( bool refresh )
		{
			return new ITreeNode[] { new MsiTableCollectionNode( this.connection ) };
		}

		bool IObjectExplorer.Sortable => false;

        #endregion
	}
}