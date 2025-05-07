using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using Foundation.Core;
using Foundation.Core.ClockAggregate;
using Foundation.Log;
using Foundation.Text;

namespace Foundation.Diagnostics;

public static class AppDomainMonitor
{
    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(AppDomainMonitor));

    private static readonly StringTableColumnInfo<AppDomainMonitor.AssemblyInfo>[] Columns =
    [
        new("Name", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Name),
        StringTableColumnInfo.Create<AppDomainMonitor.AssemblyInfo, Version?>("FileVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.FileVersion),
        StringTableColumnInfo.Create<AppDomainMonitor.AssemblyInfo, Version?>("Version", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Version),
        new("Date", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Date?.ToString("yyyy-MM-dd HH:mm:ss")),
        new("PublicKeyToken", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.PublicKeyToken),
        new("ImageRuntimeVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.ImageRuntimeVersion),
        new("Location", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Location),
        StringTableColumnInfo.CreateLeft<AppDomainMonitor.AssemblyInfo, bool>("IsDynamic", i => i.IsDynamic)
    ];

    public static string GetEnvironmentInfo()
    {
        var tickCount64 = Environment.TickCount64;
        var totalDays = (double)tickCount64 / TimeSpan.MillisecondsPerDay;
        var workingSet = Environment.WorkingSet;
        var osVersion = Environment.OSVersion;
        var windowsCurrentVersion = osVersion.Platform == PlatformID.Win32NT
            ? WindowsCurrentVersionRepository.Get()
            : null;
        var stopwatchFrequency = GetStopwatchFrequency();
        var zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
        var tickCountString = $"{tickCount64} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";
        var messageStringBuilder = new StringBuilder();

        messageStringBuilder.Append($@"Environment information
MachineName:            {Environment.MachineName}
ProcessorCount:         {Environment.ProcessorCount}
OSVersion:              {Environment.OSVersion}");

        if (windowsCurrentVersion != null)
            messageStringBuilder.Append($@"
Windows ProductName:    {windowsCurrentVersion.ProductName}
Windows DisplayVersion: {windowsCurrentVersion.DisplayVersion}
Windows ReleaseId:      {windowsCurrentVersion.ReleaseId}
Windows CurrentBuild:   {windowsCurrentVersion.CurrentBuild}");

        messageStringBuilder.Append($@"
OSArchitecture:         {RuntimeInformation.OSArchitecture}
OSDescription:          {RuntimeInformation.OSDescription}
FrameworkDescription:   {RuntimeInformation.FrameworkDescription}
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
TickCount64:            {tickCountString}
Stopwatch.Frequency:    {stopwatchFrequency}
TimeZoneInfo.Local.Id:  {TimeZoneInfo.Local.Id}
TempPath:               {Path.GetTempPath()}");
        return messageStringBuilder.ToString();
    }

    private static string GetStopwatchFrequency()
    {
        var frequency = Stopwatch.Frequency;
        var frequencyString = MeasurementUnit.ToString(frequency, 2, "Hz");
        return
            $"{frequency} ({frequencyString}, 1 tick = {Math.Round(StopwatchConstants.NanosecondsPerTick)} nanoseconds, 1 millisecond = {Math.Round(StopwatchConstants.TicksPerMillisecond)} ticks)";
    }

    public static string GetCurrentDomainState()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("CurrentDomainState:\r\n");
        var appDomain = AppDomain.CurrentDomain;
        AppendAppDomainState(appDomain, stringBuilder);
        return stringBuilder.ToString();
    }

    private static void AppendAppDomainState(AppDomain appDomain, StringBuilder stringBuilder)
    {
        try
        {
            var friendlyName = appDomain.FriendlyName;
            stringBuilder.AppendFormat("FriendlyName: {0}\r\n", friendlyName);
            var assemblies = appDomain.GetAssemblies();
            stringBuilder.AppendLine("Assemblies:");

            List<AppDomainMonitor.AssemblyInfo> assemblyInfos = [];

            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyInfo = GetAssemblyInfo(assembly);
                    assemblyInfos.Add(assemblyInfo);
                }
                catch (Exception e)
                {
                    Log.Error("{0}\t\n{1}", assembly, e);
                }
            }

            assemblyInfos.Sort((info, assemblyInfo) => string.Compare(info.Name, assemblyInfo.Name, StringComparison.InvariantCulture));

            stringBuilder.Append(assemblyInfos.ToString(Columns));
        }
        catch (Exception exception)
        {
            Log.Write(LogLevel.Error, exception.ToString());
        }
    }

    private static AppDomainMonitor.AssemblyInfo GetAssemblyInfo(Assembly assembly)
    {
        var isDynamic = assembly.IsDynamic;
        string? location = null;
        Version? fileVersion = null;
        DateTime? date = null;

        if (!isDynamic)
        {
            location = assembly.Location;
            fileVersion = GetFileVersion(location);
            date = File.GetLastWriteTime(location);
        }

        var name = assembly.GetName();
        var publicKeyToken = name.GetPublicKeyToken();
        var publicKeyTokenString = publicKeyToken != null ? Hex.GetString(publicKeyToken, false) : null;

        return new AppDomainMonitor.AssemblyInfo(name.Name, fileVersion, name.Version, date, publicKeyTokenString, assembly.ImageRuntimeVersion, location, isDynamic);
    }

    private static Version? GetFileVersion(string fileName)
    {
        Version? fileVersion = null;

        try
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);

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
        string? name,
        Version? fileVersion,
        Version? version,
        DateTime? date,
        string? publicKeyToken,
        string imageRuntimeVersion,
        string? location,
        bool isDynamic)
    {
        public readonly string? Name = name;
        public readonly Version? FileVersion = fileVersion;
        public readonly Version? Version = version;
        public readonly DateTime? Date = date;
        public readonly string? PublicKeyToken = publicKeyToken;
        public readonly string ImageRuntimeVersion = imageRuntimeVersion;
        public readonly string? Location = location;
        public readonly bool IsDynamic = isDynamic;
    }
}