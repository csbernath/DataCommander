namespace Foundation.Log
{
    internal static class InternalLogFactory
    {
        private static readonly FoundationLogFactory instance = new FoundationLogFactory(true);

        public static ILogFactory Instance => instance;
    }
}