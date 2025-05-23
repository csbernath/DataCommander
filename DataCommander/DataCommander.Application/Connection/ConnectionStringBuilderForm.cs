﻿using System;
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
    private string? _selectedProviderName;
    private ConnectionInfo? _connectionInfo;
    private bool _passwordChanged;
    private readonly ReadOnlyCollection<ProviderInfo> _providers;
    private DbProviderFactory? _dbProviderFactory;
    private DataTable? _dataSources;
    private List<string>? _initialCatalogs;
    private List<OleDbProviderInfo>? _oleDbProviders;
    private readonly ColorTheme _colorTheme;

    public ConnectionStringBuilderForm(ColorTheme? colorTheme)
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

        foreach (var provider in _providers)
            providersComboBox.Items.Add(provider.Name);
    }

    public ConnectionInfo? ConnectionInfo
    {
        get => _connectionInfo;

        set
        {
            _connectionInfo = value;
            connectionNameTextBox.Text = _connectionInfo!.ConnectionName;
            var providerIdentifier = _connectionInfo.ProviderIdentifier;
            var index = _providers.IndexOf(i => i.Identifier == providerIdentifier);
            providersComboBox.SelectedIndex = index;
            var provider = ProviderFactory.CreateProvider(providerIdentifier);
            var connectionStringBuilder = provider.CreateConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = _connectionInfo.ConnectionStringAndCredential.ConnectionString;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.DataSource, out string? dataSource))
                dataSourcesComboBox.Text = dataSource;
            else if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.Host, out string? host))
                dataSourcesComboBox.Text = host;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.InitialCatalog, out string? initialCatalog))
                initialCatalogComboBox.Text = initialCatalog;

            if (connectionStringBuilder.IsKeywordSupportedAndTryGetValue(ConnectionStringKeyword.IntegratedSecurity, out bool integratedSecurity))
                integratedSecurityCheckBox.Checked = integratedSecurity;

            var credential = _connectionInfo.ConnectionStringAndCredential.Credential;
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

        using IDataReader dataReader = OleDbEnumerator.GetRootEnumerator();
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

    private void HandleProvidersComboBoxSelectedIndexChanged(object? sender, EventArgs e)
    {
        try
        {
            var index = providersComboBox.SelectedIndex;
            var providerIdentifier = _providers[index].Identifier;
            var provider = ProviderFactory.CreateProvider(providerIdentifier);
            _selectedProviderName = provider.Identifier;
            _dbProviderFactory = provider.DbProviderFactory;

            if (_dbProviderFactory is OleDbFactory oleDbFactory)
                InitializeOleDbProvidersComboBox();

            var connectionStringBuilder = provider.CreateConnectionStringBuilder();

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

    private void GetDataSources(string[]? dataSourceArray)
    {
        dataSourcesComboBox.Items.Clear();

        if (dataSourceArray != null)
            for (var i = 0; i < dataSourceArray.Length; i++)
                dataSourcesComboBox.Items.Add(dataSourceArray[i]);
    }

    private void GetDataSources(bool refresh)
    {
        var applicationData = DataCommanderApplication.Instance.ApplicationData;
        var folder = applicationData.CurrentType;
        folder = folder.CreateNode(_selectedProviderName!);
        var contains = folder.Attributes.TryGetAttributeValue("Data Sources", out string[]? dataSourceArray);

        if (!contains || refresh)
        {
            Cursor = Cursors.WaitCursor;
            var dbDataSourceEnumerator = _dbProviderFactory!.CreateDataSourceEnumerator();

            if (dbDataSourceEnumerator != null)
            {
                _dataSources = dbDataSourceEnumerator.GetDataSources();
                List<string> dataSourceList = [];

                foreach (DataRow row in _dataSources.Rows)
                {
                    var serverName = row.GetValueOrDefault<string>("ServerName");
                    var instanceName = row.GetValueOrDefault<string>("InstanceName");
                    var stringBuilder = new StringBuilder();

                    if (serverName != null)
                        stringBuilder.Append(serverName);

                    if (instanceName != null)
                    {
                        stringBuilder.Append('\\');
                        stringBuilder.Append(instanceName);
                    }

                    var dataSource = stringBuilder.ToString();
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

    private void HandleDataSourcesComboBoxSelectedIndexChanged(object? sender, EventArgs e)
    {
        var dataSource = dataSourcesComboBox.Text;
    }

    private void HandleRefreshButtonClicked(object? sender, EventArgs e)
    {
        var provider = providersComboBox.Text;

        if (provider.Length > 0)
            GetDataSources(true);
    }

    private void HandleDataSourcesComboBoxDropDown(object? sender, EventArgs e)
    {
        if (_dataSources == null)
        {
            var provider = providersComboBox.Text;

            if (provider.Length > 0)
                GetDataSources(false);
        }
    }

    private ConnectionBase CreateConnection()
    {
        var connectionInfo = SaveDialogToConnectionInfo();
        var provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);
        var connection = provider.CreateConnection(connectionInfo.ConnectionStringAndCredential);
        return connection;
    }

    private void HandleInitialCatalogComboBoxDropDown(object? sender, EventArgs e)
    {
        var dataSource = dataSourcesComboBox.Text;

        if (!string.IsNullOrWhiteSpace(dataSource) && _initialCatalogs == null)
        {
            try
            {
                using var connection = CreateConnection().Connection!;
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
                    initialCatalogComboBox.Items.Add(database);
            }
            catch (Exception ex)
            {
                DataCommanderApplication.Instance.MainForm!.StatusBar!.Items[0].Text = ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }

    private void HandleOKClicked(object? sender, EventArgs e)
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
        var providerInfo = _providers[providersComboBox.SelectedIndex];
        var provider = ProviderFactory.CreateProvider(providerInfo.Identifier);
        var connectionStringBuilder = provider.CreateConnectionStringBuilder();
        var connectionStringAndCredential = SaveDialogToConnectionStringAndCredential(provider.Identifier, connectionStringBuilder);
        var connectionName = connectionNameTextBox.Text;
        var connectionInfo = new ConnectionInfo(connectionName, providerInfo.Identifier, connectionStringAndCredential);
        return connectionInfo;
    }

    private ConnectionStringAndCredential SaveDialogToConnectionStringAndCredential(
        string providerIdentifier,
        IDbConnectionStringBuilder connectionStringBuilder)
    {
        if (providerIdentifier == ProviderIdentifier.OleDb)
        {
            var selectedIndex = oleDbProvidersComboBox.SelectedIndex;
            var oleDbProvider = _oleDbProviders![selectedIndex];
            connectionStringBuilder.SetValue("Provider", oleDbProvider.Name);
        }
        else
        {
            if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.DataSource))
                SetValue(connectionStringBuilder, ConnectionStringKeyword.DataSource, dataSourcesComboBox.Text);
            else if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.Host))
                SetValue(connectionStringBuilder, ConnectionStringKeyword.Host, dataSourcesComboBox.Text);

            if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.InitialCatalog))
                SetValue(connectionStringBuilder, ConnectionStringKeyword.InitialCatalog, initialCatalogComboBox.Text);

            if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.IntegratedSecurity))
                connectionStringBuilder.SetValue(ConnectionStringKeyword.IntegratedSecurity, integratedSecurityCheckBox.Checked);
            
            if (connectionStringBuilder.IsKeywordSupported(ConnectionStringKeyword.TrustServerCertificate))
                connectionStringBuilder.SetValue(ConnectionStringKeyword.TrustServerCertificate, trustServerCertificateCheckBox.Checked);
        }

        Credential? credential;

        if (!integratedSecurityCheckBox.Checked)
        {
            if (_passwordChanged)
            {
                var password = PasswordFactory.CreateFromPlainText(passwordTextBox.Text);
                credential = new Credential(userIdTextBox.Text, password);
            }
            else if (_connectionInfo != null)
                credential = _connectionInfo.ConnectionStringAndCredential.Credential;
            else
                credential = null;
        }
        else
            credential = null;

        return new ConnectionStringAndCredential(connectionStringBuilder.ConnectionString, credential);
    }

    private void HandleTestButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            var connectionInfo = SaveDialogToConnectionInfo();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString
            };
            dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject);
            var dataSource = (string)dataSourceObject!;
            var containsIntegratedSecurity = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurity);
            var stringBuilder = new StringBuilder();
            var providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == connectionInfo.ProviderIdentifier);            
            stringBuilder.Append($@"Connection name: {connectionInfo.ConnectionName}
Provider name: {providerInfo.Name}
{ConnectionStringKeyword.DataSource}: {dataSource}");
            if (containsIntegratedSecurity)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
            if (connectionInfo.ConnectionStringAndCredential.Credential != null)
                stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {connectionInfo.ConnectionStringAndCredential.Credential.UserId}");
            var text = stringBuilder.ToString();

            var cancelableOperationForm =
                new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1), "Opening connection...", text, _colorTheme);
            var provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);            
            using (var connection = provider.CreateConnection(connectionInfo.ConnectionStringAndCredential))
            {
                var openConnectionTask = new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken));
                cancelableOperationForm.Execute(openConnectionTask);
                if (openConnectionTask.Exception != null)
                    throw openConnectionTask.Exception;
            }

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

    private void HandleIntegratedSecurityCheckBoxCheckedChanged(object? sender, EventArgs e)
    {
        var integratedSecurity = integratedSecurityCheckBox.Checked;
        userIdTextBox.Enabled = !integratedSecurity;
        passwordTextBox.Enabled = !integratedSecurity;
    }

    private void HandlePasswordTextBoxTextChanged(object? sender, EventArgs e) => _passwordChanged = true;

    private sealed class OleDbProviderInfo(string name)
    {
        public readonly string Name = name;
    }
}