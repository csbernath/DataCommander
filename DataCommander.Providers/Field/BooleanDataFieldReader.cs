namespace DataCommander.Providers.Field
{
    using System;
    using System.Data;

    public sealed class BooleanDataFieldReader : IDataFieldReader
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;

        public BooleanDataFieldReader(IDataRecord dataRecord, int columnOrdinal)
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
                    var booleanValue = _dataRecord.GetBoolean(_columnOrdinal);
                    value = new BooleanField(booleanValue);
                }

                return value;
            }
        }
    }
}