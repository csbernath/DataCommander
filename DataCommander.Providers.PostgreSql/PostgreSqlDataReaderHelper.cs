namespace DataCommander.Providers.PostgreSql
{
    using Providers;
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
            return dataReader.GetValues(values);
        }
    }
}