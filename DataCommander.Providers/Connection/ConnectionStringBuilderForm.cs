namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Linq;

    internal partial class ConnectionStringBuilderForm : Form
    {
        private ConnectionProperties tempConnectionProperties = new ConnectionProperties();
        private ConnectionProperties connectionProperties;
        private IList<string> providers;
        private DbProviderFactory dbProviderFactory;
        private DbConnectionStringBuilder dbConnectionStringBuilder;
        private DataTable dataSources;
        private List<string> initialCatalogs;
        private List<OleDbProviderInfo> oleDbProviders;

        public ConnectionStringBuilderForm()
        {
            InitializeComponent();

            this.oleDbProviderLabel.Visible = false;
            this.oleDbProvidersComboBox.Visible = false;

            List<string> list = ProviderFactory.Providers;
            list.Sort();
            providers = list;

            foreach (string provider in providers)
            {
                providersComboBox.Items.Add( provider );
            }
        }

        public ConnectionProperties ConnectionProperties
        {
            get
            {
                return connectionProperties;
            }

            set
            {
                connectionProperties = value;
                connectionNameTextBox.Text = connectionProperties.connectionName;
                string providerName = connectionProperties.providerName;
                providersComboBox.Text = providerName;
                DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = connectionProperties.connectionString;

                if (providerName == ProviderName.OleDb)
                {
                    string oleDbProviderName = ConnectionProperties.GetValue( dbConnectionStringBuilder, ConnectionStringProperty.Provider );
                    this.InitializeOleDbProvidersComboBox();
                    int index = this.oleDbProviders.IndexOf( 0, i => i.Name == oleDbProviderName );
                    this.oleDbProvidersComboBox.SelectedIndex = index;
                }

                dataSourcesComboBox.Text = ConnectionProperties.GetValue( dbConnectionStringBuilder, ConnectionStringProperty.DataSource );

                object valueObject;
                bool contains = dbConnectionStringBuilder.TryGetValue( ConnectionStringProperty.IntegratedSecurity, out valueObject );
                bool isIntegratedSecurity;

                if (contains)
                {
                    string s = (string) valueObject;
                    isIntegratedSecurity = bool.Parse( s );
                }
                else
                {
                    isIntegratedSecurity = false;
                }

                integratedSecurityCheckBox.Checked = isIntegratedSecurity;
                userIdTextBox.Text = ConnectionProperties.GetValue( dbConnectionStringBuilder, ConnectionStringProperty.UserId );
                passwordTextBox.Text = ConnectionProperties.GetValue( dbConnectionStringBuilder, ConnectionStringProperty.Password );
                initialCatalogComboBox.Text = ConnectionProperties.GetValue( dbConnectionStringBuilder, "Initial Catalog" );
            }
        }

        private void InitializeOleDbProvidersComboBox()
        {
            this.oleDbProviderLabel.Visible = true;
            this.oleDbProvidersComboBox.Visible = true;
            this.oleDbProviders = new List<OleDbProviderInfo>();

            using (IDataReader dataReader = OleDbEnumerator.GetRootEnumerator())
            {
                while (dataReader.Read())
                {
                    string name = dataReader.GetValue<string>( "SOURCES_NAME" );
                    string description = dataReader.GetValue<string>( "SOURCES_DESCRIPTION" );
                    OleDbProviderInfo item = new OleDbProviderInfo( name, description );
                    this.oleDbProviders.Add( item );
                    this.oleDbProvidersComboBox.Items.Add( description );
                }
            }
        }

        private void providersComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            try
            {
                int index = providersComboBox.SelectedIndex;
                string providerName = providers[ index ];
                IProvider provider = ProviderFactory.CreateProvider( providerName );
                tempConnectionProperties.provider = provider;
                this.dbProviderFactory = provider.DbProviderFactory;
                OleDbFactory oleDbFactory = this.dbProviderFactory as OleDbFactory;

                if (oleDbFactory != null)
                {
                    this.InitializeOleDbProvidersComboBox();
                }

                this.dbConnectionStringBuilder = this.dbProviderFactory.CreateConnectionStringBuilder();

                if (this.dbConnectionStringBuilder == null)
                {
                    this.dbConnectionStringBuilder = new DbConnectionStringBuilder();
                }

                bool contains = dbConnectionStringBuilder.ContainsKey( ConnectionStringProperty.IntegratedSecurity );
                integratedSecurityCheckBox.Enabled = contains;
                contains = dbConnectionStringBuilder.ContainsKey( ConnectionStringProperty.InitialCatalog );
                initialCatalogComboBox.Enabled = contains;
                contains = dbConnectionStringBuilder.ContainsKey( ConnectionStringProperty.UserId );
                this.userIdTextBox.Enabled = contains;

                foreach (string key in dbConnectionStringBuilder.Keys)
                {
                    System.Diagnostics.Trace.WriteLine( key );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( this, ex.ToString() );
            }
        }

        private void GetDataSources( string[] dataSourceArray )
        {
            dataSourcesComboBox.Items.Clear();

            if (dataSourceArray != null)
            {
                for (int i = 0; i < dataSourceArray.Length; i++)
                {
                    dataSourcesComboBox.Items.Add( dataSourceArray[ i ] );
                }
            }
        }

        private void GetDataSources( bool refresh )
        {
            ApplicationData applicationData = Application.Instance.ApplicationData;
            ConfigurationNode folder = applicationData.CurrentType;
            folder = folder.CreateNode( tempConnectionProperties.provider.Name );
            string[] dataSourceArray;
            bool contains = folder.Attributes.TryGetAttributeValue<string[]>( "Data Sources", out dataSourceArray );

            if (!contains || refresh)
            {
                Cursor = Cursors.WaitCursor;
                DbDataSourceEnumerator dbDataSourceEnumerator = dbProviderFactory.CreateDataSourceEnumerator();

                if (dbDataSourceEnumerator != null)
                {
                    dataSources = dbDataSourceEnumerator.GetDataSources();
                    List<string> dataSourceList = new List<string>();

                    foreach (DataRow row in dataSources.Rows)
                    {
                        string serverName = Database.GetValueOrDefault<string>( row[ "ServerName" ] );
                        string instanceName = Database.GetValueOrDefault<string>( row[ "InstanceName" ] );
                        StringBuilder sb = new StringBuilder();

                        if (serverName != null)
                        {
                            sb.Append( serverName );
                        }

                        if (instanceName != null)
                        {
                            sb.Append( '\\' );
                            sb.Append( instanceName );
                        }

                        string dataSource = sb.ToString();
                        dataSourceList.Add( dataSource );
                    }

                    dataSourceList.Sort();
                    dataSourceArray = dataSourceList.ToArray();
                }
                else
                {
                    dataSourceArray = null;
                }

                folder.Attributes.SetAttributeValue( "Data Sources", dataSourceArray );
                GetDataSources( dataSourceArray );
                Cursor = Cursors.Default;
            }
            else
            {
                GetDataSources( dataSourceArray );
            }
        }

        private void dataSourcesComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            string dataSource = dataSourcesComboBox.Text;
        }

        private void refreshButton_Click( object sender, EventArgs e )
        {
            string provider = providersComboBox.Text;

            if (provider.Length > 0)
            {
                this.GetDataSources( true );
            }
        }

        private void dataSourcesComboBox_DropDown( object sender, EventArgs e )
        {
            if (dataSources == null)
            {
                string provider = providersComboBox.Text;

                if (provider.Length > 0)
                {
                    this.GetDataSources( false );
                }
            }
        }

        private DbConnection CreateConnection()
        {
            string dataSource = dataSourcesComboBox.Text;
            SaveTo( dbConnectionStringBuilder );
            string connectionString = dbConnectionStringBuilder.ToString();
            DbConnection connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        private void initialCatalogComboBox_DropDown( object sender, EventArgs e )
        {
            string dataSource = dataSourcesComboBox.Text;

            if (!string.IsNullOrWhiteSpace( dataSource ) && initialCatalogs == null)
            {
                try
                {
                    using (DbConnection connection = CreateConnection())
                    {
                        connection.Open();
                        DataTable schema = connection.GetSchema( "Databases" );
                        initialCatalogs = new List<string>();

                        foreach (DataRow row in schema.Rows)
                        {
                            string database = (string) row[ "database_name" ];
                            initialCatalogs.Add( database );
                        }

                        initialCatalogs.Sort();

                        foreach (string database in initialCatalogs)
                        {
                            initialCatalogComboBox.Items.Add( database );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Application.Instance.MainForm.StatusBar.Items[ 0 ].Text = ex.Message;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void OK_Click( object sender, EventArgs e )
        {
            connectionProperties = new ConnectionProperties();
            this.SaveTo( connectionProperties );
            DialogResult = DialogResult.OK;
        }

        private void SaveTo( DbConnectionStringBuilder dbConnectionStringBuilder )
        {
            dbConnectionStringBuilder[ ConnectionStringProperty.DataSource ] = dataSourcesComboBox.Text;

            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = dbConnectionStringBuilder as OleDbConnectionStringBuilder;

            if (oleDbConnectionStringBuilder != null)
            {
                int selectedIndex = this.oleDbProvidersComboBox.SelectedIndex;
                OleDbProviderInfo oleDbProviderInfo = this.oleDbProviders[ selectedIndex ];
                oleDbConnectionStringBuilder.Provider = oleDbProviderInfo.Name;
            }

            if (dbConnectionStringBuilder.ContainsKey( ConnectionStringProperty.IntegratedSecurity ))
            {
                dbConnectionStringBuilder[ ConnectionStringProperty.IntegratedSecurity ] = integratedSecurityCheckBox.Checked;
            }

            if (dbConnectionStringBuilder.ContainsKey( ConnectionStringProperty.UserId ))
            {
                dbConnectionStringBuilder[ ConnectionStringProperty.UserId ] = this.userIdTextBox.Text;
            }

            dbConnectionStringBuilder[ ConnectionStringProperty.Password ] = passwordTextBox.Text;

            if (dbConnectionStringBuilder.ContainsKey( "Initial Catalog" ))
            {
                dbConnectionStringBuilder[ "Initial Catalog" ] = initialCatalogComboBox.Text;
            }
        }

        private void SaveTo( ConnectionProperties connectionProperties )
        {
            if (dbConnectionStringBuilder == null)
            {
                dbConnectionStringBuilder = new DbConnectionStringBuilder();
            }

            this.SaveTo( dbConnectionStringBuilder );
            List<string> keywords = new List<string>();

            foreach (string keyword in dbConnectionStringBuilder.Keys)
            {
                object obj;
                bool contains = dbConnectionStringBuilder.TryGetValue( keyword, out obj );

                if (contains && obj != null)
                {
                    string s = obj.ToString();
                    if (s.Length == 0)
                    {
                        contains = false;
                    }
                }

                if (!contains)
                {
                    keywords.Add( keyword );
                }
            }

            foreach (string keyword in keywords)
            {
                dbConnectionStringBuilder.Remove( keyword );
            }

            connectionProperties.connectionName = connectionNameTextBox.Text;
            connectionProperties.providerName = providersComboBox.Text;
            connectionProperties.connectionString = dbConnectionStringBuilder.ConnectionString;
        }

        private void testButton_Click( object sender, EventArgs e )
        {
            try
            {
                ConnectionProperties connectionProperties = new ConnectionProperties();
                SaveTo( connectionProperties );
                OpenConnectionForm form = new OpenConnectionForm( connectionProperties );

                if (form.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show( "The connection was tested successfully." );
                }
            }
            catch ( Exception ex )
            {
                Application.Instance.MainForm.StatusBar.Items[ 0 ].Text = ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private sealed class OleDbProviderInfo
        {
            public OleDbProviderInfo( string name, string description )
            {
                this.Name = name;
                this.Description = description;
            }

            public string Name;
            public string Description;
        }
    }

    public static class ConnectionStringProperty
    {
        public const string DataSource = "Data Source";
        public const string InitialCatalog = "Initial Catalog";
        public const string UserId = "User ID";
        public const string Password = "Password";
        public const string Provider = "Provider";
        public const string IntegratedSecurity = "Integrated Security";
    }
}