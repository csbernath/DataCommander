namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using System.Data;
    using Field;

    internal sealed class LongStringFieldReader : IDataFieldReader
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;

        public LongStringFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal )
        {
            _dataRecord = dataRecord;
            _columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (_dataRecord.IsDBNull( _columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var s = _dataRecord.GetString( _columnOrdinal );
                    value = new StringField( s, 1024 );
                }

                return value;
            }
        }
    }
}