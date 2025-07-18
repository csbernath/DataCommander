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
using Microsoft.Win32;
using LogLevel = Foundation.Log.LogLevel;

namespace DataCommander;

internal static class EntryPoint
{
    [STAThread]
    public static void Main()
    {
#pragma warning disable WFO5001
        var colorMode = SystemColorMode.System;
        // if (!ApplicationData.CurrentType.Attributes.TryGetAttributeValue("ColorMode", out _colorMode))
        //     _colorMode = SystemColorMode.System;

        if (colorMode == SystemColorMode.System && !AppsUseLightTheme())
            colorMode = SystemColorMode.Dark;

        if (colorMode != SystemColorMode.System)
            System.Windows.Forms.Application.SetColorMode(colorMode);
#pragma warning restore WFO5001
        
        ApplicationConfiguration.Initialize();
        
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
                    Run(colorMode);
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

#pragma warning disable WFO5001    
    private static void Run(SystemColorMode colorMode)
    {
        using var methodLog = LogFactory.Instance.GetCurrentMethodLog();
        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
        var fileName = Path.Combine(applicationDataFolderPath, "ApplicationData.xml");
        methodLog.Write(LogLevel.Trace, "fileName: {0}", fileName);
        var sectionName = Settings.SectionName;
        var dataCommanderApplication = DataCommanderApplication.Instance;
        dataCommanderApplication.LoadApplicationData(fileName, sectionName);
        dataCommanderApplication.Run(colorMode);
        dataCommanderApplication.SaveApplicationData();
    }
#pragma warning restore WFO5001    
    
    private static bool AppsUseLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is int i && i > 0;
    }    
}