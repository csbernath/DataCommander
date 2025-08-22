namespace DataCommander.Api;

public static class IDbConnectionStringBuilderExtensions
{
    extension(IDbConnectionStringBuilder dbConnectionStringBuilder)
    {
        public bool IsKeywordSupportedAndTryGetValue<T>(string keyWord, out T? value)
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
}