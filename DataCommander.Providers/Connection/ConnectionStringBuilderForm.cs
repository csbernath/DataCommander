namespace DataCommander.Providers.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Text;
    using System.Windows.Forms;
    using Foundation;
    using Foundation.Data;

    internal partial class ConnectionStringBuilderForm : Form
    {
        private readonly ConnectionProperties _tempConnectionProperties = new ConnectionProperties();
        private ConnectionProperties _connectionProperties;
        private readonly IList<string> _providers;
        private DbProviderFactory _dbProviderFactory;
        private IDbConnectionStringBuilder _dbConnectionStringBuilder;
        private DataTable _dataSources;
        private List<string> _initialCatalogs;
        private List<OleDbProviderInfo> _oleDbProviders;

        public ConnectionStringBuilderForm()
        {
            InitializeComponent();

            oleDbProviderLabel.Visible = false;
            oleDbProvidersComboBox.Visible = false;

            var list = ProviderFactory.Providers;
            list.Sort();
            _providers = list;

            foreach (var provider in _providers)
            {
                providersComboBox.Items.Add(provider);
            }
        }

        private static string TryGetValue(IDbConnectionStringBuilder connectionStringBuilder, string keyword)
        {
            object value;

            string valueString = connectionStringBuilder.TryGetValue(keyword, out value)
                ? valueString = (string)value
                : null;

            return valueString;
        }

        public ConnectionProperties ConnectionProperties
        {
            get => _connectionProperties;

            set
            {
                _connectionProperties = value;
                connectionNameTextBox.Text = _connectionProperties.ConnectionName;
                var providerName = _connectionProperties.ProviderName;
                providersComboBox.Text = providerName;
                var provider = ProviderFactory.CreateProvider(providerName);
                _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
                _dbConnectionStringBuilder.ConnectionString = _connectionProperties.ConnectionString;

                //if (providerName == ProviderName.OleDb)
                //{
                //    string oleDbProviderName = TryGetValue(ConnectionStringKeywor.Provider);
                //    this.InitializeOleDbProvidersComboBox();
                //    int index = this.oleDbProviders.IndexOf(i => i.Name == oleDbProviderName);

                //    this.oleDbProvidersComboBox.SelectedIndex = index;
                //}

                dataSourcesComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.DataSource);
                initialCatalogComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog);

                if (_dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity))
                {
                    object valueObject;
                    if (_dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out valueObject))
                    {
                        integratedSecurityCheckBox.Checked = (bool)valueObject;
                    }
                }

                userIdTextBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.UserId);
                passwordTextBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.Password);
            }
        }

        private void InitializeOleDbProvidersComboBox()
        {
            oleDbProviderLabel.Visible = true;
            oleDbProvidersComboBox.Visible = true;
            _oleDbProviders = new List<OleDbProviderInfo>();

            using (IDataReader dataReader = OleDbEnumerator.GetRootEnumerator())
            {
                var sourceName = dataReader.GetOrdinal("SOURCES_NAME");
                var sourceDescription = dataReader.GetOrdinal("SOURCES_DESCRIPTION");

                while (dataReader.Read())
                {
                    var name = dataReader.GetString(sourceName);
                    var description = dataReader.GetString(sourceDescription);
                    var item = new OleDbProviderInfo(name, description);
                    _oleDbProviders.Add(item);
                    oleDbProvidersComboBox.Items.Add(description);
                }
            }
        }

        private void providersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var index = providersComboBox.SelectedIndex;
                var providerName = _providers[index];
                var provider = ProviderFactory.CreateProvider(providerName);
                _tempConnectionProperties.Provider = provider;
                _dbProviderFactory = provider.DbProviderFactory;
                var oleDbFactory = _dbProviderFactory as OleDbFactory;

                if (oleDbFactory != null)
                {
                    InitializeOleDbProvidersComboBox();
                }

                _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
                integratedSecurityCheckBox.Enabled = _dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void GetDataSources(string[] dataSourceArray)
        {
            dataSourcesComboBox.Items.Clear();

            if (dataSourceArray != null)
            {
                for (var i = 0; i < dataSourceArray.Length; i++)
                {
                    dataSourcesComboBox.Items.Add(dataSourceArray[i]);
                }
            }
        }

        private void GetDataSources(bool refresh)
        {
            var applicationData = DataCommanderApplication.Instance.ApplicationData;
            var folder = applicationData.CurrentType;
            folder = folder.CreateNode(_tempConnectionProperties.Provider.Name);
            string[] dataSourceArray;
            var contains = folder.Attributes.TryGetAttributeValue("Data Sources", out dataSourceArray);

            if (!contains || refresh)
            {
                Cursor = Cursors.WaitCursor;
                var dbDataSourceEnumerator = _dbProviderFactory.CreateDataSourceEnumerator();

                if (dbDataSourceEnumerator != null)
                {
                    _dataSources = dbDataSourceEnumerator.GetDataSources();
                    var dataSourceList = new List<string>();

                    foreach (DataRow row in _dataSources.Rows)
                    {
                        var serverName = row.GetValueOrDefault<string>("ServerName");
                        var instanceName = row.GetValueOrDefault<string>("InstanceName");
                        var sb = new StringBuilder();

                        if (serverName != null)
                        {
                            sb.Append(serverName);
                        }

                        if (instanceName != null)
                        {
                            sb.Append('\\');
                            sb.Append(instanceName);
                        }

                        var dataSource = sb.ToString();
                        dataSourceList.Add(dataSource);
                    }

                    dataSourceList.Sort();
                    dataSourceArray = dataSourceList.ToArray();
                }
                else
                {
                    dataSourceArray = null;
                }

                folder.Attributes.SetAttributeValue("Data Sources", dataSourceArray);
                GetDataSources(dataSourceArray);
                Cursor = Cursors.Default;
            }
            else
            {
                GetDataSources(dataSourceArray);
            }
        }

        private void dataSourcesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dataSource = dataSourcesComboBox.Text;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            var provider = providersComboBox.Text;

            if (provider.Length > 0)
            {
                GetDataSources(true);
            }
        }

        private void dataSourcesComboBox_DropDown(object sender, EventArgs e)
        {
            if (_dataSources == null)
            {
                var provider = providersComboBox.Text;

                if (provider.Length > 0)
                {
                    GetDataSources(false);
                }
            }
        }

        private DbConnection CreateConnection()
        {
            var dataSource = dataSourcesComboBox.Text;
            SaveTo(_dbConnectionStringBuilder);
            var connection = _dbProviderFactory.CreateConnection();
            connection.ConnectionString = _dbConnectionStringBuilder.ConnectionString;
            return connection;
        }

        private void initialCatalogComboBox_DropDown(object sender, EventArgs e)
        {
            var dataSource = dataSourcesComboBox.Text;

            if (!string.IsNullOrWhiteSpace(dataSource) && _initialCatalogs == null)
            {
                try
                {
                    using (var connection = CreateConnection())
                    {
                        connection.Open();
                        var schema = connection.GetSchema("Databases");
                        _initialCatalogs = new List<string>();

                        foreach (DataRow row in schema.Rows)
                        {
                            var database = (string)row["database_name"];
                            _initialCatalogs.Add(database);
                        }

                        _initialCatalogs.Sort();

                        foreach (var database in _initialCatalogs)
                        {
                            initialCatalogComboBox.Items.Add(database);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DataCommanderApplication.Instance.MainForm.StatusBar.Items[0].Text = ex.Message;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            _connectionProperties = new ConnectionProperties();
            SaveTo(_connectionProperties);
            DialogResult = DialogResult.OK;
        }

        private static void SetValue(IDbConnectionStringBuilder dbConnectionStringBuilder, string keyword, string value)
        {
            if (!value.IsNullOrWhiteSpace())
            {
                dbConnectionStringBuilder.SetValue(keyword, value);
            }
        }

        private void SaveTo(IDbConnectionStringBuilder dbConnectionStringBuilder)
        {
            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.DataSource, dataSourcesComboBox.Text);
            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog, initialCatalogComboBox.Text);

            //var oleDbConnectionStringBuilder = dbConnectionStringBuilder as OleDbConnectionStringBuilder;
            //if (oleDbConnectionStringBuilder != null)
            //{
            //    int selectedIndex = this.oleDbProvidersComboBox.SelectedIndex;
            //    OleDbProviderInfo oleDbProviderInfo = this.oleDbProviders[selectedIndex];
            //    oleDbConnectionStringBuilder.Provider = oleDbProviderInfo.Name;
            //}

            if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity))
            {
                dbConnectionStringBuilder.SetValue(ConnectionStringKeyword.IntegratedSecurity, integratedSecurityCheckBox.Checked);
            }

            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.UserId, userIdTextBox.Text);
            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.Password, passwordTextBox.Text);
        }

        private void SaveTo(ConnectionProperties connectionProperties)
        {
            var providerName = providersComboBox.Text;
            var provider = ProviderFactory.CreateProvider(providerName);
            _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
            SaveTo(_dbConnectionStringBuilder);

            //var keywords = new List<string>();

            //foreach (string keyword in this.dbConnectionStringBuilder.Keys)
            //{
            //    object obj;
            //    bool contains = this.dbConnectionStringBuilder.TryGetValue(keyword, out obj);

            //    if (contains && obj != null)
            //    {
            //        string s = obj.ToString();
            //        if (s.Length == 0)
            //        {
            //            contains = false;
            //        }
            //    }

            //    if (!contains)
            //    {
            //        keywords.Add(keyword);
            //    }
            //}

            //foreach (string keyword in keywords)
            //{
            //    this.dbConnectionStringBuilder.Remove(keyword);
            //}

            connectionProperties.ConnectionName = connectionNameTextBox.Text;
            connectionProperties.ProviderName = providersComboBox.Text;
            connectionProperties.DataSource = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.DataSource);
            connectionProperties.InitialCatalog = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog);

            bool? integratedSecurity = null;
            object value;
            if (_dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out value))
            {
                integratedSecurity = (bool)value;
            }

            connectionProperties.IntegratedSecurity = integratedSecurity;
            connectionProperties.UserId = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.UserId);

            connectionProperties.ConnectionString = _dbConnectionStringBuilder.ConnectionString;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            try
            {
                var connectionProperties = new ConnectionProperties();
                SaveTo(connectionProperties);
                var form = new OpenConnectionForm(connectionProperties);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("The connection was tested successfully.");
                }
            }
            catch (Exception ex)
            {
                DataCommanderApplication.Instance.MainForm.StatusBar.Items[0].Text = ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private sealed class OleDbProviderInfo
        {
            public OleDbProviderInfo(string name, string description)
            {
                Name = name;
                _description = description;
            }

            public readonly string Name;
            private string _description;
        }
    }
}