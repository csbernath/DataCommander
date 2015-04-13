namespace SqlUtil.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;

    internal sealed class OracleConnectionStringBuilder : DbConnectionStringBuilder
    {
        private bool enlist;

        public OracleConnectionStringBuilder()
            : this(null)
        {
        }

        public OracleConnectionStringBuilder(string connectionString)
        {
            this.enlist = true;
            this.ConnectionString = connectionString;
        }

        public bool Enlist
        {
            get
            {
                return this.enlist;
            }
        }
    }
}
