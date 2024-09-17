namespace DataCommander.Api;

public static class IDbConnectionStringBuilderExtensions
{
    public static bool IsKeywordSupportedAndTryGetValue<T>(this IDbConnectionStringBuilder dbConnectionStringBuilder, string keyWord, out T? value)
    {
        object? valueObject = null;
        bool isSuppertedAndTryGetValue = dbConnectionStringBuilder.IsKeywordSupported(keyWord) &&
                                        dbConnectionStringBuilder.TryGetValue(keyWord, out valueObject);
        value = isSuppertedAndTryGetValue
            ? (T)valueObject!
            : default;
        return isSuppertedAndTryGetValue;
    }
}