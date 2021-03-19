using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Foundation.Configuration;
using Foundation.Diagnostics;
using Foundation.Log;
using Microsoft.Win32;

namespace DataCommander.Providers
{
    public sealed class DataCommanderApplication
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private string _sectionName;
        private bool _updaterStarted = false;

        #endregion

        private DataCommanderApplication()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var fileName = entryAssembly.Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            Name = versionInfo.ProductName;

            Settings.Section.SelectNode(null, true);

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Log.Write(LogLevel.Trace, "Reason: {0}", e.Reason);
            var mainForm = Instance.MainForm;
            mainForm.SaveAll();
        }

        #region Public Properties

        public static DataCommanderApplication Instance { get; } = new DataCommanderApplication();

        public string Name { get; }

        public ApplicationData ApplicationData { get; } = new ApplicationData();

        public string FileName { get; private set; }

        public MainForm MainForm { get; private set; }

        public ConfigurationNode ConnectionsConfigurationNode
        {
            get
            {
                var folder = ApplicationData.CreateNode("DataCommander/Connections");
                return folder;
            }
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            if (!_updaterStarted)
            {
                MainForm = new MainForm();

                Task.Delay(1000).ContinueWith(task =>
                {
                    Log.Write(LogLevel.Trace, "{0}\r\n{1}", AppDomainMonitor.GetEnvironmentInfo(), AppDomainMonitor.GetCurrentDomainState());
                });

                Application.Run(MainForm);
            }
        }

        public void SaveApplicationData()
        {
            var tempFileName = FileName + ".temp";
            ApplicationData.Save(tempFileName, _sectionName);
            var succeeded = NativeMethods.MoveFileEx(tempFileName, FileName,
                NativeMethods.MoveFileExFlags.ReplaceExisiting);
            Log.Write(LogLevel.Trace, "MoveFileEx succeeded: {0}", succeeded);
        }

        public void LoadApplicationData(string fileName, string sectionName)
        {
            ApplicationData.Load(fileName, sectionName);
            FileName = fileName;
            _sectionName = sectionName;
        }

        #endregion
    }
}