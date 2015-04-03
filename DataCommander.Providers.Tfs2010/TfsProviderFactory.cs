namespace DataCommander.Providers.Tfs
{
    using System.Data.Common;

    internal sealed class TfsProviderFactory : DbProviderFactory
    {
        private static readonly TfsProviderFactory instance = new TfsProviderFactory();

        public static TfsProviderFactory Instance
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

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return new TfsDataSourceEnumerator();
        }
    }
}