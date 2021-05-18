using DataCommander.Providers.Connection;
using DataCommander.Providers2.Connection;
using Foundation.Core;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.OleDb
{
    internal sealed class Connection : ConnectionBase
    {
        private readonly OleDbConnection oledbConnection;

        public Connection(string connectionString)
        {
            oledbConnection = new OleDbConnection(connectionString);
            Connection = oledbConnection;

            oledbConnection.InfoMessage += OnInfoMessage;
        }

        public override string ConnectionName { get; set; }
        public override Task OpenAsync(CancellationToken cancellationToken) => oledbConnection.OpenAsync(cancellationToken);
        public override string Caption => oledbConnection.ConnectionString;

        void OnInfoMessage(object sender, OleDbInfoMessageEventArgs e)
        {
            var text = e.Message;
            InvokeInfoMessage(new[] { InfoMessageFactory.Create(InfoMessageSeverity.Information, null, text) });
        }

        public override string DataSource => oledbConnection.DataSource;
        public override string ServerVersion => oledbConnection.ServerVersion;

        public override IDbCommand CreateCommand() => oledbConnection.CreateCommand();

        protected override void SetDatabase(string database)
        {
        }

        public override int TransactionCount => 0;
    }
}