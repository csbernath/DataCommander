using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Providers.Connection;

namespace DataCommander.Providers.Wmi
{
    internal sealed class WmiProviderConnection : ConnectionBase
    {
        private readonly WmiConnection _wmiConnection;

        public WmiProviderConnection(string connectionString)
        {
            _wmiConnection = new WmiConnection(connectionString);
            Connection = _wmiConnection;
        }

        public override string ConnectionName { get; set; }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(_wmiConnection.Open);
        }

        public override IDbCommand CreateCommand()
        {
            return _wmiConnection.CreateCommand();
        }

        public override string Caption => $"WMI@{_wmiConnection.DataSource}";

        public override string DataSource => _wmiConnection.DataSource;

        public override string ServerVersion => null;

        public override int TransactionCount => 0;

        protected override void SetDatabase(string database)
        {
        }
    }
}