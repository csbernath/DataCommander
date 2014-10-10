namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using DataCommander.Providers;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsConnection : ConnectionBase
    {
        private TeamFoundationServer teamFoundationServer;
        private VersionControlServer versionControlServer;
        private string connectionName;
        private ConnectionState state;

        public TfsConnection(string teamFoundationServerUrl)
        {
            this.teamFoundationServer = new TeamFoundationServer(teamFoundationServerUrl);
            this.versionControlServer = (VersionControlServer)teamFoundationServer.GetService(typeof(VersionControlServer));
            this.Connection = new TfsDbConnection(this);
        }

        internal TeamFoundationServer TeamFoundationServer
        {
            get
            {
                return this.teamFoundationServer;
            }
        }

        internal VersionControlServer VersionControlServer
        {
            get
            {
                return this.versionControlServer;
            }
        }

        public override void Open()
        {
            teamFoundationServer.Authenticate();
            this.state = ConnectionState.Open;
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

        public override string Caption
        {
            get
            {
                return "Team Foundation Server";
            }
        }

        public override string DataSource
        {
            get
            {
                return null;
            }
        }

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
                return string.Format("versionControlServer.SupportedFeatures: {0}", supportedFeaturesEnum.ToString("G"));
            }
        }

        public override int TransactionCount
        {
            get
            {
                return 0;
            }
        }

        public ConnectionState ConnectionState
        {
            get
            {
                return this.state;
            }
        }
    }
}
