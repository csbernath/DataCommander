using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class SmallDateTimeDataFieldReader : IDataFieldReader
{
    private readonly int _columnOrdinal;
    private readonly IDataRecord _dataRecord;

    public SmallDateTimeDataFieldReader(
        IDataRecord dataRecord,
        int columnOrdinal)
    {
        _dataRecord = dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (_dataRecord.IsDBNull(_columnOrdinal))
            {
                value = DBNull.Value;
            }
            else
            {
                var dateTime = _dataRecord.GetDateTime(_columnOrdinal);
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