namespace SqlUtil.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using Oracle.DataAccess.Client;

    internal sealed class OracleClientFactory : DbProviderFactory
    {
        private static OracleClientFactory instance = new OracleClientFactory();

        public static OracleClientFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new OracleConnectionStringBuilder();
        }
    }
}
