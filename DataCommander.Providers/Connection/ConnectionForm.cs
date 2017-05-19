namespace DataCommander.Providers.Connection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml;
    using Foundation.Configuration;
    using Foundation.Diagnostics;
    using Foundation.Diagnostics.Log;
    using Foundation.Linq;
    using Foundation.Windows.Forms;
    using ResultWriter;

    internal sealed class ConnectionForm : Form
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private Button btnOK;
        private DoubleBufferedDataGridView dataGrid;
        private Button btnCancel;
        private Button newButton;
        private StatusStrip statusBar;
        private readonly DataTable dataTable = new DataTable();
        private bool isDirty;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = new Container();

        public ConnectionForm(StatusStrip statusBar, ColorTheme colorTheme)
        {
            this.statusBar = statusBar;
            this.InitializeComponent();

            this.dataTable.Columns.Add("ConnectionName", typeof(string));
            this.dataTable.Columns.Add("ProviderName", typeof(string));
            this.dataTable.Columns.Add(ConnectionStringKeyword.DataSource);
            this.dataTable.Columns.Add(ConnectionStringKeyword.InitialCatalog);
            this.dataTable.Columns.Add(ConnectionStringKeyword.IntegratedSecurity);
            this.dataTable.Columns.Add(ConnectionStringKeyword.UserId);
            this.dataTable.Columns.Add("Persist Security Info");
            this.dataTable.Columns.Add("Enlist");
            this.dataTable.Columns.Add("Pooling");
            this.dataTable.Columns.Add("Driver");
            this.dataTable.Columns.Add("DBQ");
            this.dataTable.Columns.Add("Unicode");
            this.dataTable.Columns.Add("Extended Properties");
            this.dataTable.Columns.Add("Naming");

            var folder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;

            foreach (var subFolder in folder.ChildNodes)
            {
                var dataRow = this.dataTable.NewRow();
                this.LoadConnection(subFolder, dataRow);
                this.dataTable.Rows.Add(dataRow);
            }

            var graphics = this.CreateGraphics();

            foreach (DataColumn column in this.dataTable.Columns)
            {
                var dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                var columnName = column.ColumnName;
                dataGridViewTextBoxColumn.DataPropertyName = columnName;
                dataGridViewTextBoxColumn.HeaderText = columnName;

                if (columnName == "ConnectionName" ||
                    columnName == ConnectionStringKeyword.DataSource ||
                    columnName == ConnectionStringKeyword.InitialCatalog ||
                    columnName == ConnectionStringKeyword.UserId)
                {
                    float dataRowMaxWidth;

                    if (this.dataTable.Rows.Count > 0)
                    {
                        IEnumerable<DataRow> enumerableDataRow = this.dataTable.Rows.Cast<DataRow>();
                        var dataRowSelector = new DataRowSelector(column, graphics, this.Font);
                        var enumerableDataRowWidth = enumerableDataRow.Select(dataRowSelector.GetWidth);
                        dataRowMaxWidth = enumerableDataRowWidth.Max();
                    }
                    else
                    {
                        dataRowMaxWidth = 95;
                    }

                    //columnStyle.Width = (int)dataRowMaxWidth;
                    //dataGridViewTextBoxColumn.Width = (int)dataRowMaxWidth;
                }

                //this.dataGrid.Columns.Add(dataGridViewTextBoxColumn);
            }

            this.dataGrid.DataSource = this.dataTable;
            if (colorTheme != null)
            {
                this.BackColor = colorTheme.BackColor;
                this.ForeColor = colorTheme.ForeColor;

                ColorThemeApplyer.Apply(this.dataGrid, colorTheme);
            }
        }

        public ConnectionProperties ConnectionProperties { get; private set; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (this.components != null)
                    this.components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.dataGrid = new DoubleBufferedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.Location = new System.Drawing.Point(402, 637);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 24);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&Connect";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(490, 637);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newButton.Location = new System.Drawing.Point(12, 637);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 24);
            this.newButton.TabIndex = 8;
            this.newButton.Text = "&New";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGrid.Location = new System.Drawing.Point(8, 8);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.PublicDoubleBuffered = true;
            this.dataGrid.ReadOnly = true;
            this.dataGrid.Size = new System.Drawing.Size(944, 621);
            this.dataGrid.TabIndex = 6;
            this.dataGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGrid_UserDeletingRow);
            this.dataGrid.DoubleClick += new System.EventHandler(this.dataGrid_DoubleClick);
            this.dataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyDown);
            this.dataGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseClick);
            // 
            // ConnectionForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(954, 668);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to database";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public long Duration { get; private set; }

        private void LoadConnection(ConfigurationNode folder, DataRow row)
        {
            var connectionProperties = new ConnectionProperties();
            connectionProperties.Load(folder);
            row["ConnectionName"] = connectionProperties.ConnectionName;
            row["ProviderName"] = connectionProperties.ProviderName;
            row[ConnectionStringKeyword.DataSource] = connectionProperties.DataSource;
            row[ConnectionStringKeyword.InitialCatalog] = connectionProperties.InitialCatalog;
            row[ConnectionStringKeyword.IntegratedSecurity] = connectionProperties.IntegratedSecurity;
            row[ConnectionStringKeyword.UserId] = connectionProperties.UserId;

            //var provider = ProviderFactory.CreateProvider(connectionProperties.ProviderName);
            //var connectionStringBuilder = provider.CreateConnectionStringBuilder();
            //connectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;

            //foreach (DataColumn dataColumn in this.dataTable.Columns.Cast<DataColumn>().Skip(2))
            //{
            //    object value;
            //    if (connectionStringBuilder.TryGetValue(dataColumn.ColumnName, out value))
            //    {
            //        row[dataColumn.ColumnName] = value;
            //    }
            //}
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var folder = this.SelectedConfigurationNode;
            this.Connect(folder);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            var folder = this.SelectedConfigurationNode;
            this.Connect(folder);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.Formatting = Formatting.Indented;

            foreach (var node in this.SelectedConfigurationNodes)
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(node);
                ConfigurationWriter.WriteNode(xmlTextWriter, node);
            }

            var s = stringWriter.ToString();
            Clipboard.SetText(s);
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            try
            {
                var s = Clipboard.GetText();
                var stringReader = new StringReader(s);
                var xmlTextReader = new XmlTextReader(stringReader);
                var configurationReader = new ConfigurationReader();
                var propertyFolder = configurationReader.Read(xmlTextReader);
                propertyFolder.Write(TraceWriter.Instance);
                IEnumerable<ConfigurationNode> configurationNodes;

                if (propertyFolder.ChildNodes.Count > 0)
                {
                    configurationNodes = propertyFolder.ChildNodes;
                }
                else
                {
                    configurationNodes = new ConfigurationNode[] {propertyFolder};
                }

                foreach (var configurationNode in configurationNodes)
                {
                    var connectionProperties = new ConnectionProperties();
                    connectionProperties.Load(configurationNode);
                    this.Add(connectionProperties);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Delete()
        {
            if (MessageBox.Show(this, "Do you want to delete the selected item(s)?", DataCommanderApplication.Instance.Name, MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                var connectionsFolder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
                var index = this.SelectedIndex;
                var selectedFolder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(selectedFolder);
                this.dataTable.Rows.RemoveAt(index);
                this.isDirty = true;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            this.Delete();
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            var folder = this.SelectedConfigurationNode;
            var connectionProperties = new ConnectionProperties();
            connectionProperties.Load(folder);
            var form = new ConnectionStringBuilderForm();
            form.ConnectionProperties = connectionProperties;
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                connectionProperties = form.ConnectionProperties;
                connectionProperties.Save(folder);
                var row = this.dataTable.DefaultView[this.dataGrid.CurrentCell.RowIndex].Row;
                this.LoadConnection(folder, row);
            }
        }

        private void MoveDown()
        {
            var index = this.SelectedIndex;
            var connectionsFolder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;

            if (index < connectionsFolder.ChildNodes.Count - 1)
            {
                var folder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(folder);
                connectionsFolder.InsertChildNode(index + 1, folder);

                this.dataTable.Rows.RemoveAt(index);
                var row = this.dataTable.NewRow();
                this.LoadConnection(folder, row);
                this.dataTable.Rows.InsertAt(row, index + 1);
                this.dataGrid.CurrentCell = this.dataGrid[0, index + 1];
            }
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            this.MoveDown();
        }

        private void MoveUp()
        {
            var index = this.SelectedIndex;

            if (index > 0)
            {
                var connectionsFolder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
                var folder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(folder);
                connectionsFolder.InsertChildNode(index - 1, folder);

                this.dataTable.Rows.RemoveAt(index);
                var row = this.dataTable.NewRow();
                this.LoadConnection(folder, row);
                this.dataTable.Rows.InsertAt(row, index - 1);
                this.dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                this.dataGrid.CurrentCell = this.dataGrid[0, index - 1];
            }
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            this.MoveUp();
        }

        private void dataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = this.dataGrid.HitTest(e.X, e.Y);
                var rowIndex = hitTestInfo.RowIndex;
                var contextMenu = new ContextMenuStrip(this.components);
                ToolStripMenuItem menuItem;

                if (rowIndex >= 0)
                {
                    this.dataGrid.CurrentCell = this.dataGrid[0, rowIndex];
                    menuItem = new ToolStripMenuItem("&Edit", null, this.Edit_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("C&onnect", null, this.Connect_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("&Copy", null, this.Copy_Click);
                    contextMenu.Items.Add(menuItem);
                }

                menuItem = new ToolStripMenuItem("&Paste", null, this.Paste_Click);
                contextMenu.Items.Add(menuItem);

                if (rowIndex >= 0)
                {
                    menuItem = new ToolStripMenuItem("&Delete", null, this.Delete_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("Move &up", null, this.MoveUp_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("Move &down", null, this.MoveDown_Click);
                    contextMenu.Items.Add(menuItem);
                }

                contextMenu.Show(this, e.Location);
            }
        }

        private int SelectedIndex
        {
            get
            {
                var index = this.dataGrid.CurrentCell.RowIndex;

                if (index >= 0)
                {
                    var dataView = this.dataTable.DefaultView;
                    var rowView = dataView[index];
                    var row = rowView.Row;
                    index = this.dataTable.Rows.IndexOf(row);
                }

                return index;
            }
        }

        private IEnumerable<int> SelectedIndexes
        {
            get
            {
                var count = this.dataTable.Rows.Count;
                var dataView = this.dataTable.DefaultView;
                var selectedCount = 0;

                foreach (DataGridViewRow dataGridViewRow in this.dataGrid.SelectedRows)
                {
                    var dataRowView = (DataRowView)dataGridViewRow.DataBoundItem;
                    var row = dataRowView.Row;
                    var index = this.dataTable.Rows.IndexOf(row);
                    selectedCount++;
                    yield return index;
                }

                if (selectedCount == 0)
                {
                    yield return this.SelectedIndex;
                }
            }
        }

        private ConfigurationNode SelectedConfigurationNode
        {
            get
            {
                ConfigurationNode folder;
                var index = this.SelectedIndex;

                if (index >= 0)
                {
                    folder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
                    folder = folder.ChildNodes[index];
                }
                else
                {
                    folder = null;
                }

                return folder;
            }
        }

        private ConfigurationNode ToConfigurationNode(int index)
        {
            var node = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
            node = node.ChildNodes[index];
            return node;
        }

        private IEnumerable<ConfigurationNode> SelectedConfigurationNodes
        {
            get
            {
                var configurationNodes =
                    from index in this.SelectedIndexes
                    select this.ToConfigurationNode(index);

                return configurationNodes;
            }
        }

        private void Connect(ConfigurationNode folder)
        {
            if (this.isDirty)
            {
                if (MessageBox.Show(this, "Do you want to save changes?", null, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataCommanderApplication.Instance.SaveApplicationData();
                }
            }

            using (new CursorManager(Cursors.WaitCursor))
            {
                var connectionProperties = new ConnectionProperties();
                connectionProperties.Load(folder);
                connectionProperties.LoadProtectedPassword(folder);
                var form = new OpenConnectionForm(connectionProperties);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.ConnectionProperties = connectionProperties;
                    this.DialogResult = DialogResult.OK;
                    this.Duration = form.Duration;
                }
            }
        }

        private void dataGrid_DoubleClick(object sender, EventArgs e)
        {
            var position = this.dataGrid.PointToClient(Cursor.Position);
            var hitTestInfo = this.dataGrid.HitTest(position.X, position.Y);

            switch (hitTestInfo.Type)
            {
                case DataGridViewHitTestType.ColumnHeader:
                    break;

                default:
                    var folder = this.SelectedConfigurationNode;

                    if (folder != null)
                    {
                        this.Connect(folder);
                    }

                    break;
            }
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            log.Write(LogLevel.Trace, "e.KeyCode: {0}\r\ne.KeyData: {1}", e.KeyCode, e.KeyData);

            if (e.KeyData == (Keys.Alt | Keys.Up))
            {
                e.Handled = true;
                this.MoveUp();
            }
            else if (e.KeyData == (Keys.Alt | Keys.Down))
            {
                e.Handled = true;
                this.MoveDown();
            }
            else if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                var node = this.SelectedConfigurationNode;
                this.Connect(node);
            }

            //if (e.KeyCode == Keys.D)
            //{
            //}
            //else if (e.KeyCode == Keys.Delete)
            //{
            //    e.Handled = true;
            //    int index = SelectedIndex;

            //    if (index >= 0)
            //    {
            //        this.Delete();
            //    }
            //}
            //else if (e.KeyCode == (Keys.Alt | Keys.Up))
            //{
            //    log.Write("hahooo", LogLevel.Trace);
            //}
        }

        private void Add(ConnectionProperties connectionProperties)
        {
            var node = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
            var subFolder = new ConfigurationNode(null);
            node.AddChildNode(subFolder);
            connectionProperties.Save(subFolder);
            var row = this.dataTable.NewRow();
            this.LoadConnection(subFolder, row);
            this.dataTable.Rows.Add(row);
            this.isDirty = true;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            var form = new ConnectionStringBuilderForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                var connectionProperties = form.ConnectionProperties;
                this.Add(connectionProperties);
            }
        }

        private void dataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            this.Delete();
            e.Cancel = true;
        }
    }

    internal static class ColorThemeApplyer
    {
        public static void Apply(DataGridView dataGridView, ColorTheme colorTheme)
        {
            dataGridView.BackgroundColor = colorTheme.BackColor;
            dataGridView.BackColor = colorTheme.BackColor;
            dataGridView.ForeColor = colorTheme.ForeColor;

            dataGridView.EnableHeadersVisualStyles = true;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = colorTheme.BackColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = colorTheme.ForeColor;
            dataGridView.RowsDefaultCellStyle.BackColor = colorTheme.BackColor;
            dataGridView.RowsDefaultCellStyle.ForeColor = colorTheme.ForeColor;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = colorTheme.BackColor;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = colorTheme.BackColor;
        }
    }
}