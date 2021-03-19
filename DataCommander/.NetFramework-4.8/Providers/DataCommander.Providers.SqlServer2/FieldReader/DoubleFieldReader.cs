using System;
using System.Data;
using DataCommander.Providers2.FieldNamespace;

namespace DataCommander.Providers.SqlServer2.FieldReader
{
    internal sealed class DoubleFieldReader : IDataFieldReader
    {
        private readonly int _columnOrdinal;
        private readonly IDataRecord _dataRecord;

        public DoubleFieldReader(IDataRecord dataRecord, int columnOrdinal)
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
    }
}