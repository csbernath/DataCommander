using System;
using System.Data;
using DataCommander.Providers.FieldNamespace;

namespace DataCommander.Providers.SqlServer.FieldReader
{
    sealed class ShortStringFieldReader : IDataFieldReader
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;
        private readonly SqlDbType _sqlDbType;

        public ShortStringFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal,
            SqlDbType sqlDbType)
        {
            _dataRecord = dataRecord;
            _columnOrdinal = columnOrdinal;
            _sqlDbType = sqlDbType;
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
                    var s = _dataRecord.GetString(_columnOrdinal);

                    if (_sqlDbType == SqlDbType.Char ||
                        _sqlDbType == SqlDbType.NChar)
                    {
                        s = s.TrimEnd();
                    }

                    value = s;
                }

                return value;
            }
        }

        #endregion
    }
}