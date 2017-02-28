namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Configuration;

    /// <summary>
    /// 
    /// </summary>
    public static class LogFactory
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetTypeLog(typeof (LogFactory));

        /// <summary>
        /// 
        /// </summary>
        public static ILogFactory Instance { get; set; } = NullApplicationLog.Instance;

        /// <summary>
        /// 
        /// </summary>
        public static void Read()
        {
            log.Trace("Reading LogFactory configuration...");
            var node = Settings.SelectCurrentType();
            if (node != null)
            {
                var typeName = node.Attributes["TypeName"].GetValue<string>();
                var type = Type.GetType(typeName, true);
                var instance = Activator.CreateInstance(type);
                Contract.Assert(instance is ILogFactory);
                var applicationLog = (ILogFactory)instance;
                instance = applicationLog;
            }

            log.Trace("LogFactory configuration has been read successfully.");
        }
    }
}