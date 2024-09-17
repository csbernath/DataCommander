using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Linq;

namespace DataCommander.Application.Connection;

internal partial class ConnectionStringBuilderForm : Form
{
    private string _selectedProviderName;
    private ConnectionInfo _connectionInfo;
    private bool _passwordChanged;
    private readonly ReadOnlyCollection<ProviderInfo> _providers;
    private DbProviderFactory _dbProviderFactory;
    private DataTable _dataSources;
    private List<string>? _initialCatalogs;
    private List<OleDbProviderInfo> _oleDbProviders;
    private readonly ColorTheme _colorTheme;

    public ConnectionStringBuilderForm(ColorTheme colorTheme)
    {
        _colorTheme = colorTheme;

        InitializeComponent();

        oleDbProviderLabel.Visible = false;
        oleDbProvidersComboBox.Visible = false;

        if (colorTheme != null)
            colorTheme.Apply(this);

        _providers = ProviderInfoRepository.GetProviderInfos()
            .OrderBy(i => i.Name)
            .ToReadOnlyCollection();

        foreach (ProviderInfo provider in _providers)
            providersComboBox.Items.Add(provider.Name);
    }

    public ConnectionInfo ConnectionInfo
    {
        get => _connectionInfo;

        set
        {
            _connectionInfo = value;
            connectionNameTextBox.Text = _connectionInfo.ConnectionName;
            string providerIdentifier = _connectionInfo.ProviderIdentifier;
            int index = _providers.IndexOf(i => i.Identifier == providerIdentifier);
            providersComboBox.SelectedIndex = index;
            IProvider provider = ProviderFactory.CreateProvider(providerIdentifier);
            IDbConnectionStringBuilder connectionStringBuilder = provider.CreateConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = _connectionInfo.ConnectionStringAndCredential.ConnectionString;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.DataSource, out string? dataSource))
                dataSourcesComboBox.Text = dataSource;
            else if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.Host, out string? host))
                dataSourcesComboBox.Text = host;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.InitialCatalog, out string? initialCatalog))
                initialCatalogComboBox.Text = initialCatalog;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.IntegratedSecurity, out bool integratedSecurity))
                integratedSecurityCheckBox.Checked = integratedSecurity;

            Credential? credential = _connectionInfo.ConnectionStringAndCredential.Credential;
            if (credential != null)
                userIdTextBox.Text = credential.UserId;

            passwordTextBox.Text = null;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.TrustServerCertificate, out bool trustServerCertificate))
                trustServerCertificateCheckBox.Checked = trustServerCertificate;
        }
    }

    private void InitializeOleDbProvidersComboBox()
    {
        oleDbProviderLabel.Visible = true;
        oleDbProvidersComboBox.Visible = true;
        _oleDbProviders = [];

        using (IDataReader dataReader = OleDbEnumerator.GetRootEnumerator())
        {
            int sourceName = dataReader.GetOrdinal("SOURCES_NAME");
            int sourceDescription = dataReader.GetOrdinal("SOURCES_DESCRIPTION");

            while (dataReader.Read())
            {
                string name = dataReader.GetString(sourceName);
                string description = dataReader.GetString(sourceDescription);
                OleDbProviderInfo item = new OleDbProviderInfo(name);
                _oleDbProviders.Add(item);
                oleDbProvidersComboBox.Items.Add(description);
            }
        }
    }

    private void providersComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int index = providersComboBox.SelectedIndex;
            string providerIdentifier = _providers[index].Identifier;
            IProvider provider = ProviderFactory.CreateProvider(providerIdentifier);
            _selectedProviderName = provider.Identifier;
            _dbProviderFactory = provider.DbProviderFactory;

            if (_dbProviderFactory is OleDbFactory oleDbFactory)
                InitializeOleDbProvidersComboBox();

            IDbConnectionStringBuilder connectionStringBuilder = provider.CreateConnectionStringBuilder();

            if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.Host))
                dataSourceLabel.Text = $"{ConnectionStringKeyword.Host}:";
            
            integratedSecurityCheckBox.Enabled = connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity);
            trustServerCertificateCheckBox.Enabled = connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate);
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
            for (int i = 0; i < dataSourceArray.Length; i++)
                dataSourcesComboBox.Items.Add(dataSourceArray[i]);
    }

    private void GetDataSources(bool refresh)
    {
        Foundation.Configuration.ApplicationData applicationData = DataCommanderApplication.Instance.ApplicationData;
        Foundation.Configuration.ConfigurationNode folder = applicationData.CurrentType;
        folder = folder.CreateNode(_selectedProviderName);
        bool contains = folder.Attributes.TryGetAttributeValue("Data Sources", out string[] dataSourceArray);

        if (!contains || refresh)
        {
            Cursor = Cursors.WaitCursor;
            DbDataSourceEnumerator? dbDataSourceEnumerator = _dbProviderFactory.CreateDataSourceEnumerator();

            if (dbDataSourceEnumerator != null)
            {
                _dataSources = dbDataSourceEnumerator.GetDataSources();
                List<string> dataSourceList = [];

                foreach (DataRow row in _dataSources.Rows)
                {
                    string serverName = row.GetValueOrDefault<string>("ServerName");
                    string instanceName = row.GetValueOrDefault<string>("InstanceName");
                    StringBuilder stringBuilder = new StringBuilder();

                    if (serverName != null)
                        stringBuilder.Append(serverName);

                    if (instanceName != null)
                    {
                        stringBuilder.Append('\\');
                        stringBuilder.Append(instanceName);
                    }

                    string dataSource = stringBuilder.ToString();
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
        string dataSource = dataSourcesComboBox.Text;
    }

    private void refreshButton_Click(object sender, EventArgs e)
    {
        string provider = providersComboBox.Text;

        if (provider.Length > 0)
            GetDataSources(true);
    }

    private void dataSourcesComboBox_DropDown(object sender, EventArgs e)
    {
        if (_dataSources == null)
        {
            string provider = providersComboBox.Text;

            if (provider.Length > 0)
                GetDataSources(false);
        }
    }

    private ConnectionBase CreateConnection()
    {
        ConnectionInfo connectionInfo = SaveDialogToConnectionInfo();
        IProvider provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);
        ConnectionBase connection = provider.CreateConnection(connectionInfo.ConnectionStringAndCredential);
        return connection;
    }

    private void initialCatalogComboBox_DropDown(object sender, EventArgs e)
    {
        string dataSource = dataSourcesComboBox.Text;

        if (!string.IsNullOrWhiteSpace(dataSource) && _initialCatalogs == null)
        {
            try
            {
                using (DbConnection connection = CreateConnection().Connection)
                {
                    connection.Open();
                    DataTable schema = connection.GetSchema("Databases");
                    _initialCatalogs = [];

                    foreach (DataRow row in schema.Rows)
                    {
                        string database = (string)row["database_name"];
                        _initialCatalogs.Add(database);
                    }

                    _initialCatalogs.Sort();

                    foreach (string database in _initialCatalogs) 
                        initialCatalogComboBox.Items.Add(database);
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
        _connectionInfo = SaveDialogToConnectionInfo();
        DialogResult = DialogResult.OK;
    }

    private static void SetValue(IDbConnectionStringBuilder dbConnectionStringBuilder, string keyword, string? value)
    {
        if (!value.IsNullOrWhiteSpace())
            dbConnectionStringBuilder.SetValue(keyword, value);
    }

    private ConnectionInfo SaveDialogToConnectionInfo()
    {
        ProviderInfo providerInfo = _providers[providersComboBox.SelectedIndex];
        IProvider provider = ProviderFactory.CreateProvider(providerInfo.Identifier);
        IDbConnectionStringBuilder connectionStringBuilder = provider.CreateConnectionStringBuilder();
        ConnectionStringAndCredential connectionStringAndCredential = SaveDialogToConnectionStringAndCredential(connectionStringBuilder);
        string connectionName = connectionNameTextBox.Text;
        ConnectionInfo connectionInfo = new ConnectionInfo(connectionName, providerInfo.Identifier, connectionStringAndCredential);
        return connectionInfo;
    }

    private ConnectionStringAndCredential SaveDialogToConnectionStringAndCredential(IDbConnectionStringBuilder dbConnectionStringBuilder)
    {
        if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.DataSource))
            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.DataSource, dataSourcesComboBox.Text);
        else if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.Host))
            SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.Host, dataSourcesComboBox.Text);

        SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog, initialCatalogComboBox.Text);

        //var oleDbConnectionStringBuilder = dbConnectionStringBuilder as OleDbConnectionStringBuilder;
        //if (oleDbConnectionStringBuilder != null)
        //{
        //    int selectedIndex = this.oleDbProvidersComboBox.SelectedIndex;
        //    OleDbProviderInfo oleDbProviderInfo = this.oleDbProviders[selectedIndex];
        //    oleDbConnectionStringBuilder.Provider = oleDbProviderInfo.Name;
        //}

        if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity))
            dbConnectionStringBuilder.SetValue(ConnectionStringKeyword.IntegratedSecurity, integratedSecurityCheckBox.Checked);

        Credential? credential;

        if (!integratedSecurityCheckBox.Checked)
        {
            if (_passwordChanged)
            {
                Password password = PasswordFactory.CreateFromPlainText(passwordTextBox.Text);
                credential = new Credential(userIdTextBox.Text, password);
            }
            else if (_connectionInfo != null)
                credential = _connectionInfo.ConnectionStringAndCredential.Credential;
            else
                credential = null;
        }
        else
            credential = null;

        if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate))
            dbConnectionStringBuilder.SetValue(ConnectionStringKeyword.TrustServerCertificate, trustServerCertificateCheckBox.Checked);

        return new ConnectionStringAndCredential(dbConnectionStringBuilder.ConnectionString, credential);
    }

    private void testButton_Click(object sender, EventArgs e)
    {
        try
        {
            ConnectionInfo connectionInfo = SaveDialogToConnectionInfo();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString
            };
            dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out object? dataSourceObject);
            string? dataSource = (string)dataSourceObject;
            bool containsIntegratedSecurity = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out object? integratedSecurity);
            StringBuilder stringBuilder = new StringBuilder();
            ProviderInfo providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == connectionInfo.ProviderIdentifier);            
            stringBuilder.Append($@"Connection name: {connectionInfo.ConnectionName}
Provider name: {providerInfo.Name}
{ConnectionStringKeyword.DataSource}: {dataSource}");
            if (containsIntegratedSecurity)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
            if (connectionInfo.ConnectionStringAndCredential.Credential != null)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {connectionInfo.ConnectionStringAndCredential.Credential.UserId}");
            string text = stringBuilder.ToString();

            CancelableOperationForm cancelableOperationForm =
                new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1), "Opening connection...", text, _colorTheme);
            IProvider provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);            
            using (ConnectionBase connection = provider.CreateConnection(connectionInfo.ConnectionStringAndCredential))
            {
                Task openConnectionTask = new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken));
                cancelableOperationForm.Execute(openConnectionTask);
                if (openConnectionTask.Exception != null)
                    throw openConnectionTask.Exception;
            }

            MessageBox.Show("The connection was tested successfully.", DataCommanderApplication.Instance.Name, MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            string text = exception.Message;
            string caption = "Opening connection failed.";
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void integratedSecurityCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        bool integratedSecurity = integratedSecurityCheckBox.Checked;
        userIdTextBox.Enabled = !integratedSecurity;
        passwordTextBox.Enabled = !integratedSecurity;
    }

    private void passwordTextBox_TextChanged(object sender, EventArgs e)
    {
        _passwordChanged = true;
    }

    private sealed class OleDbProviderInfo(string name)
    {
        public readonly string Name = name;
    }
}