using System;
using System.Diagnostics;
using System.Reflection;

namespace Foundation.Configuration;

public static class ConfigurationNodeName
{
    public static string FromType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        string name = type.FullName;
        string nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    private static MethodBase GetMethod(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        StackFrame frame = trace.GetFrame(frameIndex);
        MethodBase method = frame.GetMethod();
        return method;
    }

    internal static string FromMethod(MethodBase method)
    {
        ArgumentNullException.ThrowIfNull(method);

        string name = method.DeclaringType.FullName + Type.Delimiter + method.Name;
        string nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    internal static string FromMethod(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        MethodBase method = GetMethod(trace, frameIndex);
        string nodeName = FromMethod(method);
        return nodeName;
    }

    internal static string FromNamespace(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        MethodBase method = GetMethod(trace, frameIndex);
        string name = method.DeclaringType.Namespace;
        string nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    internal static string FromType(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        MethodBase method = GetMethod(trace, frameIndex);
        Type type = method.DeclaringType;
        string nodeName = FromType(type);
        return nodeName;
    }

    private static string FromTypeDelimitedName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        string nodeName = name.Replace(Type.Delimiter, ConfigurationNode.Delimiter);
        return nodeName;
    }
}