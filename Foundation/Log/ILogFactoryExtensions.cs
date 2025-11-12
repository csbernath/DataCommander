using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Log;

public static class LogFactoryExtensions
{
    public static ILog GetTypeLog(this ILogFactory logFactory, Type? type)
    {
        ArgumentNullException.ThrowIfNull(logFactory);
        ArgumentNullException.ThrowIfNull(type);

        var name = type.FullName;
        var log = logFactory.GetLog(name);
        return log;
    }

    public static ILog GetTypeLog<T>(this ILogFactory logFactory)
    {
        var name = typeof(T).FullName;
        var log = logFactory.GetLog(name);
        return log;
    }

    public static ILog GetCurrentTypeLog(this ILogFactory applicationLog)
    {
        var stackFrame = new StackFrame(1, false);
        var type = stackFrame.GetMethod()!.DeclaringType;
        return applicationLog.GetTypeLog(type);
    }

    public static ILog GetCurrentTypeSectionLog(this ILogFactory applicationLog, string sectionName)
    {
        var stackFrame = new StackFrame(1, false);
        var type = stackFrame.GetMethod()!.DeclaringType!;
        var name = $"{type.FullName}.{sectionName}";
        var log = applicationLog.GetLog(name);
        //if (log is DefaultLog.Log foundationLog)
        //    foundationLog.LoggedName = $"{type.Name}.{sectionName}";

        return log;
    }

    public static ILog GetCurrentMethodLog(this ILogFactory applicationLog, params object[] parameters)
    {
        var stackFrame = new StackFrame(1, false);
        var method = stackFrame.GetMethod()!;
        var type = method.DeclaringType!;
        var name = $"{type.FullName}.{method.Name}";
        var log = applicationLog.GetLog(name);
        //if (log is DefaultLog.Log foundationLog)
        //    foundationLog.LoggedName = $"{type.Name}.{method.Name}";

        if (parameters.Length > 0)
        {
            var parameterInfos = method.GetParameters();
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Entering method {method.Name}(");
            var count = Math.Min(parameterInfos.Length, parameters.Length);

            for (var i = 0; i < count; i++)
            {
                var parameterInfo = parameterInfos[i];
                stringBuilder.Append($"\r\n{parameterInfo.ParameterType.Name} {parameterInfo.Name}");
                if (i < parameters.Length)
                {
                    stringBuilder.Append(" = ");
                    var parameterString = ParameterValueToString(parameters[i]);
                    stringBuilder.Append(parameterString);
                }

                if (i < count - 1)
                {
                    stringBuilder.Append(',');
                }
            }

            stringBuilder.Append(')');
            var message = stringBuilder.ToString();
            log.Trace(message);
        }

        return log;
    }

    private static string? ParameterValueToString(object value)
    {
        string? parameterString;
        if (value != null)
        {
            parameterString = value as string;
            parameterString = parameterString != null
                ? $"\"{parameterString}\""
                : value.ToString();
        }
        else
            parameterString = "null";

        return parameterString;
    }
}