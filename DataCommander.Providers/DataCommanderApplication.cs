namespace DataCommander.Providers
{
    using System.Data.Common;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Win32;
    using Application = System.Windows.Forms.Application;
    using Task = System.Threading.Tasks.Task;

    public sealed class DataCommanderApplication
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly DataCommanderApplication dataCommanderApplication = new DataCommanderApplication();
        private readonly ApplicationData applicationData = new ApplicationData();
        private string fileName;
        private string sectionName;
        private readonly string name;
        private MainForm mainForm;

        #endregion

        private DataCommanderApplication()
        {
            string fileName = Assembly.GetEntryAssembly().Location;
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            this.name = versionInfo.ProductName;

            Settings.Section.SelectNode(null, true);

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            log.Write(LogLevel.Trace, "Reason: {0}", e.Reason);
            var mainForm = dataCommanderApplication.mainForm;
            mainForm.SaveAll();
        }

        #region Public Properties

        public static DataCommanderApplication Instance
        {
            get
            {
                return dataCommanderApplication;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public ApplicationData ApplicationData
        {
            get
            {
                return this.applicationData;
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        public MainForm MainForm
        {
            get
            {
                return this.mainForm;
            }
        }

        public ConfigurationNode ConnectionsConfigurationNode
        {
            get
            {
                ConfigurationNode folder = this.applicationData.CreateNode("DataCommander/Connections");
                return folder;
            }
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            Task.Run(() =>
            {
                log.Write(LogLevel.Trace, "{0}\r\n{1}", AppDomainMonitor.EnvironmentInfo, AppDomainMonitor.CurrentDomainState);
            });

            this.mainForm = new MainForm();
            Application.Run(this.mainForm);
        }

        public void SaveApplicationData()
        {
            ConfigurationNode folder = this.ConnectionsConfigurationNode;

            foreach (ConfigurationNode subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(subFolder);

                var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
                object obj;
                bool contains = dbConnectionStringBuilder.TryGetValue(ConnectionStringProperty.UserId, out obj);

                if (contains)
                {
                    string password = dbConnectionStringBuilder.GetValue(ConnectionStringProperty.Password);
                    dbConnectionStringBuilder.Remove(ConnectionStringProperty.Password);
                    connectionProperties.ConnectionString = dbConnectionStringBuilder.ConnectionString;
                    password = ConnectionProperties.ProtectPassword(password);
                    subFolder.Attributes.SetAttributeValue(ConnectionStringProperty.Password, password);
                }

                connectionProperties.Save(subFolder);
            }

            string tempFileName = this.fileName + ".temp";
            this.applicationData.Save(tempFileName, this.sectionName);
            bool succeeded = NativeMethods.MoveFileEx(tempFileName, this.fileName,
                NativeMethods.MoveFileExFlags.ReplaceExisiting);
            log.Write(LogLevel.Trace, "MoveFileEx succeeded: {0}", succeeded);
        }

        public void LoadApplicationData(string fileName, string sectionName)
        {
            this.applicationData.Load(fileName, sectionName);
            ConfigurationNode folder = this.ConnectionsConfigurationNode;

            foreach (ConfigurationNode subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(subFolder);
                connectionProperties.LoadProtectedPassword(subFolder);
                connectionProperties.Save(subFolder);
            }

            this.fileName = fileName;
            this.sectionName = sectionName;
        }

        #endregion
    }
}