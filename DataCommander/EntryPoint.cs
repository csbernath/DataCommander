namespace DataCommander
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Providers;
    using Foundation.Diagnostics.Log;
    using Foundation.Diagnostics.MethodProfiler;

    internal static class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            LogFactory.Read();
            MethodProfiler.BeginMethod();

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                var path = Path.GetTempPath();
                using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
                {
                    try
                    {
                        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
                        var fileName = applicationDataFolderPath + Path.DirectorySeparatorChar + "ApplicationData.xml";
                        methodLog.Write(LogLevel.Trace, "fileName: {0}", fileName);
                        var sectionName = Settings.SectionName;
                        var dataCommanderApplication = DataCommanderApplication.Instance;
                        dataCommanderApplication.LoadApplicationData(fileName, sectionName);
                        dataCommanderApplication.Run();
                        dataCommanderApplication.SaveApplicationData();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString(), "Fatal Application Error in Data Commander!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                LogFactory.Instance.Dispose();
                MethodProfiler.EndMethod();
                MethodProfiler.Close();
            }
        }
    }
}