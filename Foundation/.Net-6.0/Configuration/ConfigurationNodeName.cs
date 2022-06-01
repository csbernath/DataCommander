using System;
using System.Diagnostics;
using System.Reflection;
using Foundation.Assertions;

namespace Foundation.Configuration;

public static class ConfigurationNodeName
{
    public static string FromType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        var name = type.FullName;
        var nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    private static MethodBase GetMethod(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var frame = trace.GetFrame(frameIndex);
        var method = frame.GetMethod();
        return method;
    }

    internal static string FromMethod(MethodBase method)
    {
        ArgumentNullException.ThrowIfNull(method, nameof(method));

        var name = method.DeclaringType.FullName + Type.Delimiter + method.Name;
        var nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    internal static string FromMethod(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace, nameof(trace));

        var method = GetMethod(trace, frameIndex);
        var nodeName = FromMethod(method);
        return nodeName;
    }

    internal static string FromNamespace(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var method = GetMethod(trace, frameIndex);
        var name = method.DeclaringType.Namespace;
        var nodeName = FromTypeDelimitedName(name);
        return nodeName;
    }

    internal static string FromType(StackTrace trace, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(trace, nameof(trace));

        var method = GetMethod(trace, frameIndex);
        var type = method.DeclaringType;
        var nodeName = FromType(type);
        return nodeName;
    }

    private static string FromTypeDelimitedName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        var nodeName = name.Replace(Type.Delimiter, ConfigurationNode.Delimiter);
        return nodeName;
    }
}