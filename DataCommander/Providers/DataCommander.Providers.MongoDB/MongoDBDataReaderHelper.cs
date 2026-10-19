using DataCommander.Api;

namespace DataCommander.Providers.MongoDB;

public class MongoDBDataReaderHelper(MongoDbDataReader mongoDbDataReader) : IDataReaderHelper
{
    public int GetValues(object?[] values) => mongoDbDataReader.GetValues(values);
}