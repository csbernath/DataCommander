using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Text;
using Foundation.Core;
using Foundation.Log;
using Foundation.Text;

namespace Foundation.Diagnostics;

public static class AppDomainMonitor
{
    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(AppDomainMonitor));

    private static readonly StringTableColumnInfo<AssemblyInfo>[] Columns =
    [
        new("Name", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Name),
        StringTableColumnInfo.Create<AssemblyInfo, Version>("FileVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.FileVersion),
        StringTableColumnInfo.Create<AssemblyInfo, Version>("Version", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Version),
        new("Date", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Date?.ToString("yyyy-MM-dd HH:mm:ss")),
        new("PublicKeyToken", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.PublicKeyToken),
        new("ImageRuntimeVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.ImageRuntimeVersion),
        new("Location", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Location),
        StringTableColumnInfo.CreateLeft<AssemblyInfo, bool>("IsDynamic", i => i.IsDynamic)
    ];

    public static string GetEnvironmentInfo()
    {
        long tickCount64 = Environment.TickCount64;
        double totalDays = (double)tickCount64 / TimeSpan.MillisecondsPerDay;
        long workingSet = Environment.WorkingSet;
        WindowsCurrentVersion windowsVersionInfo = WindowsCurrentVersionRepository.Get();
        string stopwatchFrequency = GetStopwatchFrequency();
        DateTime zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
        string tickCountString = $"{tickCount64} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";

        string message = $@"Environment information
MachineName:            {Environment.MachineName}
ProcessorCount:         {Environment.ProcessorCount}
OSVersion:              {Environment.OSVersion}
Windows ProductName:    {windowsVersionInfo.ProductName}
Windows DisplayVersion: {windowsVersionInfo.DisplayVersion}
Windows ReleaseId:      {windowsVersionInfo.ReleaseId}
Windows CurrentBuild:   {windowsVersionInfo.CurrentBuild}
Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}
Is64BitProcess:         {Environment.Is64BitProcess}
IntPtr.Size:            {IntPtr.Size} ({IntPtr.Size * 8} bit)
CLR version:            {Environment.Version}
.NET Runtime version:   {DotNetVersionGetter.GetDotNetRuntimeVersion()}
UserDomainName:         {Environment.UserDomainName}
UserName:               {Environment.UserName}
UserInteractive:        {Environment.UserInteractive}
CurrentDirectory:       {Environment.CurrentDirectory}
CommandLine:            {Environment.CommandLine}
GC IsServerGC:          {GCSettings.IsServerGC}
GC LargeObjectHeapCompactionMode: {GCSettings.LargeObjectHeapCompactionMode}
GC LatencyMode:         {GCSettings.LatencyMode}
WorkingSet:             {(double)workingSet / (1024 * 1024):N} MB ({workingSet} bytes)
TickCount:              {tickCountString}
Stopwatch.Frequency:    {stopwatchFrequency}
TimeZoneInfo.Local.Id:  {TimeZoneInfo.Local.Id}
TempPath:               {Path.GetTempPath()}";
        return message;
    }

    private static string GetStopwatchFrequency() =>
        $"{Stopwatch.Frequency} ({Math.Round((double)Stopwatch.Frequency / TenPowerConstants.TenPower6, 2)} MHz, 1 tick = {Math.Round(StopwatchConstants.NanosecondsPerTick)} nanoseconds, 1 millisecond = {Math.Round(StopwatchConstants.TicksPerMillisecond)} ticks)";

    public static string GetCurrentDomainState()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("CurrentDomainState:\r\n");
        AppDomain appDomain = AppDomain.CurrentDomain;
        AppendAppDomainState(appDomain, stringBuilder);
        return stringBuilder.ToString();
    }

    private static void AppendAppDomainState(AppDomain appDomain, StringBuilder sb)
    {
        try
        {
            string friendlyName = appDomain.FriendlyName;
            sb.AppendFormat("FriendlyName: {0}\r\n", friendlyName);
            Assembly[] assemblies = appDomain.GetAssemblies();
            sb.AppendLine("Assemblies:");

            List<AssemblyInfo> assemblyInfos = [];

            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    Assembly assembly = assemblies[i];
                    AssemblyInfo assemblyInfo = GetAssemblyInfo(assembly);
                    assemblyInfos.Add(assemblyInfo);
                }
                catch (Exception e)
                {
                    Log.Error("{0}\t\n{1}", assemblies[i], e);
                }
            }

            assemblyInfos.Sort((info, assemblyInfo) => string.Compare(info.Name, assemblyInfo.Name, StringComparison.InvariantCulture));

            sb.Append(assemblyInfos.ToString(Columns));
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, e.ToString());
        }
    }

    private static AssemblyInfo GetAssemblyInfo(Assembly assembly)
    {
        bool isDynamic = assembly.IsDynamic;
        string location = null;
        Version fileVersion = null;
        DateTime? date = null;

        if (!isDynamic)
        {
            location = assembly.Location;
            fileVersion = GetFileVersion(location);
            date = File.GetLastWriteTime(location);
        }

        AssemblyName name = assembly.GetName();
        byte[] publicKeyToken = name.GetPublicKeyToken();
        string publicKeyTokenString = publicKeyToken != null ? Hex.GetString(publicKeyToken, false) : null;

        return new AssemblyInfo(name.Name, fileVersion, name.Version, date, publicKeyTokenString, assembly.ImageRuntimeVersion, location, isDynamic);
    }

    private static Version GetFileVersion(string fileName)
    {
        Version fileVersion = null;

        try
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);

            fileVersion = new Version(
                fileVersionInfo.FileMajorPart,
                fileVersionInfo.FileMinorPart,
                fileVersionInfo.FileBuildPart,
                fileVersionInfo.FilePrivatePart);
        }
        catch (Exception e)
        {
            Log.Trace($"exception:\r\n{e}");
        }

        return fileVersion;
    }

    private sealed class AssemblyInfo(
        string name,
        Version fileVersion,
        Version version,
        DateTime? date,
        string publicKeyToken,
        string imageRuntimeVersion,
        string location,
        bool isDynamic)
    {
        public readonly string Name = name;
        public readonly Version FileVersion = fileVersion;
        public readonly Version Version = version;
        public readonly DateTime? Date = date;
        public readonly string PublicKeyToken = publicKeyToken;
        public readonly string ImageRuntimeVersion = imageRuntimeVersion;
        public readonly string Location = location;
        public readonly bool IsDynamic = isDynamic;
    }
}