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

                string path = Path.GetTempPath();
                using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
                {
                    try
                    {
                        string applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
                        string fileName = applicationDataFolderPath + Path.DirectorySeparatorChar + "ApplicationData.xml";
                        methodLog.Write(LogLevel.Trace, "fileName: {0}", fileName);
                        string sectionName = Settings.SectionName;
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