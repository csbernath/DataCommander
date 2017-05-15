namespace DataCommander.Providers.SqlServer.FieldReader
{
    using System;
    using System.Data;
    using Field;

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

                if (this.dataRecord.IsDBNull(this.columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var d = this.dataRecord.GetDouble(this.columnOrdinal);
                    value = new DoubleField(d);
                }

                return value;
            }
        }

        readonly IDataRecord dataRecord;
        readonly int columnOrdinal;
    }
}