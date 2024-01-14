using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Security;
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
using Microsoft.Data.SqlClient;

namespace DataCommander.Application.Connection;

internal partial class ConnectionStringBuilderForm : Form
{
    private string _selectedProviderName;
    private ConnectionProperties _connectionProperties;
    private bool _passwordChanged;
    private readonly ReadOnlyCollection<Provider> _providers;
    private DbProviderFactory _dbProviderFactory;
    private IDbConnectionStringBuilder _dbConnectionStringBuilder;
    private DataTable _dataSources;
    private List<string> _initialCatalogs;
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

        _providers = ProviderFactory.GetProviders()
            .OrderBy(i => i.Name)
            .ToReadOnlyCollection();

        foreach (var provider in _providers)
            providersComboBox.Items.Add(provider.Name);
    }

    private static string TryGetValue(IDbConnectionStringBuilder connectionStringBuilder, string keyword)
    {
        var valueString = connectionStringBuilder.TryGetValue(keyword, out var value)
            ? (string)value
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
            var providerIdentifier = _connectionProperties.ProviderIdentifier;
            var index = _providers.IndexOf(i => i.Identifier == providerIdentifier);
            providersComboBox.SelectedIndex = index;
            var provider = ProviderFactory.CreateProvider(providerIdentifier);
            _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
            _dbConnectionStringBuilder.ConnectionString = _connectionProperties.ConnectionString;
            dataSourcesComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.DataSource);
            initialCatalogComboBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.InitialCatalog);

            if (_dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity) &&
                _dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var valueObject))
                integratedSecurityCheckBox.Checked = (bool)valueObject;

            userIdTextBox.Text = TryGetValue(_dbConnectionStringBuilder, ConnectionStringKeyword.UserId);
            passwordTextBox.Text = null;

            if (_dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate) &&
                _dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.TrustServerCertificate, out valueObject))
                trustServerCertificateCheckBox.Checked = (bool)valueObject;
        }
    }

    private void InitializeOleDbProvidersComboBox()
    {
        oleDbProviderLabel.Visible = true;
        oleDbProvidersComboBox.Visible = true;
        _oleDbProviders = [];

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
            var providerIdentifier = _providers[index].Identifier;
            var provider = ProviderFactory.CreateProvider(providerIdentifier);
            _selectedProviderName = provider.Name;
            _dbProviderFactory = provider.DbProviderFactory;

            if (_dbProviderFactory is OleDbFactory oleDbFactory)
                InitializeOleDbProvidersComboBox();

            _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
            integratedSecurityCheckBox.Enabled = _dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity);
            trustServerCertificateCheckBox.Enabled = _dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate);
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
        folder = folder.CreateNode(_selectedProviderName);
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
                    _initialCatalogs = [];

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
        _connectionProperties = CreateConnectionProperties();
        DialogResult = DialogResult.OK;
    }

    private static void SetValue(IDbConnectionStringBuilder dbConnectionStringBuilder, string keyword, string? value)
    {
        if (!value.IsNullOrWhiteSpace())
            dbConnectionStringBuilder.SetValue(keyword, value);
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
            dbConnectionStringBuilder.SetValue(ConnectionStringKeyword.IntegratedSecurity, integratedSecurityCheckBox.Checked);

        SetValue(dbConnectionStringBuilder, ConnectionStringKeyword.UserId, userIdTextBox.Text);

        if (dbConnectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate))
            dbConnectionStringBuilder.SetValue(ConnectionStringKeyword.TrustServerCertificate, trustServerCertificateCheckBox.Checked);
    }

    private ConnectionProperties CreateConnectionProperties()
    {
        var providerInfo = _providers[providersComboBox.SelectedIndex];
        var provider = ProviderFactory.CreateProvider(providerInfo.Identifier);
        _dbConnectionStringBuilder = provider.CreateConnectionStringBuilder();
        SaveTo(_dbConnectionStringBuilder);
        var connectionName = connectionNameTextBox.Text;
        byte[]? password;
        if (_passwordChanged)
        {
            password = passwordTextBox.Text.Length > 0
                ? ConnectionPropertiesRepository.ProtectPassword(passwordTextBox.Text)
                : null;
        }
        else
        {
            password = _connectionProperties.Password;
        }

        var connectionProperties =
            new ConnectionProperties(connectionName, providerInfo.Identifier, provider, _dbConnectionStringBuilder.ConnectionString, password);
        return connectionProperties;
    }

    private void testButton_Click(object sender, EventArgs e)
    {
        try
        {
            var connectionProperties = CreateConnectionProperties();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var connection = connectionProperties.Provider.CreateConnection(connectionProperties.ConnectionString, connectionProperties.GetPasswordSecureString());
            var dbConnectionStringBuilder = new DbConnectionStringBuilder();
            dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
            dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject);
            var dataSource = (string)dataSourceObject;
            var containsIntegratedSecurity = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurity);
            var containsUserId = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.UserId, out var userId);
            var provider = ProviderFactory.GetProviders().First(i => i.Identifier == connectionProperties.ProviderIdentifier);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"Connection name: {connectionProperties.ConnectionName}
Provider name: {provider.Name}
{ConnectionStringKeyword.DataSource}: {dataSource}");
            if (containsIntegratedSecurity)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
            if (containsUserId)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {userId}");
            var text = stringBuilder.ToString();

            var cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(2), "Opening connection...", text, _colorTheme);
            var openConnectionTask = new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken));
            cancelableOperationForm.Execute(openConnectionTask);
            if (openConnectionTask.Exception != null)
                throw openConnectionTask.Exception;
            MessageBox.Show("The connection was tested successfully.", DataCommanderApplication.Instance.Name, MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            var text = exception.Message;
            var caption = "Opening connection failed.";
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void integratedSecurityCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        var integratedSecurity = integratedSecurityCheckBox.Checked;
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