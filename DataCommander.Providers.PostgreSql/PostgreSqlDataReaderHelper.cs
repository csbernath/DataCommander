namespace DataCommander.Providers.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataCommander.Providers;
    using Npgsql;

    internal sealed class PostgreSqlDataReaderHelper : IDataReaderHelper
    {
        private readonly NpgsqlDataReader dataReader;

        public PostgreSqlDataReaderHelper(NpgsqlDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }
    }
}