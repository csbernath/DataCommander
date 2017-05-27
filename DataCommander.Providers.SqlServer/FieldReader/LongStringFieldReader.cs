using System;
using System.Data;
using DataCommander.Providers.Field;

namespace DataCommander.Providers.SqlServer.FieldReader
{
    internal sealed class LongStringFieldReader : IDataFieldReader
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;

        public LongStringFieldReader(IDataRecord dataRecord, int columnOrdinal)
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
                    var s = _dataRecord.GetString(_columnOrdinal);
                    value = new StringField(s, SqlServerProvider.ShortStringSize);
                }

                return value;
            }
        }
    }
}