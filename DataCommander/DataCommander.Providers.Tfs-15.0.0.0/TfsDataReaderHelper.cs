namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDataReaderHelper : IDataReaderHelper
    {
        private readonly TfsDataReader _dataReader;

        public TfsDataReaderHelper(TfsDataReader dataReader)
        {
            this._dataReader = dataReader;
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        #endregion
    }
}