using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class ShortStringFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal,
    SqlDbType sqlDbType) : IDataFieldReader
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
                string s = dataRecord.GetString(columnOrdinal);

                if (sqlDbType == SqlDbType.Char ||
                    sqlDbType == SqlDbType.NChar)
                    s = s.TrimEnd();

                value = s;
            }

            return value;
        }
    }
}