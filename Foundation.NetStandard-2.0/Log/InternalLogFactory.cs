namespace Foundation.Log
{
    internal static class InternalLogFactory
    {
        public static readonly FoundationLogFactory Instance = new FoundationLogFactory(true);
    }
}