namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using DataCommander.Providers;
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

		bool IObjectExplorer.Sortable
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}