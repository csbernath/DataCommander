using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Foundation.Configuration;
using Foundation.Diagnostics;
using Foundation.Log;
using Microsoft.Win32;

namespace DataCommander.Application;

public sealed class DataCommanderApplication
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private string? _sectionName;
    private readonly bool _updaterStarted = false;

    private DataCommanderApplication()
    {
        AssemblyLoadContext.Default.Resolving += Default_Resolving;

        var entryAssembly = Assembly.GetEntryAssembly()!;
        var fileName = entryAssembly.Location;
        var versionInfo = FileVersionInfo.GetVersionInfo(fileName);
        Name = versionInfo.ProductName!;

        Settings.Section.SelectNode(null, true);

        SystemEvents.SessionEnding += SystemEvents_SessionEnding;
    }

    public static DataCommanderApplication Instance { get; } = new();

    public string Name { get; }

    public ApplicationData ApplicationData { get; } = new();

    public string? ApplicationDataFileName { get; private set; }

    public MainForm? MainForm { get; private set; }

    public void Run()
    {
        if (!_updaterStarted)
        {
            MainForm = new MainForm();

            Task.Delay(1000).ContinueWith(_ =>
                Log.Write(LogLevel.Trace, "{0}\r\n{1}", AppDomainMonitor.GetEnvironmentInfo(), AppDomainMonitor.GetCurrentDomainState()));

            System.Windows.Forms.Application.Run(MainForm);
        }
    }

    public void SaveApplicationData()
    {
        var tempFileName = ApplicationDataFileName + ".temp";
        ApplicationData.Save(tempFileName, _sectionName!);
        var succeeded = NativeMethods.MoveFileEx(tempFileName, ApplicationDataFileName!, NativeMethods.MoveFileExFlags.ReplaceExisiting);
        Log.Write(LogLevel.Trace, "MoveFileEx succeeded: {0}", succeeded);
    }

    public void LoadApplicationData(string fileName, string sectionName)
    {
        ApplicationData.Load(fileName, sectionName);
        ApplicationDataFileName = fileName;
        _sectionName = sectionName;
    }

    private static Assembly Default_Resolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
    {
        var location = Assembly.GetEntryAssembly()!.Location;
        var directory = Path.GetDirectoryName(location)!;
        //var assemblyPath = Path.Combine(Environment.CurrentDirectory, $"{assemblyName.Name}.dll");
        var assemblyPath = Path.Combine(directory, $"{assemblyName.Name}.dll");
        var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyPath);
        return assembly;
    }

    private static void SystemEvents_SessionEnding(object? sender, SessionEndingEventArgs e)
    {
        Log.Write(LogLevel.Trace, "Reason: {0}", e.Reason);
        var mainForm = Instance.MainForm!;
        mainForm.SaveAll();
    }
}