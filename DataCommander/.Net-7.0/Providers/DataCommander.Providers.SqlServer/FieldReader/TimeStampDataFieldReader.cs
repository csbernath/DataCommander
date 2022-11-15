using System.Data;
using Microsoft.Data.SqlClient;
using DataCommander.Api.FieldNamespace;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class TimeStampDataFieldReader : IDataFieldReader
{
    private readonly int _columnOrdinal;

    private readonly SqlDataReader _sqlDataReader;

    public TimeStampDataFieldReader(
        IDataRecord dataRecord,
        int columnOrdinal)
    {
        _sqlDataReader = (SqlDataReader) dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    public object Value
    {
        get
        {
            var o = _sqlDataReader.GetValue(_columnOrdinal);
            return o;
        }
    }
}