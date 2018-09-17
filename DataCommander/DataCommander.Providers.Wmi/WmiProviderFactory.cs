using System.Data.Common;

namespace DataCommander.Providers.Wmi
{
    internal sealed class WmiProviderFactory : DbProviderFactory
    {
        public static WmiProviderFactory Instance = new WmiProviderFactory();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new DbConnectionStringBuilder();
        }
    }
}