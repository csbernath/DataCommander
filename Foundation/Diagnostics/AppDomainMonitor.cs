using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Text;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;
using Microsoft.Win32;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppDomainMonitor
    {
        private static readonly ILog log = LogFactory.Instance.GetTypeLog(typeof(AppDomainMonitor));
        private static readonly StringTableColumnInfo<AssemblyInfo>[] columns;

        static AppDomainMonitor()
        {
            columns = new StringTableColumnInfo<AssemblyInfo>[]
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
                StringTableColumnInfo.Create<AssemblyInfo, bool>("GlobalAssemblyCache", StringTableColumnAlign.Left,
                    assemblyInfo => assemblyInfo.GlobalAssemblyCache),
                new StringTableColumnInfo<AssemblyInfo>("CodeBase", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.CodeBase),
                new StringTableColumnInfo<AssemblyInfo>("Location", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Location)
            };
        }

        #region Public Properties

        public static string GetWindowsVersion()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("ProductName:{0},ReleaseId:{1},CurrentBuild:{2}",
                    key.GetValue("ProductName"),
                    key.GetValue("ReleaseId"),
                    key.GetValue("CurrentBuild"));

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetDotNetFrameworkRelease()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                var release = (int)key.GetValue("Release");
                return release;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        public static string GetDotNetFrameworkVersion(int release)
        {
            string version;

            switch (release)
            {
                #region 4.5

                case 378389:
                    version = "4.5";
                    break;

                case 378675:
                    version = "4.5.1 (server)";
                    break;

                case 378758:
                    version = "4.5.1 (client)";
                    break;

                case 379893:
                    version = "4.5.2";
                    break;

                #endregion

                #region 4.6

                case 394254:
                    version = "4.6.1 (Windows 10)";
                    break;

                case 394271:
                    version = "4.6.1";
                    break;

                case 394802:
                    version = "4.6.2 (Windows 10 Anniversary Update)";
                    break;

                case 394806:
                    version = "4.6.2";
                    break;

                #endregion

                #region 4.7

                case 460798:
                    version = "4.7 (Windows 10 Creators Update)";
                    break;

                case 460805:
                    version = "4.7";
                    break;

                case 461308:
                    version = "4.7.1 (Windows 10 Fall Creators Update 1709)";
                    break;

                #endregion

                default:
                    version = null;
                    break;
            }

            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string EnvironmentInfo
        {
            get
            {
                var tickCount = UniversalTime.GetTickCount();
                var milliSecondsPerDay = DateTimeConstants.SecondsPerDay * 1000;
                var totalDays = (double)tickCount/milliSecondsPerDay;
                var zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
                var tickCountString = $"{tickCount} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";
                var workingSet = Environment.WorkingSet;
                var dotNetFrameworkRelease = GetDotNetFrameworkRelease();
                var dotNetFrameworkVersion = GetDotNetFrameworkVersion(dotNetFrameworkRelease);

                var message = $@"Environment information
MachineName:            {Environment.MachineName}
ProcessorCount:         {Environment.ProcessorCount}
OSVersion:              {Environment.OSVersion}
Windows version:        {GetWindowsVersion()}
Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}
Is64BitProcess:         {Environment.Is64BitProcess}
IntPtr.Size:            {IntPtr.Size} ({IntPtr.Size*8} bit)
CLR version:            {Environment.Version}
.NET Framework release: {dotNetFrameworkRelease}
.NET Framework version: {dotNetFrameworkVersion}
UserDomainName:         {Environment.UserDomainName}
UserName:               {Environment.UserName}
UserInteractive:        {Environment.UserInteractive}
CurrentDirectory:       {Environment.CurrentDirectory}
CommandLine:            {Environment.CommandLine},
GCSettings.IsServerGC:  {GCSettings.IsServerGC}
GCSettings.LargeObjectHeapCompactionMode: {GCSettings.LargeObjectHeapCompactionMode}
GCSettings.LatencyMode: {GCSettings.LatencyMode}
WorkingSet:             {(double)workingSet/(1024*1024):N} MB ({workingSet} bytes)
TickCount:              {tickCountString}
Stopwatch.Frequency:    {Stopwatch.Frequency}";
                return message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrentDomainState
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("CurrentDomainState:\r\n");
                var appDomain = AppDomain.CurrentDomain;
                AppendAppDomainState(appDomain, stringBuilder);
                return stringBuilder.ToString();
            }
        }

        #endregion

        #region Private Methods

        private static Version GetFileVersion(Assembly assembly)
        {
            Version fileVersion = null;

            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

                fileVersion = new Version(
                    fileVersionInfo.FileMajorPart,
                    fileVersionInfo.FileMinorPart,
                    fileVersionInfo.FileBuildPart,
                    fileVersionInfo.FilePrivatePart);
            }
            catch (Exception e)
            {
                log.Trace("exception:\r\n{0}", e.ToLogString());
            }

            return fileVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="sb"></param>
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
                        log.Error("{0}\t\n{1}", assemblies[i], e.ToLogString());
                    }
                }

                assemblyInfos.Sort((info, assemblyInfo) => string.Compare(info.Name, assemblyInfo.Name, StringComparison.InvariantCulture));

                sb.Append(assemblyInfos.ToString(columns));
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        #endregion

        private static AssemblyInfo GetAssemblyInfo(Assembly assembly)
        {
            string location = null;
            try
            {
                location = assembly.Location;
            }
            catch
            {
            }

            var fileVersion = !string.IsNullOrEmpty(location) ? GetFileVersion(assembly) : null;
            var date = !string.IsNullOrEmpty(location)
                ? File.GetLastWriteTime(location)
                : (DateTime?)null;

            var name = assembly.GetName();

            var publicKeyToken = name.GetPublicKeyToken();
            var publicKeyTokenString = publicKeyToken != null ? Hex.GetString(publicKeyToken, false) : null;

            return new AssemblyInfo
            {
                Name = name.Name,
                FileVersion = fileVersion,
                Version = name.Version,
                ProcessorArchitecture = name.ProcessorArchitecture,
                Date = date,
                PublicKeyToken = publicKeyTokenString,
                ImageRuntimeVersion = assembly.ImageRuntimeVersion,
                GlobalAssemblyCache = assembly.GlobalAssemblyCache,
                CodeBase = name.CodeBase,
                Location = location
            };
        }

        private sealed class AssemblyInfo
        {
            public string Name;
            public Version FileVersion;
            public Version Version;
            public ProcessorArchitecture ProcessorArchitecture;
            public DateTime? Date;
            public string PublicKeyToken;
            public string ImageRuntimeVersion;
            public bool GlobalAssemblyCache;
            public string CodeBase;
            public string Location;
        }
    }
}