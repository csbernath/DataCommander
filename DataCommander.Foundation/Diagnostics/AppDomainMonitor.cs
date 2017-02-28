namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Text;
    using Microsoft.Win32;

    /// <summary>
    /// 
    /// </summary>
    public static class AppDomainMonitor
    {
        private static readonly ILog log = LogFactory.Instance.GetTypeLog(typeof (AppDomainMonitor));
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
                new StringTableColumnInfo<AssemblyInfo>("Date", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Date?.ToString("yyyy-MM-dd HH:mm:ss")),
                new StringTableColumnInfo<AssemblyInfo>("PublicKeyToken", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.PublicKeyToken),
                new StringTableColumnInfo<AssemblyInfo>("ImageRuntimeVersion", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.ImageRuntimeVersion),
                StringTableColumnInfo.Create<AssemblyInfo, bool>("GlobalAssemblyCache", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.GlobalAssemblyCache),
                new StringTableColumnInfo<AssemblyInfo>("CodeBase", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.CodeBase),
                new StringTableColumnInfo<AssemblyInfo>("Location", StringTableColumnAlign.Left, assemblyInfo => assemblyInfo.Location)
            };
        }

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static string DotNetFrameworkVersion
        {
            get
            {
                string dotNetFrameworkVersion;

                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                {
                    var release = (int) key.GetValue("Release");

                    switch (release)
                    {
                        case 378389:
                            dotNetFrameworkVersion = "4.5";
                            break;

                        case 378675:
                            dotNetFrameworkVersion = "4.5.1 (server)";
                            break;

                        case 378758:
                            dotNetFrameworkVersion = "4.5.1 (client)";
                            break;

                        case 379893:
                            dotNetFrameworkVersion = "4.5.2";
                            break;

                        case 394254:
                            dotNetFrameworkVersion = "4.6.1 (Windows 10)";
                            break;

                        case 394271:
                            dotNetFrameworkVersion = "4.6.1";
                            break;

                        case 394802:
                            dotNetFrameworkVersion = "4.6.2 (Windows 10 Anniversary Update)";
                            break;

                        case 394806:
                            dotNetFrameworkVersion = "4.6.2";
                            break;

                        default:
                            dotNetFrameworkVersion = null;
                            break;
                    }
                }

                return dotNetFrameworkVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string EnvironmentInfo
        {
            get
            {
                var tickCount = UniversalTime.GetTickCount();
                var milliSecondsPerDay = StopwatchTimeSpan.SecondsPerDay*1000;
                var totalDays = (double)tickCount/milliSecondsPerDay;
                var zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
                string tickCountString = $"{tickCount} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";
                var workingSet = Environment.WorkingSet;

                string message = $@"Environment information
MachineName:            {Environment.MachineName}
ProcessorCount:         {Environment.ProcessorCount}
OSVersion:              {Environment.OSVersion}
Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}
Is64BitProcess:         {Environment.Is64BitProcess}
IntPtr.Size:            {IntPtr.Size} ({IntPtr.Size*8} bit)
CLR version:            {Environment.Version}
.NET Framework version: {DotNetFrameworkVersion}
UserDomainName:         {Environment.UserDomainName}
UserName:               {Environment.UserName}
UserInteractive:        {Environment.UserInteractive}
CurrentDirectory:       {Environment.CurrentDirectory}
CommandLine:            {Environment.CommandLine},
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