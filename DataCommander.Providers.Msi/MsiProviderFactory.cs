namespace DataCommander.Providers.Msi
{
    using System.Data.Common;

    internal sealed class MsiProviderFactory : DbProviderFactory
	{
		private static MsiProviderFactory instance = new MsiProviderFactory();

		private MsiProviderFactory()
		{
		}

		public static MsiProviderFactory Instance => instance;

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new DbConnectionStringBuilder();
		}
	}
}
