using System;
using System.IO;
using System.Reflection;

namespace Foundation.Diagnostics;

public static class DotNetVersionGetter
{
    public static string GetDotNetRuntimeVersion() => GetVersion(typeof(int));

    public static string GetVersion(Type type)
    {
        Assembly assembly = Assembly.GetAssembly(type);
        string location = assembly!.Location;
        string directoryName = Path.GetDirectoryName(location);
        string version = Path.GetFileName(directoryName);
        return version!;
    }
}