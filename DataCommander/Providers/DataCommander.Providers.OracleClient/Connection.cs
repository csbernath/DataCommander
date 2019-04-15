using System;
using System.Data;
using System.Data.OracleClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Providers.Connection;
using Foundation.Core;

namespace DataCommander.Providers.OracleClient
{
    internal sealed class Connection : ConnectionBase
    {
#pragma warning disable 618
        private readonly OracleConnection _oracleConnection;
#pragma warning restore 618
        private string _connectionName;

        public Connection(string connectionString)
        {
#pragma warning disable 618
            _oracleConnection = new OracleConnection(connectionString);
#pragma warning restore 618
            Connection = _oracleConnection;
            _oracleConnection.InfoMessage += OnInfoMessage;
        }

        public override Task OpenAsync(CancellationToken cancellationToken) => _oracleConnection.OpenAsync(cancellationToken);

        public override string Caption => null;

        private void OnInfoMessage(object sender, OracleInfoMessageEventArgs e)
        {
            var now = LocalTime.Default.Now;

            var sb = new StringBuilder();
            sb.Append(e.Message);
            sb.Append(Environment.NewLine);
            sb.Append("Code: ");
            sb.Append(e.Code);
            sb.Append(Environment.NewLine);
            sb.Append("Source: ");
            sb.Append(e.Source);

            InvokeInfoMessage(new[] {InfoMessageFactory.Create(InfoMessageSeverity.Information, null, sb.ToString())});
        }

        public override string DataSource => _oracleConnection.DataSource;
        public override string ServerVersion => _oracleConnection.ServerVersion;
        public override int TransactionCount => 0;
        public override IDbCommand CreateCommand() => _oracleConnection.CreateCommand();

        protected override void SetDatabase(string database)
        {
        }

        public override string ConnectionName
        {
            get => _connectionName;
            set => _connectionName = value;
        }
    }
}