using System;
using Foundation.Assertions;
using Foundation.Diagnostics;
using Foundation.InternalLog;
using Foundation.Log;

namespace Foundation.Configuration;

public static class LogFactoryReader
{
    public static void Read()
    {
        var currentLog = InternalLogFactory.Instance.GetTypeLog(typeof(LogFactory));
        GarbageMonitor.Default.Add(nameof(InternalLogFactory), currentLog);
        
        currentLog.Trace("Reading LogFactory configuration...");
        var node = Settings.SelectCurrentType();
        if (node != null)
        {
            var typeName = node.Attributes["TypeName"].GetValue<string>()!;
            var type = Type.GetType(typeName, true)!;
            var instance = Activator.CreateInstance(type)!;

            Assert.IsTrue(instance is ILogFactory);
            var applicationLog = (ILogFactory) instance;

            LogFactory.Set(applicationLog);
            LogFactory.Instance.Write(InternalLogFactory.InternalLogWriter.LogEntries);
            currentLog = LogFactory.Instance.GetLog(null);
        }

        currentLog.Trace("LogFactory configuration has been read successfully.");
    }
}