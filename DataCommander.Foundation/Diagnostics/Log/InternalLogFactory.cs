namespace DataCommander.Foundation.Diagnostics
{
    internal static class InternalLogFactory
    {
        private static readonly FoundationLogFactory instance = new FoundationLogFactory( true );

        public static ILogFactory Instance
        {
            get
            {
                return instance;
            }
        }
    }
}