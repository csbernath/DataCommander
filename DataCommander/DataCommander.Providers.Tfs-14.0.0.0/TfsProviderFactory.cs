namespace DataCommander.Providers.Tfs
{
    using System.Data.Common;

    internal sealed class TfsProviderFactory : DbProviderFactory
    {
        public static TfsProviderFactory Instance { get; } = new TfsProviderFactory();

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