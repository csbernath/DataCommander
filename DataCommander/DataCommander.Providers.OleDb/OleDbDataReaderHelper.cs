using System.Data.OleDb;

namespace DataCommander.Providers.OleDb
{
    internal sealed class OleDbDataReaderHelper : IDataReaderHelper
    {
        public OleDbDataReaderHelper(OleDbDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return dataReader.GetValues(values);
        }

        private readonly OleDbDataReader dataReader;
    }
}