namespace DataCommander.Providers.MySql
{
    using Providers;
    using global::MySql.Data.MySqlClient;

    internal sealed class MySqlDataReaderHelper : IDataReaderHelper
    {
        private readonly MySqlDataReader dataReader;

        public MySqlDataReaderHelper(MySqlDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return dataReader.GetValues(values);
        }
    }
}
