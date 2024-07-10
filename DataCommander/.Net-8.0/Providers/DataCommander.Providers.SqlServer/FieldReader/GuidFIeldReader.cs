using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

public class GuidFieldReader : IDataFieldReader
{
    private readonly IDataRecord _dataRecord;
    private readonly int _columnOrdinal;

    public GuidFieldReader(IDataRecord dataRecord, int columnOrdinal)
    {
        _dataRecord = dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    public object Value => _dataRecord.GetGuid(_columnOrdinal).ToString().ToUpper();
}