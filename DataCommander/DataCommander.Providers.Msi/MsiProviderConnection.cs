namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Connection;

    internal sealed class MsiProviderConnection : ConnectionBase
	{
		private string _connectionString;
		private readonly MsiConnection _msiConnection;

        public MsiProviderConnection( string connectionString )
		{
			_connectionString = connectionString;
			_msiConnection = new MsiConnection( connectionString );
			Connection = _msiConnection;
		}

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(_msiConnection.Open);
        }

        public override IDbCommand CreateCommand()
		{
			return _msiConnection.CreateCommand();
		}

		public override string ConnectionName { get; set; }

        public override string Caption => _msiConnection.Database.FilePath;

        public override string DataSource => _msiConnection.Database.FilePath;

        protected override void SetDatabase( string database )
		{
			throw new NotImplementedException();
		}

		public override string ServerVersion => null;

        public override int TransactionCount => 0;
	}
}