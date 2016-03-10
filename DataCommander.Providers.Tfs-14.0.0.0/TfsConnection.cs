namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsConnection : ConnectionBase
    {
        private readonly TfsTeamProjectCollection tfsTeamProjectCollection;
        private readonly VersionControlServer versionControlServer;
        private string connectionName;
        private ConnectionState state;

        public TfsConnection(Uri uri)
        {
            this.tfsTeamProjectCollection = new TfsTeamProjectCollection(uri);
            this.versionControlServer = (VersionControlServer)this.tfsTeamProjectCollection.GetService(typeof (VersionControlServer));
            this.Connection = new TfsDbConnection(this);
        }

        internal TfsTeamProjectCollection TfsTeamProjectCollection => this.tfsTeamProjectCollection;

        internal VersionControlServer VersionControlServer => this.versionControlServer;

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                this.tfsTeamProjectCollection.Authenticate();
                this.state = ConnectionState.Open;
            });
        }

        public override IDbCommand CreateCommand()
        {
            return new TfsCommand(this);
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

        public override string Caption => "Team Foundation Server";

        public override string DataSource => null;

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion
        {
            get
            {
                int supportedFeatures = this.versionControlServer.SupportedFeatures;
                SupportedFeatures supportedFeaturesEnum = (SupportedFeatures)supportedFeatures;
                return $"versionControlServer.SupportedFeatures: {supportedFeaturesEnum.ToString("G")}";
            }
        }

        public override int TransactionCount => 0;

        public ConnectionState ConnectionState => this.state;
    }
}