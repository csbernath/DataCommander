using DataCommander.Providers2;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDataReaderHelper : IDataReaderHelper
    {
        private readonly TfsDataReader _dataReader;

        public TfsDataReaderHelper(TfsDataReader dataReader) => _dataReader = dataReader;

        int IDataReaderHelper.GetValues(object[] values) => _dataReader.GetValues(values);
    }
}