namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class BooleanDataFieldReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

        public BooleanDataFieldReader(IDataRecord dataRecord, int columnOrdinal)
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
                    var booleanValue = this.dataRecord.GetBoolean(this.columnOrdinal);
                    value = new BooleanField(booleanValue);
                }

                return value;
            }
        }
    }
}