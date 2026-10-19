using DataCommander.Api;

namespace DataCommander.Providers.MongoDB;

public class MongoConnectionStringBuilder : IDbConnectionStringBuilder
{
    public string ConnectionString
    {
        get => string.Empty;

        set { }
    }

    public bool IsKeywordSupported(string keyword)
    {
        return false;
    }

    public void SetValue(string keyword, object? value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(string keyword, out object? value)
    {
        value = null;
        return false;
    }

    public bool Remove(string keyword) => throw new System.NotImplementedException();
}