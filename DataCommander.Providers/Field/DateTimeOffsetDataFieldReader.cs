namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class DateTimeOffsetDataFieldReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

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

                if (this.dataRecord.IsDBNull(this.columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = this.dataRecord[this.columnOrdinal];
                    DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                    value = new DateTimeOffsetField(dateTimeOffset);
                }

                return value;
            }
        }
    }
}