namespace DataCommander.Providers.SqlServer.FieldReader
{
    using System;
    using System.Data;
    using Field;

    sealed class DoubleFieldReader : IDataFieldReader
    {
        public DoubleFieldReader(
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
                    var d = _dataRecord.GetDouble(_columnOrdinal);
                    value = new DoubleField(d);
                }

                return value;
            }
        }

        readonly IDataRecord _dataRecord;
        readonly int _columnOrdinal;
    }
}