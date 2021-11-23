using DataCommander.Providers2.Connection;
using Foundation.Core;
using Foundation.Log;

namespace DataCommander.Providers.Connection
{
    public sealed class ConnectionProperties
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        public string ConnectionName;
        public string ProviderName;

        public string DataSource;
        public string InitialCatalog;
        public bool? IntegratedSecurity;
        public string UserId;
        public Option<string> Password;
        public bool? TrustServerCertificate;

        public IProvider Provider;
        public string ConnectionString;
        public ConnectionBase Connection;
    }
}