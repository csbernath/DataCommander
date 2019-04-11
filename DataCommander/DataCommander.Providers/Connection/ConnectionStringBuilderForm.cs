using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using System.Windows.Forms;
using Foundation.Core;
using Foundation.Data;

namespace DataCommander.Providers.Connection
{
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
                providersComboBox.Items.Add(provider);
        }

        private static string TryGetValue(IDbConnectionStringBuilder connectionStringBuilder, string keyword)
        {
            var valueString = connectionStringBuilder.TryGetValue(keyword, out var value)
                ? (string) value
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
                dataSourcesComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.DataSource);
                initialCatalogComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog);

                if (_dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity) &&
                    _dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var valueObject))
                    integratedSecurityCheckBox.Checked = (bool) valueObject;

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
                    var item = new OleDbProviderInfo(name);
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

                if (_dbProviderFactory is OleDbFactory oleDbFactory)
                    InitializeOleDbProvidersComboBox();

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
            var contains = folder.Attributes.TryGetAttributeValue("Data Sources", out string[] dataSourceArray);

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
                            var database = (string) row["database_name"];
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

            if (passwordTextBox.Text.Length > 0)
                SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.Password, passwordTextBox.Text);
            else
                dbConnectionStringBuilder.Remove(ConnectionStringKeyword.Password);
        }

        private void SaveTo(ConnectionProperties connectionProperties)
        {
            var providerName = providersComboBox.Text;
            var provider = ProviderFactory.CreateProvider(providerName);
            _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
            SaveTo(_dbConnectionStringBuilder);

            connectionProperties.ConnectionName = connectionNameTextBox.Text;
            connectionProperties.ProviderName = providersComboBox.Text;
            connectionProperties.DataSource = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.DataSource);
            connectionProperties.InitialCatalog = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog);

            bool? integratedSecurity = null;
            if (_dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var value))
                integratedSecurity = (bool) value;

            connectionProperties.IntegratedSecurity = integratedSecurity;
            connectionProperties.UserId = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.UserId);

            if (_dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.Password, out value))
            {
                var password = (string) value;
                connectionProperties.Password = password.Length > 0
                    ? new Option<string>(password)
                    : null;
            }
            else
                connectionProperties.Password = null;

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
                    MessageBox.Show("The connection was tested successfully.");
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
            public readonly string Name;
            public OleDbProviderInfo(string name) => Name = name;
        }
    }
}