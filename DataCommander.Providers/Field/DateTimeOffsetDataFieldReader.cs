namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class DateTimeOffsetDataFieldReader : IDataFieldReader
    {
        private IDataRecord dataRecord;
        private int columnOrdinal;

        public DateTimeOffsetDataFieldReader(
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
                    value = dataRecord[columnOrdinal];
                    DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                    value = new DateTimeOffsetField(dateTimeOffset);
                }

                return value;
            }
        }
    }
}