using DataCommander.Api;

namespace DataCommander.Providers.MongoDB;

public class MongoDataReaderHelper(MongoDataReader mongoDataReader) : IDataReaderHelper
{
    public int GetValues(object?[] values) => mongoDataReader.GetValues(values);
}