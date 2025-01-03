using System;
using System.IO;
using System.Reflection;

namespace Foundation.Diagnostics;

public static class DotNetVersionGetter
{
    public static string GetDotNetRuntimeVersion() => GetVersion(typeof(int));

    public static string GetVersion(Type type)
    {
        var assembly = Assembly.GetAssembly(type);
        var location = assembly!.Location;
        var directoryName = Path.GetDirectoryName(location);
        var version = Path.GetFileName(directoryName);
        return version!;
    }
}