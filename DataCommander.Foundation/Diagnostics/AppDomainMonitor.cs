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

        /// <summary>
        /// 
        /// </summary>
        public static String EnvironmentInfo
        {
            get
            {
                String message = String.Format( @"Environment information
Environment.MachineName:            {0}
Environment.ProcessorCount:         {1}
Environment.OSVersion:              {2}
Environment.Is64BitOperatingSystem: {3}
Environment.Is64BitProcess:         {4}
IntPtr.Size:                        {5}
Environment.Version:                {6}
Environment.UserDomainName:         {7}
Environment.UserName:               {8}
Environment.UserInteractive:        {9}
Environment.CurrentDirectory:       {10}
Environment.CommandLine:            {11},
Stopwatch.Frequency:                {12}",
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
                    Environment.Version,
                    Environment.UserDomainName,
                    Environment.UserName,
                    Environment.UserInteractive,
                    Environment.CurrentDirectory,
                    Environment.CommandLine,
                    Stopwatch.Frequency );
                return message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static String CurrentDomainState
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat( "Environment.Version: {0}, CurrentDomainState:\r\n", Environment.Version );
                AppDomain appDomain = AppDomain.CurrentDomain;
                AppendAppDomainState( appDomain, sb );
                return sb.ToString();
            }
        }

        private static Version GetFileVersion( Assembly assembly )
        {
            Version fileVersion = null;

            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo( assembly.Location );
                fileVersion = new Version( fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart );
            }
            catch (Exception e)
            {
                log.Trace( "exception:\r\n{0}", e.ToLogString() );
            }

            return fileVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="sb"></param>
        private static void AppendAppDomainState( AppDomain appDomain, StringBuilder sb )
        {
            try
            {
                String friendlyName = appDomain.FriendlyName;
                sb.AppendFormat( "FriendlyName: {0}\r\n", friendlyName );
                Assembly[] assemblies = appDomain.GetAssemblies();
                sb.AppendLine( "Assemblies:" );
                DataTable table =
                    new DataTable
                    {
                        Locale = CultureInfo.InvariantCulture
                    };
                DataColumnCollection columns = table.Columns;
                columns.Add( "Name" );
                columns.Add( "FileVersion" );
                columns.Add( "Version" );
                columns.Add( "ProcessArchitecture" );
                columns.Add( "Date" );
                columns.Add( "PublicKeyToken" );
                columns.Add( "ImageRuntimeVersion" );
                columns.Add( "GlobalAssemblyCache" );
                columns.Add( "CodeBase" );
                columns.Add( "Location" );

                for (Int32 i = 0; i < assemblies.Length; i++)
                {
                    try
                    {
                        Assembly assembly = assemblies[i];
                        String location = null;
                        try
                        {
                            location = assembly.Location;
                        }
                        catch
                        {
                        }

                        Version fileVersion = !String.IsNullOrEmpty( location ) ? GetFileVersion( assembly ) : null;
                        DateTime? date = !String.IsNullOrEmpty( location ) ? File.GetLastWriteTime( location ) : (DateTime?)null;
                        AssemblyName name = assembly.GetName();
                        String publicKeyTokenString;
                        Byte[] publicKeyToken = name.GetPublicKeyToken();

                        if (publicKeyToken != null)
                        {
                            publicKeyTokenString = Hex.GetString( publicKeyToken, false );
                        }
                        else
                        {
                            publicKeyTokenString = null;
                        }

                        Object[] values =
                        {
                            name.Name,
                            fileVersion,
                            name.Version,
                            name.ProcessorArchitecture,
                            date != null ? date.Value.ToString( "yyyy-MM-dd HH:mm:ss" ) : null,
                            publicKeyTokenString,
                            assembly.ImageRuntimeVersion,
                            assembly.GlobalAssemblyCache,
                            name.CodeBase,
                            location
                        };

                        table.Rows.Add( values );
                    }
                    catch (Exception e)
                    {
                        log.Error( "{0}\t\n{1}", assemblies[i], e.ToLogString() );
                    }
                }

                DataView view = table.DefaultView;
                view.Sort = "Name";

                sb.Append( view.ToStringTable() );
            }
            catch (Exception e)
            {
                log.Write( LogLevel.Error, e.ToLogString() );
            }
        }
    }
}