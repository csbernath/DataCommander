using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Log;

public static class LogFactoryExtensions
{
    public static ILog GetTypeLog(this ILogFactory logFactory, Type type)
    {
        ArgumentNullException.ThrowIfNull(logFactory);
        ArgumentNullException.ThrowIfNull(type);

        string name = type.FullName;

        ILog log = logFactory.GetLog(name);
        //if (log is DefaultLog.Log foundationLog)
        //    foundationLog.LoggedName = type.Name;

        return log;
    }

    public static ILog GetCurrentTypeLog(this ILogFactory applicationLog)
    {
        StackFrame stackFrame = new StackFrame(1, false);
        Type type = stackFrame.GetMethod().DeclaringType;
        return applicationLog.GetTypeLog(type);
    }

    public static ILog GetCurrentTypeSectionLog(this ILogFactory applicationLog, string sectionName)
    {
        StackFrame stackFrame = new StackFrame(1, false);
        Type type = stackFrame.GetMethod().DeclaringType;
        string name = $"{type.FullName}.{sectionName}";
        ILog log = applicationLog.GetLog(name);
        //if (log is DefaultLog.Log foundationLog)
        //    foundationLog.LoggedName = $"{type.Name}.{sectionName}";

        return log;
    }

    public static ILog GetCurrentMethodLog(this ILogFactory applicationLog, params object[] parameters)
    {
        StackFrame stackFrame = new StackFrame(1, false);
        System.Reflection.MethodBase method = stackFrame.GetMethod();
        Type type = method.DeclaringType;
        string name = $"{type.FullName}.{method.Name}";
        ILog log = applicationLog.GetLog(name);
        //if (log is DefaultLog.Log foundationLog)
        //    foundationLog.LoggedName = $"{type.Name}.{method.Name}";

        if (parameters.Length > 0)
        {
            System.Reflection.ParameterInfo[] parameterInfos = method.GetParameters();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Entering method {0}(", method.Name);
            int count = Math.Min(parameterInfos.Length, parameters.Length);

            for (int i = 0; i < count; i++)
            {
                System.Reflection.ParameterInfo parameterInfo = parameterInfos[i];
                sb.AppendFormat("\r\n{0} {1}", parameterInfo.ParameterType.Name, parameterInfo.Name);
                if (i < parameters.Length)
                {
                    sb.Append(" = ");
                    string parameterString = ParameterValueToString(parameters[i]);
                    sb.Append(parameterString);
                }

                if (i < count - 1)
                {
                    sb.Append(',');
                }
            }

            sb.Append(')');
            string message = sb.ToString();
            log.Trace(message);
        }

        return log;
    }

    private static string ParameterValueToString(object value)
    {
        string parameterString;
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