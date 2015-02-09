namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;

    internal sealed class SmallDateTimeDataFieldReader : IDataFieldReader
    {
        private IDataRecord dataRecord;
        private int columnOrdinal;

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

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    DateTime dateTime = dataRecord.GetDateTime(columnOrdinal);
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