using System;
using System.Data;

namespace DataCommander.Providers2.FieldNamespace
{
    public sealed class SingleFieldDataReader : IDataFieldReader
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;

        public SingleFieldDataReader(IDataRecord dataRecord, int columnOrdinal)
        {
            _dataRecord = dataRecord;
            _columnOrdinal = columnOrdinal;
        }

        #region IDataFieldReader Members

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
                    var singleValue = (float)_dataRecord[_columnOrdinal];
                    value = new SingleField(singleValue);
                }

                return value;
            }
        }

        #endregion
    }
}