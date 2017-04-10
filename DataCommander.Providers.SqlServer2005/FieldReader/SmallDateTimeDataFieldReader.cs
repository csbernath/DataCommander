namespace DataCommander.Providers.SqlServer2005.FieldReader
{
    using System;
    using System.Data;

    internal sealed class SmallDateTimeDataFieldReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

        public SmallDateTimeDataFieldReader(
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
                    var dateTime = this.dataRecord.GetDateTime(this.columnOrdinal);
                    string format;

                    if (dateTime.TimeOfDay.Ticks == 0)
                        format = "yyyy-MM-dd";
                    else
                        format = "yyyy-MM-dd HH:mm";

                    value = dateTime.ToString(format);
                }

                return value;
            }
        }
    }
}