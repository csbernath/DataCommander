namespace DataCommander.Api;

public static class IDbConnectionStringBuilderExtensions
{
    public static bool IsKeywordSupportedAndTryGetValue<T>(this IDbConnectionStringBuilder dbConnectionStringBuilder, string keyWord, out T? value)
    {
        var isKeywordSupportedAndTryGetValue = false;
        value = default;
        
        if (dbConnectionStringBuilder.IsKeywordSupported(keyWord))
        {
            isKeywordSupportedAndTryGetValue = dbConnectionStringBuilder.TryGetValue(keyWord, out var valueObject);
            if (isKeywordSupportedAndTryGetValue)
                value = (T?)valueObject;
        }

        return isKeywordSupportedAndTryGetValue;
    }
}