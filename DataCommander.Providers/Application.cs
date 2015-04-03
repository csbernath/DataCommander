namespace DataCommander.Providers
{
    using System.Data.Common;
    using System.Diagnostics;
    using System.Reflection;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;
    using Microsoft.Win32;

    public sealed class Application
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly Application application = new Application();
        private readonly ApplicationData applicationData = new ApplicationData();
        private string fileName;
        private string sectionName;
        private readonly string name;
        private MainForm mainForm;

        #endregion

        private Application()
        {
            string fileName = Assembly.GetEntryAssembly().Location;
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo( fileName );
            this.name = versionInfo.ProductName;

            Settings.Section.SelectNode( null, true );

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private static void SystemEvents_SessionEnding( object sender, SessionEndingEventArgs e )
        {
            log.Write( LogLevel.Trace,  "Reason: {0}", e.Reason );
            var mainForm = application.mainForm;
            mainForm.SaveAll();
        }

        #region Public Properties

        public static Application Instance
        {
            get
            {
                return application;
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
                ConfigurationNode folder = this.applicationData.CreateNode( "DataCommander/Connections" );
                return folder;
            }
        }
        #endregion

        #region Public Methods

        public void Run()
        {
            log.Write( LogLevel.Trace,  "{0}\r\n{1}", AppDomainMonitor.EnvironmentInfo, AppDomainMonitor.CurrentDomainState );
            this.mainForm = new MainForm();
            System.Windows.Forms.Application.Run(this.mainForm );
        }

        public void SaveApplicationData()
        {
            ConfigurationNode folder = this.ConnectionsConfigurationNode;

            foreach (ConfigurationNode subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load( subFolder );

                var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
                object obj;
                bool contains = dbConnectionStringBuilder.TryGetValue( "User ID", out obj );

                if (contains)
                {
                    string password = ConnectionProperties.GetValue( dbConnectionStringBuilder, "Password" );
                    dbConnectionStringBuilder.Remove( "Password" );
                    connectionProperties.ConnectionString = dbConnectionStringBuilder.ConnectionString;
                    password = ConnectionProperties.ProtectPassword( password );
                    subFolder.Attributes.SetAttributeValue( "Password", password );
                }

                connectionProperties.Save( subFolder );
            }

            string tempFileName = this.fileName + ".temp";
            this.applicationData.Save( tempFileName, this.sectionName );
            bool succeeded = NativeMethods.MoveFileEx( tempFileName, this.fileName, NativeMethods.MoveFileExFlags.ReplaceExisiting );
            log.Write( LogLevel.Trace,  "MoveFileEx succeeded: {0}", succeeded );
        }

        public void LoadApplicationData( string fileName, string sectionName )
        {
            this.applicationData.Load( fileName, sectionName );
            ConfigurationNode folder = this.ConnectionsConfigurationNode;

            foreach (ConfigurationNode subFolder in folder.ChildNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load( subFolder );
                connectionProperties.LoadProtectedPassword( subFolder );
                connectionProperties.Save( subFolder );
            }

            this.fileName = fileName;
            this.sectionName = sectionName;
        }

        #endregion
    }
}