using System;
using Foundation.Assertions;
using Foundation.Configuration;

namespace Foundation.Log
{
    public static class LogFactory
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(LogFactory));

        public static ILogFactory Instance { get; set; } = NullLogFactory.Instance;

        public static void Read()
        {
            Log.Trace("Reading LogFactory configuration...");
            var node = Settings.SelectCurrentType();
            if (node != null)
            {
                var typeName = node.Attributes["TypeName"].GetValue<string>();
                var type = Type.GetType(typeName, true);
                var instance = Activator.CreateInstance(type);

                Assert.IsTrue(instance is ILogFactory);
                var applicationLog = (ILogFactory) instance;
                instance = applicationLog;
            }

            Log.Trace("LogFactory configuration has been read successfully.");
        }
    }
}