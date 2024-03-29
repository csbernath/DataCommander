﻿using System;
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
    private static readonly StringTableColumnInfo<AssemblyInfo>[] Columns;

    static AppDomainMonitor()
    {
        Columns = new[]
        {
            new StringTableColumnInfo<AssemblyInfo>("Name", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Name),
            StringTableColumnInfo.Create<AssemblyInfo, Version>("FileVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.FileVersion),
            StringTableColumnInfo.Create<AssemblyInfo, Version>("Version", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Version),
            StringTableColumnInfo.Create<AssemblyInfo, ProcessorArchitecture>("ProcessorArchitecture", StringTableColumnAlign.Left,
                assemblyInfo => assemblyInfo.ProcessorArchitecture),
            new StringTableColumnInfo<AssemblyInfo>("Date", StringTableColumnAlign.Left,
                assemblyInfo => assemblyInfo.Date?.ToString("yyyy-MM-dd HH:mm:ss")),
            new StringTableColumnInfo<AssemblyInfo>("PublicKeyToken", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.PublicKeyToken),
            new StringTableColumnInfo<AssemblyInfo>("ImageRuntimeVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.ImageRuntimeVersion),
            new StringTableColumnInfo<AssemblyInfo>("CodeBase", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.CodeBase),
            new StringTableColumnInfo<AssemblyInfo>("Location", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Location),
            StringTableColumnInfo.CreateLeft<AssemblyInfo, bool>("IsDynamic", i => i.IsDynamic)
        };
    }

    public static string GetEnvironmentInfo()
    {
        var tickCount = Environment.TickCount;
        var totalDays = (double)tickCount / DateTimeConstants.MillisecondsPerDay;
        var workingSet = Environment.WorkingSet;
        var windowsVersionInfo = WindowsVersionInfo.Get();
        var stopwatchFrequency = GetStopwatchFrequency();
        var zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
        var tickCountString = $"{tickCount} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";

        var message = $@"Environment information
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
UserDomainName:         {Environment.UserDomainName}
UserName:               {Environment.UserName}
UserInteractive:        {Environment.UserInteractive}
CurrentDirectory:       {Environment.CurrentDirectory}
CommandLine:            {Environment.CommandLine},
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
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("CurrentDomainState:\r\n");
        var appDomain = AppDomain.CurrentDomain;
        AppendAppDomainState(appDomain, stringBuilder);
        return stringBuilder.ToString();
    }

    private static void AppendAppDomainState(AppDomain appDomain, StringBuilder sb)
    {
        try
        {
            var friendlyName = appDomain.FriendlyName;
            sb.AppendFormat("FriendlyName: {0}\r\n", friendlyName);
            var assemblies = appDomain.GetAssemblies();
            sb.AppendLine("Assemblies:");

            var assemblyInfos = new List<AssemblyInfo>();

            for (var i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    var assembly = assemblies[i];
                    var assemblyInfo = GetAssemblyInfo(assembly);
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
        var isDynamic = assembly.IsDynamic;
        string location = null;
        Version fileVersion = null;
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

        return new AssemblyInfo(name.Name, fileVersion, name.Version, name.ProcessorArchitecture, date, publicKeyTokenString, assembly.ImageRuntimeVersion,
            name.CodeBase, location, isDynamic);
    }

    private static Version GetFileVersion(string fileName)
    {
        Version fileVersion = null;

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

    private sealed class AssemblyInfo
    {
        public readonly string Name;
        public readonly Version FileVersion;
        public readonly Version Version;
        public readonly ProcessorArchitecture ProcessorArchitecture;
        public readonly DateTime? Date;
        public readonly string PublicKeyToken;
        public readonly string ImageRuntimeVersion;
        public readonly string CodeBase;
        public readonly string Location;
        public readonly bool IsDynamic;

        public AssemblyInfo(string name, Version fileVersion, Version version, ProcessorArchitecture processorArchitecture, DateTime? date,
            string publicKeyToken, string imageRuntimeVersion, string codeBase, string location, bool isDynamic)
        {
            Name = name;
            FileVersion = fileVersion;
            Version = version;
            ProcessorArchitecture = processorArchitecture;
            Date = date;
            PublicKeyToken = publicKeyToken;
            ImageRuntimeVersion = imageRuntimeVersion;
            CodeBase = codeBase;
            Location = location;
            IsDynamic = isDynamic;
        }
    }
}