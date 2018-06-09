using System.Data.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsProviderFactory : DbProviderFactory
    {
        public static TfsProviderFactory Instance { get; } = new TfsProviderFactory();
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new DbConnectionStringBuilder();
        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => new TfsDataSourceEnumerator();
    }
}