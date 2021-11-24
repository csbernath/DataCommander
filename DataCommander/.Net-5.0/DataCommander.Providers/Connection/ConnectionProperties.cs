using DataCommander.Providers2.Connection;
using Foundation.Core;

namespace DataCommander.Providers.Connection
{
    public sealed class ConnectionProperties
    {
        public string ConnectionName;
        public string ProviderName;
        public Option<string> Password;
        public IProvider Provider;
        public string ConnectionString;
        public ConnectionBase Connection;
    }
}