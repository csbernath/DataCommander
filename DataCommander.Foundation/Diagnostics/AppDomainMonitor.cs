namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
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
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();

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
                    int release = (int)key.GetValue("Release");

                    switch (release)
                    {
                        case 378389:
                            dotNetFrameworkVersion = "4.5";
                            break;

                        case 378675:
                        case 378758:
                            dotNetFrameworkVersion = "4.5.1";
                            break;

                        case 379893:
                            dotNetFrameworkVersion = "4.5.2";
                            break;

                        case 394254:
                            dotNetFrameworkVersion = "4.6.1 (Windows 10 November Update)";
                            break;

                        case 394271:
                            dotNetFrameworkVersion = "4.6.1";
                            break;

                        default:
                            dotNetFrameworkVersion = null;
                            break;
                    }
                }

                //switch (Environment.Version.ToString())
                //{
                //    case "4.0.30319.18063":
                //        dotNetFrameworkVersion = "4.5";
                //        break;

                //    case "4.0.30319.34209":
                //        dotNetFrameworkVersion = "4.5.2";
                //        break;

                //    case "4.0.30319.42000":
                //        dotNetFrameworkVersion = "4.6";
                //        break;

                //    default:
                //        dotNetFrameworkVersion = null;
                //        break;
                //}

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
                int tickCount = UniversalTime.GetTickCount();
                int milliSecondsPerDay = StopwatchTimeSpan.SecondsPerDay*1000;
                double totalDays = (double)tickCount/milliSecondsPerDay;
                DateTime zeroDateTime = LocalTime.Default.Now.AddDays(-totalDays);
                string tickCountString = $"{tickCount} ({totalDays:N2} days(s) from {zeroDateTime:yyyy.MM.dd HH:mm:ss})";

                string message =
                    $@"Environment information
Environment.MachineName:            {Environment.MachineName}
Environment.ProcessorCount:         {
                        Environment.ProcessorCount}
Environment.OSVersion:              {Environment.OSVersion}
Environment.Is64BitOperatingSystem: {
                        Environment.Is64BitOperatingSystem}
Environment.Is64BitProcess:         {Environment.Is64BitProcess
                        }
IntPtr.Size:                        {IntPtr.Size} ({IntPtr.Size*8} bit)
CLR version:                        {Environment.Version
                        }
.NET Framework version:             {DotNetFrameworkVersion}
Environment.UserDomainName:         {Environment.UserDomainName
                        }
Environment.UserName:               {Environment.UserName}
Environment.UserInteractive:        {Environment.UserInteractive
                        }
Environment.CurrentDirectory:       {Environment.CurrentDirectory}
Environment.CommandLine:            {Environment.CommandLine
                        },
Environment.TickCount:              {tickCountString}
Stopwatch.Frequency:                {Stopwatch.Frequency}";
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
                var sb = new StringBuilder();
                sb.Append("CurrentDomainState:\r\n");
                AppDomain appDomain = AppDomain.CurrentDomain;
                AppendAppDomainState(appDomain, sb);
                return sb.ToString();
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
                fileVersion = new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                    fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
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
                string friendlyName = appDomain.FriendlyName;
                sb.AppendFormat("FriendlyName: {0}\r\n", friendlyName);
                var assemblies = appDomain.GetAssemblies();
                sb.AppendLine("Assemblies:");
                var table = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };
                var columns = table.Columns;
                columns.Add("Name");
                columns.Add("FileVersion");
                columns.Add("Version");
                columns.Add("ProcessArchitecture");
                columns.Add("Date");
                columns.Add("PublicKeyToken");
                columns.Add("ImageRuntimeVersion");
                columns.Add("GlobalAssemblyCache");
                columns.Add("CodeBase");
                columns.Add("Location");

                for (int i = 0; i < assemblies.Length; i++)
                {
                    try
                    {
                        Assembly assembly = assemblies[i];
                        string location = null;
                        try
                        {
                            location = assembly.Location;
                        }
                        catch
                        {
                        }

                        Version fileVersion = !string.IsNullOrEmpty(location) ? GetFileVersion(assembly) : null;
                        DateTime? date = !string.IsNullOrEmpty(location)
                            ? File.GetLastWriteTime(location)
                            : (DateTime?) null;
                        AssemblyName name = assembly.GetName();
                        string publicKeyTokenString;
                        byte[] publicKeyToken = name.GetPublicKeyToken();

                        if (publicKeyToken != null)
                        {
                            publicKeyTokenString = Hex.GetString(publicKeyToken, false);
                        }
                        else
                        {
                            publicKeyTokenString = null;
                        }

                        object[] values =
                        {
                            name.Name,
                            fileVersion,
                            name.Version,
                            name.ProcessorArchitecture,
                            date != null ? date.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                            publicKeyTokenString,
                            assembly.ImageRuntimeVersion,
                            assembly.GlobalAssemblyCache,
                            name.CodeBase,
                            location
                        };

                        table.Rows.Add(values);
                    }
                    catch (Exception e)
                    {
                        log.Error("{0}\t\n{1}", assemblies[i], e.ToLogString());
                    }
                }

                DataView view = table.DefaultView;
                view.Sort = "Name";

                sb.Append(view.ToStringTable());
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        #endregion
    }
}