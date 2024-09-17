using System.Data;
using DataCommander.Api.FieldReaders;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class TimeStampDataFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal) : IDataFieldReader
{
    private readonly SqlDataReader _sqlDataReader = (SqlDataReader) dataRecord;

    public object Value
    {
        get
        {
            object o = _sqlDataReader.GetValue(columnOrdinal);
            return o;
        }
    }
}