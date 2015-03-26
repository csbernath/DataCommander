using System.Runtime.Remoting.Messaging;

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

    /// <summary>
    /// 
    /// </summary>
    public static class AppDomainMonitor
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static string DotNetFrameworkVersion
        {
            get
            {
                string dotNetFrameworkVersion;

                switch (Environment.Version.ToString())
                {
                    case "4.0.30319.18063":
                        dotNetFrameworkVersion = "4.5";
                        break;

                    case "4.0.30319.34209":
                        dotNetFrameworkVersion = "4.5.2";
                        break;                        

                    default:
                        dotNetFrameworkVersion = null;
                        break;
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
                string message = string.Format(@"Environment information
Environment.MachineName:            {0}
Environment.ProcessorCount:         {1}
Environment.OSVersion:              {2}
Environment.Is64BitOperatingSystem: {3}
Environment.Is64BitProcess:         {4}
IntPtr.Size:                        {5} ({6} bit)
CLR version:                        {7}
.NET Framework version:             {8}
Environment.UserDomainName:         {9}
Environment.UserName:               {10}
Environment.UserInteractive:        {11}
Environment.CurrentDirectory:       {12}
Environment.CommandLine:            {13},
Stopwatch.Frequency:                {14}",
                    Environment.MachineName,
                    Environment.ProcessorCount,
                    Environment.OSVersion,
#if FOUNDATION_3_5
                    "?",
                    "?",
#else
                    Environment.Is64BitOperatingSystem,
                    Environment.Is64BitProcess,
#endif
                    IntPtr.Size,
                    IntPtr.Size * 8,
                    Environment.Version,
                    DotNetFrameworkVersion,
                    Environment.UserDomainName,
                    Environment.UserName,
                    Environment.UserInteractive,
                    Environment.CurrentDirectory,
                    Environment.CommandLine,
                    Stopwatch.Frequency);
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
                fileVersion = new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
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
                        DateTime? date = !string.IsNullOrEmpty(location) ? File.GetLastWriteTime(location) : (DateTime?)null;
                        AssemblyName name = assembly.GetName();
                        string publicKeyTokenString;
                        Byte[] publicKeyToken = name.GetPublicKeyToken();

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