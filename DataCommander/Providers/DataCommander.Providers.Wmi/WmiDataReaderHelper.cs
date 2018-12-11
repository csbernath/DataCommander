using System.Data;

namespace DataCommander.Providers.Wmi
{
    /// <summary>
    /// Summary description for WmiDataReaderHelper.
    /// </summary>
    sealed class WmiDataReaderHelper : IDataReaderHelper
    {
        public WmiDataReaderHelper(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        readonly IDataReader _dataReader;
    }
}