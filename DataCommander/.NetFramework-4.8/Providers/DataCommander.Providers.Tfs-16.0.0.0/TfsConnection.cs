using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Providers2.Connection;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsConnection : ConnectionBase
    {
        public TfsConnection(Uri uri)
        {
            TfsTeamProjectCollection = new TfsTeamProjectCollection(uri);
            VersionControlServer = (VersionControlServer) TfsTeamProjectCollection.GetService(typeof(VersionControlServer));
            Connection = new TfsDbConnection(this);
        }

        internal TfsTeamProjectCollection TfsTeamProjectCollection { get; }
        internal VersionControlServer VersionControlServer { get; }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                TfsTeamProjectCollection.Authenticate();
                ConnectionState = ConnectionState.Open;
            });
        }

        public override IDbCommand CreateCommand() => new TfsCommand(this);
        public override string ConnectionName { get; set; }
        public override string Caption => "Team Foundation Server";
        public override string DataSource => null;
        protected override void SetDatabase(string database) => throw new NotImplementedException();

        public override string ServerVersion
        {
            get
            {
                var supportedFeatures = VersionControlServer.SupportedFeatures;
                var supportedFeaturesEnum = (SupportedFeatures) supportedFeatures;
                return $"versionControlServer.SupportedFeatures: {supportedFeaturesEnum.ToString("G")}";
            }
        }

        public override int TransactionCount => 0;

        public ConnectionState ConnectionState { get; private set; }
    }
}