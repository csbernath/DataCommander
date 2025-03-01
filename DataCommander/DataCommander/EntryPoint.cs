using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DataCommander.Application;
//using DataCommander.Updater;
using Foundation.Configuration;
using Foundation.Data.MethodProfiler;
using Foundation.Log;
using LogLevel = Foundation.Log.LogLevel;

namespace DataCommander;

internal static class EntryPoint
{
    [STAThread]
    public static void Main()
    {
        try
        {
            //var updateStarted = Update();
            var updateStarted = false;
            if (!updateStarted)
            {
                LogFactoryReader.Read();
                MethodProfiler.BeginMethod();

                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                    Run();
                }
                finally
                {
                    LogFactory.Instance.Dispose();
                    MethodProfiler.EndMethod();
                    MethodProfiler.Close();
                }
            }
        }
        catch (Exception e)
        {
            var message = e.ToString();
            var log = LogFactory.Instance.GetCurrentMethodLog();
            log.Error(message);
            MessageBox.Show(message, "Fatal Application Error in Data Commander!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    //private static bool Update()
    //{
    //    var updateStarted = false;
    //    var random = new Random().Next(10);
    //    if (random == 0)
    //    {
    //        var updaterForm = new UpdaterForm();
    //        updaterForm.WindowState = FormWindowState.Minimized;
    //        Application.Run(updaterForm);
    //        updateStarted = updaterForm.Updater.UpdateStarted;
    //    }

    //    return updateStarted;
    //}

    private static void Run()
    {
        using var methodLog = LogFactory.Instance.GetCurrentMethodLog();
        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
        var fileName = Path.Combine(applicationDataFolderPath, "ApplicationData.xml");
        methodLog.Write(LogLevel.Trace, "fileName: {0}", fileName);
        var sectionName = Settings.SectionName;
        var dataCommanderApplication = DataCommanderApplication.Instance;
        dataCommanderApplication.LoadApplicationData(fileName, sectionName);
        dataCommanderApplication.Run();
        dataCommanderApplication.SaveApplicationData();
    }
}