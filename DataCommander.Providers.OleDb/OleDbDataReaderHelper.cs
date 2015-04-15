namespace DataCommander.Providers.OleDb
{
    using System.Data.OleDb;

    internal sealed class OleDbDataReaderHelper : IDataReaderHelper
    {
        public OleDbDataReaderHelper(OleDbDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }

        private readonly OleDbDataReader dataReader;
    }
}