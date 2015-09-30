namespace DataCommander.Providers.PostgreSql
{
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