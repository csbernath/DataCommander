namespace Foundation.Log
{
    public static class LogFactory
    {
        public static ILogFactory Instance { get; set; } = NullLogFactory.Instance;
    }
}