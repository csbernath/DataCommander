using Foundation.DefaultLog;

namespace Foundation.InternalLog
{
    internal static class InternalLogFactory
    {
        public static readonly LogFactory Instance = new LogFactory(true);
    }
}