namespace DataCommander.Providers.QueryConfiguration
{
    public static class Parser
    {
        public static void ParseResult(string result, out string name, out string fieldName)
        {
            name = result;
            fieldName = result;

            var startIndex = result.IndexOf('(');
            if (startIndex >= 0 && startIndex < result.Length - 1)
            {
                var endIndex = result.IndexOf(')', startIndex + 1);
                if (endIndex >= 0)
                {
                    name = result.Substring(0, startIndex);
                    fieldName = name + result.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
            }
        }
    }
}