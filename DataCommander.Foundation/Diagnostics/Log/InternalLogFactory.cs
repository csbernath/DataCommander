namespace DataCommander.Foundation.Diagnostics.Log
{
    internal static class InternalLogFactory
    {
        private static readonly FoundationLogFactory instance = new FoundationLogFactory(true);

        public static ILogFactory Instance => instance;
    }
}