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
        var node = Settings.SelectCurrentType();
        if (node != null)
        {
            var typeName = node.Attributes["TypeName"].GetValue<string>()!;
            var type = Type.GetType(typeName, true)!;
            var instance = Activator.CreateInstance(type)!;

            Assert.IsTrue(instance is ILogFactory);
            var applicationLog = (ILogFactory) instance;
            LogFactory.Set(applicationLog);
        }

        Log.Trace("LogFactory configuration has been read successfully.");
    }
}