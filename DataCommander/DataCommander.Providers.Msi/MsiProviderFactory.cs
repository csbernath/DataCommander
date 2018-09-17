using System.Data.Common;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiProviderFactory : DbProviderFactory
	{
        private MsiProviderFactory()
		{
		}

		public static MsiProviderFactory Instance { get; } = new MsiProviderFactory();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new DbConnectionStringBuilder();
		}
	}
}
