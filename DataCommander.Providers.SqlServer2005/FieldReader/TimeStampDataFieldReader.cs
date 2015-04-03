namespace DataCommander.Providers.SqlServer2005
{
    using System.Data;
    using System.Data.SqlClient;

    sealed class TimeStampDataFieldReader : IDataFieldReader
    {
        public TimeStampDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.sqlDataReader = (SqlDataReader)dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        public object Value
        {
            get
            {
                object o = this.sqlDataReader.GetValue(this.columnOrdinal);
                return o;
            }
        }

        readonly SqlDataReader sqlDataReader;
        readonly int columnOrdinal;
    }
}