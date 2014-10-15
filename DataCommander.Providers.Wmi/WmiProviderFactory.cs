namespace DataCommander.Providers.Wmi
{
    using System.Data.Common;

    internal sealed class WmiProviderFactory : DbProviderFactory
    {
        public static WmiProviderFactory Instance = new WmiProviderFactory();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new DbConnectionStringBuilder();
        }
    }
}