using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class SmallDateTimeDataFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal) : IDataFieldReader
{
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
                var dateTime = dataRecord.GetDateTime(columnOrdinal);
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