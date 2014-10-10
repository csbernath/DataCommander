namespace DataCommander
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    internal static class EntryPoint
    {
        private static ILog log;

        [STAThread]
        public static void Main()
        {
            LogFactory.Read();
            log = LogFactory.Instance.GetCurrentTypeLog();
            MethodProfiler.BeginMethod();

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                string path = Path.GetTempPath();
                //string defaulgLogFileName = Path.Combine(path, "DataCommander.defaultlog");
                //DefaultLog.Add( new FileLogWriter( defaulgLogFileName, Encoding.UTF8, false, 4096, true, FileAttributes.ReadOnly | FileAttributes.Hidden ) );
                using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
                {
                    try
                    {
                        string applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath( false );
                        string fileName = applicationDataFolderPath + Path.DirectorySeparatorChar + "ApplicationData.xml";
                        methodLog.Write( LogLevel.Trace, "fileName: {0}", fileName );
                        string sectionName = Settings.SectionName;
                        DataCommander.Providers.Application application = DataCommander.Providers.Application.Instance;
                        application.LoadApplicationData( fileName, sectionName );
                        application.Run();
                        application.SaveApplicationData();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show( e.ToString(), "Fatal Application Error in Data Commander!", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                }
            }
            finally
            {
                LogFactory.Instance.Dispose();
                //DefaultLog.Close();
                MethodProfiler.EndMethod();
                MethodProfiler.Close();
            }
        }
    }
}