namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDataReaderHelper : IDataReaderHelper
    {
        private readonly TfsDataReader dataReader;

        public TfsDataReaderHelper(TfsDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }

        #endregion
    }
}