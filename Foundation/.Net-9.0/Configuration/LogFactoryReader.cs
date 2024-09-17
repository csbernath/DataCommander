using System;
using Foundation.Assertions;
using Foundation.InternalLog;
using Foundation.Log;

namespace Foundation.Configuration;

public static class LogFactoryReader
{
    private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(LogFactory));

    public static void Read()
    {
        Log.Trace("Reading LogFactory configuration...");
        ConfigurationNode node = Settings.SelectCurrentType();
        if (node != null)
        {
            string typeName = node.Attributes["TypeName"].GetValue<string>();
            Type type = Type.GetType(typeName, true);
            object instance = Activator.CreateInstance(type);

            Assert.IsTrue(instance is ILogFactory);
            ILogFactory applicationLog = (ILogFactory) instance;
            LogFactory.Set(applicationLog);
        }

        Log.Trace("LogFactory configuration has been read successfully.");
    }
}