namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class MsiProviderConnection : ConnectionBase
	{
		private string connectionString;
		private MsiConnection msiConnection;

        public MsiProviderConnection( string connectionString )
		{
			this.connectionString = connectionString;
			this.msiConnection = new MsiConnection( connectionString );
			this.Connection = this.msiConnection;
		}

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(this.msiConnection.Open);
        }

        public override IDbCommand CreateCommand()
		{
			return this.msiConnection.CreateCommand();
		}

		public override string ConnectionName { get; set; }

        public override string Caption => this.msiConnection.Database.FilePath;

        public override string DataSource => this.msiConnection.Database.FilePath;

        protected override void SetDatabase( string database )
		{
			throw new NotImplementedException();
		}

		public override string ServerVersion => null;

        public override int TransactionCount => 0;
	}
}