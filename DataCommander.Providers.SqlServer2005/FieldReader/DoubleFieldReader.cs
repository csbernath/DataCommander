namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;

    sealed class DoubleFieldReader : IDataFieldReader
    {
        public DoubleFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    double d = dataRecord.GetDouble(columnOrdinal);
                    value = new DoubleField(d);
                }

                return value;
            }
        }

        IDataRecord dataRecord;
        int columnOrdinal;
    }
}