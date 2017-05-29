namespace DataCommander.Providers.Wmi
{
    using System.Data;

    /// <summary>
    /// Summary description for WmiDataReaderHelper.
    /// </summary>
    sealed class WmiDataReaderHelper : IDataReaderHelper
    {
        public WmiDataReaderHelper(IDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }

        readonly IDataReader dataReader;
    }
}