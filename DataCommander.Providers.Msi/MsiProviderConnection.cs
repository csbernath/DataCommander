namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Linq;
	using System.Text;
	using DataCommander.Providers;
	using Microsoft.Deployment.WindowsInstaller;
	using DataCommander.Foundation.Diagnostics;

	internal sealed class MsiProviderConnection : ConnectionBase
	{
		private string connectionString;
		private MsiConnection msiConnection;
		private string connectionName;

		public MsiProviderConnection( string connectionString )
		{
			this.connectionString = connectionString;
			this.msiConnection = new MsiConnection( connectionString );
			this.Connection = this.msiConnection;
		}

		public override void Open()
		{
			this.msiConnection.Open();
		}

		public override System.Data.IDbCommand CreateCommand()
		{
			return this.msiConnection.CreateCommand();
		}

		public override string ConnectionName
		{
			get
			{
				return this.connectionName;
			}

			set
			{
				this.connectionName = value;
			}
		}

		public override string Caption
		{
			get
			{
				return this.msiConnection.Database.FilePath;
			}
		}

		public override string DataSource
		{
			get
			{
				return this.msiConnection.Database.FilePath;
			}
		}

		protected override void SetDatabase( string database )
		{
			throw new NotImplementedException();
		}

		public override string ServerVersion
		{
			get
			{
				return null;
			}
		}

        public override int TransactionCount
        {
            get
            {
                return 0;
            }
        }
	}
}