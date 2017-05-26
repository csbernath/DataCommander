using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using DataCommander.Providers.Connection;
using Foundation.Configuration;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;
using Task = System.Threading.Tasks.Task;

namespace DataCommander.Providers
{
    public sealed class DataCommanderApplication
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private string sectionName;

        #endregion

        private DataCommanderApplication()
        {
            var fileName = Assembly.GetEntryAssembly().Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            this.Name = versionInfo.ProductName;

            Settings.Section.SelectNode(null, true);

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            log.Write(LogLevel.Trace, "Reason: {0}", e.Reason);
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
                var folder = this.ApplicationData.CreateNode("DataCommander/Connections");
                return folder;
            }
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            this.MainForm = new MainForm();

            Task.Delay(1000).ContinueWith(task =>
            {
                log.Write(LogLevel.Trace, "{0}\r\n{1}", AppDomainMonitor.EnvironmentInfo, AppDomainMonitor.CurrentDomainState);
            });

            Application.Run(this.MainForm);
        }

        public void SaveApplicationData()
        {
            var folder = this.ConnectionsConfigurationNode;

            foreach (var subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(subFolder);

                var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
                object obj;
                var contains = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.UserId, out obj);

                if (contains)
                {
                    var password = dbConnectionStringBuilder.GetValue(ConnectionStringKeyword.Password);
                    dbConnectionStringBuilder.Remove(ConnectionStringKeyword.Password);
                    connectionProperties.ConnectionString = dbConnectionStringBuilder.ConnectionString;
                    password = ConnectionProperties.ProtectPassword(password);
                    subFolder.Attributes.SetAttributeValue(ConnectionStringKeyword.Password, password);
                }

                connectionProperties.Save(subFolder);
            }

            var tempFileName = this.FileName + ".temp";
            this.ApplicationData.Save(tempFileName, this.sectionName);
            var succeeded = NativeMethods.MoveFileEx(tempFileName, this.FileName,
                NativeMethods.MoveFileExFlags.ReplaceExisiting);
            log.Write(LogLevel.Trace, "MoveFileEx succeeded: {0}", succeeded);
        }

        public void LoadApplicationData(string fileName, string sectionName)
        {
            this.ApplicationData.Load(fileName, sectionName);

            //var writer = new ConfigurationJsonWriter();
            //writer.Write(this.ApplicationData.RootNode);

            var folder = this.ConnectionsConfigurationNode;

            foreach (var subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(subFolder);
                connectionProperties.LoadProtectedPassword(subFolder);
            }

            this.FileName = fileName;
            this.sectionName = sectionName;
        }

        #endregion
    }
}