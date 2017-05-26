using System;
using Foundation.Configuration;

namespace Foundation.Diagnostics.Log
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogFactory
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof (LogFactory));

        /// <summary>
        /// 
        /// </summary>
        public static ILogFactory Instance { get; set; } = NullApplicationLog.Instance;

        /// <summary>
        /// 
        /// </summary>
        public static void Read()
        {
            Log.Trace("Reading LogFactory configuration...");
            var node = Settings.SelectCurrentType();
            if (node != null)
            {
                var typeName = node.Attributes["TypeName"].GetValue<string>();
                var type = Type.GetType(typeName, true);
                var instance = Activator.CreateInstance(type);
#if CONTRACTS_FULL
                Contract.Assert(instance is ILogFactory);
#endif
                var applicationLog = (ILogFactory)instance;
                instance = applicationLog;
            }

            Log.Trace("LogFactory configuration has been read successfully.");
        }
    }
}