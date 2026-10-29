using System;
using System.Linq;
using DataCommander.Api;
using DataCommander.Api.Connection;
using MongoDB.Driver;

namespace DataCommander.Providers.MongoDB;

public class MongoConnectionStringBuilder : IDbConnectionStringBuilder
{
    private MongoUrlBuilder? _mongoUrlBuilder;

    public MongoConnectionStringBuilder()
    {
    }

    public MongoConnectionStringBuilder(string connectionString)
    {
        _mongoUrlBuilder = new MongoUrlBuilder(connectionString);
    }

    public string ConnectionString
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_mongoUrlBuilder);
            return _mongoUrlBuilder.ToString();
        }

        set => _mongoUrlBuilder = new MongoUrlBuilder(value);
    }

    public bool IsKeywordSupported(string keyword)
    {
        string[] supportedKeyWords =
        [
            ConnectionStringKeyword.DataSource,
            ConnectionStringKeyword.InitialCatalog,
            ConnectionStringKeyword.UserId,
            ConnectionStringKeyword.Password
        ];
        return Enumerable.Contains(supportedKeyWords, keyword);
    }

    public void SetValue(string keyword, object? value)
    {
        if (_mongoUrlBuilder == null)
            _mongoUrlBuilder = new MongoUrlBuilder();
        
        switch (keyword)
        {
            case ConnectionStringKeyword.DataSource:
                _mongoUrlBuilder.Server = MongoServerAddress.Parse((string?)value);
                break;
            
            case ConnectionStringKeyword.InitialCatalog:
                _mongoUrlBuilder.DatabaseName = (string?)value;
                break;
            
            case ConnectionStringKeyword.UserId:
                _mongoUrlBuilder.Username = (string?)value;
                break;
            
            case ConnectionStringKeyword.Password:
                _mongoUrlBuilder.Password = (string?)value;
                break;
        }
    }

    public bool TryGetValue(string keyword, out object? value)
    {
        bool contains;
        switch (keyword)
        {
            case ConnectionStringKeyword.DataSource:
                value = _mongoUrlBuilder.Server.ToString();
                contains = true;
                break;
            
            case ConnectionStringKeyword.InitialCatalog:
                value = _mongoUrlBuilder.DatabaseName;
                contains = true;
                break;
            
            case ConnectionStringKeyword.UserId:
                value = _mongoUrlBuilder.Username;
                contains = true;
                break;

            default:
                value = null;
                contains = false;
                break;
        }

        return contains;
    }

    public bool Remove(string keyword) => throw new NotImplementedException();
}