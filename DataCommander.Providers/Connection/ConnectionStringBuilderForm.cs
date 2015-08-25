namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Linq;
    using Foundation;
    using Foundation.Collections;

    internal partial class ConnectionStringBuilderForm : Form
    {
        private readonly ConnectionProperties tempConnectionProperties = new ConnectionProperties();
        private ConnectionProperties connectionProperties;
        private readonly IList<string> providers;
        private DbProviderFactory dbProviderFactory;
        private DbConnectionStringBuilder dbConnectionStringBuilder;
        private DataTable dataSources;
        private List<string> initialCatalogs;
        private List<OleDbProviderInfo> oleDbProviders;

        public ConnectionStringBuilderForm()
        {
            this.InitializeComponent();

            this.oleDbProviderLabel.Visible = false;
            this.oleDbProvidersComboBox.Visible = false;

            List<string> list = ProviderFactory.Providers;
            list.Sort();
            this.providers = list;

            foreach (string provider in this.providers)
            {
                this.providersComboBox.Items.Add(provider);
            }
        }

        public ConnectionProperties ConnectionProperties
        {
            get
            {
                return this.connectionProperties;
            }

            set
            {
                this.connectionProperties = value;
                this.connectionNameTextBox.Text = this.connectionProperties.ConnectionName;
                string providerName = this.connectionProperties.ProviderName;
                this.providersComboBox.Text = providerName;
                var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = this.connectionProperties.ConnectionString;

                if (providerName == ProviderName.OleDb)
                {
                    string oleDbProviderName = dbConnectionStringBuilder.GetValue(ConnectionStringProperty.Provider);
                    this.InitializeOleDbProvidersComboBox();
                    int index = this.oleDbProviders.IndexOf(i => i.Name == oleDbProviderName);

                    this.oleDbProvidersComboBox.SelectedIndex = index;
                }

                this.dataSourcesComboBox.Text =
                    dbConnectionStringBuilder.GetValue(ConnectionStringProperty.DataSource)
                    ?? dbConnectionStringBuilder.GetValue(ConnectionStringProperty.Server)
                    ?? dbConnectionStringBuilder.GetValue(ConnectionStringProperty.Host);

                object valueObject;
                bool contains = dbConnectionStringBuilder.TryGetValue(ConnectionStringProperty.IntegratedSecurity, out valueObject);
                bool isIntegratedSecurity;

                if (contains)
                {
                    string s = (string)valueObject;
                    isIntegratedSecurity = bool.Parse(s);
                }
                else
                {
                    isIntegratedSecurity = false;
                }

                this.integratedSecurityCheckBox.Checked = isIntegratedSecurity;
                this.userIdTextBox.Text = dbConnectionStringBuilder.GetValue(ConnectionStringProperty.UserId);
                this.passwordTextBox.Text = dbConnectionStringBuilder.GetValue(ConnectionStringProperty.Password);
                this.initialCatalogComboBox.Text = dbConnectionStringBuilder.GetValue(ConnectionStringProperty.InitialCatalog) ?? dbConnectionStringBuilder.GetValue("Database");
            }
        }

        private void InitializeOleDbProvidersComboBox()
        {
            this.oleDbProviderLabel.Visible = true;
            this.oleDbProvidersComboBox.Visible = true;
            this.oleDbProviders = new List<OleDbProviderInfo>();

            using (IDataReader dataReader = OleDbEnumerator.GetRootEnumerator())
            {
                int sourceName = dataReader.GetOrdinal("SOURCES_NAME");
                int sourceDescription = dataReader.GetOrdinal("SOURCES_DESCRIPTION");

                while (dataReader.Read())
                {
                    string name = dataReader.GetString(sourceName);
                    string description = dataReader.GetString(sourceDescription);
                    var item = new OleDbProviderInfo(name, description);
                    this.oleDbProviders.Add(item);
                    this.oleDbProvidersComboBox.Items.Add(description);
                }
            }
        }

        private void providersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = this.providersComboBox.SelectedIndex;
                string providerName = this.providers[index];
                IProvider provider = ProviderFactory.CreateProvider(providerName);
                this.tempConnectionProperties.Provider = provider;
                this.dbProviderFactory = provider.DbProviderFactory;
                var oleDbFactory = this.dbProviderFactory as OleDbFactory;

                if (oleDbFactory != null)
                {
                    this.InitializeOleDbProvidersComboBox();
                }

                this.dbConnectionStringBuilder = this.dbProviderFactory.CreateConnectionStringBuilder();

                if (this.dbConnectionStringBuilder == null)
                {
                    this.dbConnectionStringBuilder = new DbConnectionStringBuilder();
                }

                bool contains = this.dbConnectionStringBuilder.ContainsKey(ConnectionStringProperty.IntegratedSecurity);
                this.integratedSecurityCheckBox.Enabled = contains;
                //contains = this.dbConnectionStringBuilder.ContainsKey(ConnectionStringProperty.InitialCatalog);
                //this.initialCatalogComboBox.Enabled = contains;
                //contains = this.dbConnectionStringBuilder.ContainsKey(ConnectionStringProperty.UserId);
                //this.userIdTextBox.Enabled = contains;

                foreach (string key in this.dbConnectionStringBuilder.Keys)
                {
                    Trace.WriteLine(key);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void GetDataSources(string[] dataSourceArray)
        {
            this.dataSourcesComboBox.Items.Clear();

            if (dataSourceArray != null)
            {
                for (int i = 0; i < dataSourceArray.Length; i++)
                {
                    this.dataSourcesComboBox.Items.Add(dataSourceArray[i]);
                }
            }
        }

        private void GetDataSources(bool refresh)
        {
            ApplicationData applicationData = DataCommanderApplication.Instance.ApplicationData;
            ConfigurationNode folder = applicationData.CurrentType;
            folder = folder.CreateNode(this.tempConnectionProperties.Provider.Name);
            string[] dataSourceArray;
            bool contains = folder.Attributes.TryGetAttributeValue("Data Sources", out dataSourceArray);

            if (!contains || refresh)
            {
                this.Cursor = Cursors.WaitCursor;
                DbDataSourceEnumerator dbDataSourceEnumerator = this.dbProviderFactory.CreateDataSourceEnumerator();

                if (dbDataSourceEnumerator != null)
                {
                    this.dataSources = dbDataSourceEnumerator.GetDataSources();
                    List<string> dataSourceList = new List<string>();

                    foreach (DataRow row in this.dataSources.Rows)
                    {
                        string serverName = Database.GetValueOrDefault<string>(row["ServerName"]);
                        string instanceName = Database.GetValueOrDefault<string>(row["InstanceName"]);
                        StringBuilder sb = new StringBuilder();

                        if (serverName != null)
                        {
                            sb.Append(serverName);
                        }

                        if (instanceName != null)
                        {
                            sb.Append('\\');
                            sb.Append(instanceName);
                        }

                        string dataSource = sb.ToString();
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
                this.GetDataSources(dataSourceArray);
                this.Cursor = Cursors.Default;
            }
            else
            {
                this.GetDataSources(dataSourceArray);
            }
        }

        private void dataSourcesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dataSource = this.dataSourcesComboBox.Text;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            string provider = this.providersComboBox.Text;

            if (provider.Length > 0)
            {
                this.GetDataSources(true);
            }
        }

        private void dataSourcesComboBox_DropDown(object sender, EventArgs e)
        {
            if (this.dataSources == null)
            {
                string provider = this.providersComboBox.Text;

                if (provider.Length > 0)
                {
                    this.GetDataSources(false);
                }
            }
        }

        private DbConnection CreateConnection()
        {
            string dataSource = this.dataSourcesComboBox.Text;
            this.SaveTo(this.dbConnectionStringBuilder);
            string connectionString = this.dbConnectionStringBuilder.ToString();
            DbConnection connection = this.dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        private void initialCatalogComboBox_DropDown(object sender, EventArgs e)
        {
            string dataSource = this.dataSourcesComboBox.Text;

            if (!string.IsNullOrWhiteSpace(dataSource) && this.initialCatalogs == null)
            {
                try
                {
                    using (DbConnection connection = this.CreateConnection())
                    {
                        connection.Open();
                        DataTable schema = connection.GetSchema("Databases");
                        this.initialCatalogs = new List<string>();

                        foreach (DataRow row in schema.Rows)
                        {
                            string database = (string)row["database_name"];
                            this.initialCatalogs.Add(database);
                        }

                        this.initialCatalogs.Sort();

                        foreach (string database in this.initialCatalogs)
                        {
                            this.initialCatalogComboBox.Items.Add(database);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DataCommanderApplication.Instance.MainForm.StatusBar.Items[0].Text = ex.Message;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.connectionProperties = new ConnectionProperties();
            this.SaveTo(this.connectionProperties);
            this.DialogResult = DialogResult.OK;
        }

        private void SaveTo(DbConnectionStringBuilder dbConnectionStringBuilder)
        {
            if (dbConnectionStringBuilder.ContainsKey(ConnectionStringProperty.DataSource))
            {
                dbConnectionStringBuilder[ConnectionStringProperty.DataSource] = this.dataSourcesComboBox.Text;
            }
            else
            {
                dbConnectionStringBuilder[ConnectionStringProperty.Server] = this.dataSourcesComboBox.Text;
            }

            var oleDbConnectionStringBuilder = dbConnectionStringBuilder as OleDbConnectionStringBuilder;
            if (oleDbConnectionStringBuilder != null)
            {
                int selectedIndex = this.oleDbProvidersComboBox.SelectedIndex;
                OleDbProviderInfo oleDbProviderInfo = this.oleDbProviders[selectedIndex];
                oleDbConnectionStringBuilder.Provider = oleDbProviderInfo.Name;
            }

            if (dbConnectionStringBuilder.ContainsKey(ConnectionStringProperty.IntegratedSecurity))
            {
                dbConnectionStringBuilder[ConnectionStringProperty.IntegratedSecurity] =
                    this.integratedSecurityCheckBox.Checked;
            }

            if (!this.userIdTextBox.Text.IsNullOrWhiteSpace())
            {
                dbConnectionStringBuilder[ConnectionStringProperty.UserId] = this.userIdTextBox.Text;
            }

            dbConnectionStringBuilder[ConnectionStringProperty.Password] = this.passwordTextBox.Text;

            if (!this.initialCatalogComboBox.Text.IsNullOrWhiteSpace())
            {
                try
                {
                    dbConnectionStringBuilder[ConnectionStringProperty.InitialCatalog] = this.initialCatalogComboBox.Text;
                }
                catch
                {
                    dbConnectionStringBuilder[ConnectionStringProperty.Database] = this.initialCatalogComboBox.Text;
                }
            }
        }

        private void SaveTo(ConnectionProperties connectionProperties)
        {
            if (this.dbConnectionStringBuilder == null)
            {
                this.dbConnectionStringBuilder = new DbConnectionStringBuilder();
            }

            this.SaveTo(this.dbConnectionStringBuilder);
            var keywords = new List<string>();

            foreach (string keyword in this.dbConnectionStringBuilder.Keys)
            {
                object obj;
                bool contains = this.dbConnectionStringBuilder.TryGetValue(keyword, out obj);

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
                    keywords.Add(keyword);
                }
            }

            foreach (string keyword in keywords)
            {
                this.dbConnectionStringBuilder.Remove(keyword);
            }

            connectionProperties.ConnectionName = this.connectionNameTextBox.Text;
            connectionProperties.ProviderName = this.providersComboBox.Text;
            connectionProperties.ConnectionString = this.dbConnectionStringBuilder.ConnectionString;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            try
            {
                var connectionProperties = new ConnectionProperties();
                this.SaveTo(connectionProperties);
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
                this.Cursor = Cursors.Default;
            }
        }

        private sealed class OleDbProviderInfo
        {
            public OleDbProviderInfo(string name, string description)
            {
                this.Name = name;
                this.Description = description;
            }

            public readonly string Name;
            private string Description;
        }
    }
}