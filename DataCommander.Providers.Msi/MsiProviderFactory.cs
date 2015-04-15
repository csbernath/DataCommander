namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Linq;
	using System.Text;

	internal sealed class MsiProviderFactory : DbProviderFactory
	{
		private static MsiProviderFactory instance = new MsiProviderFactory();

		private MsiProviderFactory()
		{
		}

		public static MsiProviderFactory Instance
		{
			get
			{
				return instance;
			}
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new DbConnectionStringBuilder();
		}
	}
}
