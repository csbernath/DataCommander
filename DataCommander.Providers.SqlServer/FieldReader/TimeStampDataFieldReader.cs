namespace DataCommander.Providers.SqlServer.FieldReader
{
    using System.Data;
    using System.Data.SqlClient;
    using Field;

    sealed class TimeStampDataFieldReader : IDataFieldReader
    {
        public TimeStampDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            _sqlDataReader = (SqlDataReader)dataRecord;
            _columnOrdinal = columnOrdinal;
        }

        public object Value
        {
            get
            {
                var o = _sqlDataReader.GetValue(_columnOrdinal);
                return o;
            }
        }

        readonly SqlDataReader _sqlDataReader;
        readonly int _columnOrdinal;
    }
}