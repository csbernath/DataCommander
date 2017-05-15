namespace DataCommander.Providers.Connection
{
    using System.Data.Common;

    internal static class DbConnectionStringBuilderExtensions
    {
        public static string GetValue(this DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
        {
            object obj;
            var contains = dbConnectionStringBuilder.TryGetValue(keyword, out obj);
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