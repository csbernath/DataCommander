using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application;
//using DataCommander.Updater;
using Foundation.Configuration;
using Foundation.Data.MethodProfiler;
using Foundation.InternalLog;
using Foundation.Log;
using Foundation.Windows.Forms;
using Microsoft.Win32;
using LogLevel = Foundation.Log.LogLevel;

namespace DataCommander;

internal static class EntryPoint
{
    [STAThread]
    public static void Main()
    {
        LogFactory.Set(InternalLogFactory.Instance);

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
            var text = $@"Fatal Application Error in Data Commander!

{message}";
            DataCommanderMessageBox.MessageBox.Show(text, MessageBoxCaption.Value, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    private static void Run()
    {
        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(true);
        var applicationData = new ApplicationData();
        var fileName = Path.Combine(applicationDataFolderPath, "ApplicationData.xml");
        var sectionName = Settings.SectionName;
        applicationData.Load(fileName, sectionName);
        var node = applicationData.RootNode.SelectNode("DataCommander/Application/MainForm");
        var colorMode = SystemColorMode.System;
        var initializeApplicationConfiguration = true;
        if (node != null)
        {
            var attributes = node.Attributes;
            attributes.TryGetAttributeValue("ColorMode", SystemColorMode.System, out colorMode);
            attributes.TryGetAttributeValue("InitializeApplicationConfiguration", true, out initializeApplicationConfiguration);
        }

#pragma warning disable WFO5001
        if (colorMode == SystemColorMode.System && !AppsUseLightTheme())
            colorMode = SystemColorMode.Dark;

        if (colorMode != SystemColorMode.System)
            System.Windows.Forms.Application.SetColorMode(colorMode);

        if (initializeApplicationConfiguration)
            ApplicationConfiguration.Initialize();
        
        var messageBox = colorMode != SystemColorMode.System
            ? (IMessageBox)new FoundationMessageBox()
            // ? (IMessageBox)new TestMessageBox()
            : new SystemMessageBox();
        DataCommanderMessageBox.Set(messageBox);
        
#pragma warning restore WFO5001
        
        using var methodLog = LogFactory.Instance.GetCurrentMethodLog();
        methodLog.Write(LogLevel.Trace, "fileName: {0}", fileName);
        var dataCommanderApplication = DataCommanderApplication.Instance;
        dataCommanderApplication.SetApplicationData(applicationData, fileName, sectionName);
        dataCommanderApplication.Run(colorMode);
        dataCommanderApplication.SaveApplicationData();
    }
    
    private static bool AppsUseLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is int i && i > 0;
    }    
}