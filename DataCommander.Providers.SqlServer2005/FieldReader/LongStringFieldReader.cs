namespace DataCommander.Providers.SqlServer2005.FieldReader
{
    using System;
    using System.Data;

    internal sealed class LongStringFieldReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

        public LongStringFieldReader(
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
                    var s = this.dataRecord.GetString(this.columnOrdinal);
                    value = new StringField(s, SqlServerProvider.ShortStringSize);
                }

                return value;
            }
        }
    }
}