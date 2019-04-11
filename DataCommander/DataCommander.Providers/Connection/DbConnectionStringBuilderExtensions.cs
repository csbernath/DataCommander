using System.Data.Common;

namespace DataCommander.Providers.Connection
{
    internal static class DbConnectionStringBuilderExtensions
    {
        public static string GetValue(this DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
        {
            var contains = dbConnectionStringBuilder.TryGetValue(keyword, out var obj);
            string value;

            if (contains)
            {
                value = (string)obj;
            }
            else
            {
                value = null;
            }

            return value;
        }
    }
}