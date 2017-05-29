using Foundation.Log;

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
    using Foundation.Linq;
    using Foundation.Windows.Forms;
    using ResultWriter;

    internal sealed class ConnectionForm : Form
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private Button _btnOk;
        private DoubleBufferedDataGridView _dataGrid;
        private Button _btnCancel;
        private Button _newButton;
        private StatusStrip _statusBar;
        private readonly DataTable _dataTable = new DataTable();
        private bool _isDirty;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container _components = new Container();

        public ConnectionForm(StatusStrip statusBar, ColorTheme colorTheme)
        {
            _statusBar = statusBar;
            InitializeComponent();

            _dataTable.Columns.Add("ConnectionName", typeof(string));
            _dataTable.Columns.Add("ProviderName", typeof(string));
            _dataTable.Columns.Add(ConnectionStringKeyword.DataSource);
            _dataTable.Columns.Add(ConnectionStringKeyword.InitialCatalog);
            _dataTable.Columns.Add(ConnectionStringKeyword.IntegratedSecurity);
            _dataTable.Columns.Add(ConnectionStringKeyword.UserId);
            _dataTable.Columns.Add("Persist Security Info");
            _dataTable.Columns.Add("Enlist");
            _dataTable.Columns.Add("Pooling");
            _dataTable.Columns.Add("Driver");
            _dataTable.Columns.Add("DBQ");
            _dataTable.Columns.Add("Unicode");
            _dataTable.Columns.Add("Extended Properties");
            _dataTable.Columns.Add("Naming");

            var folder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;

            foreach (var subFolder in folder.ChildNodes)
            {
                var dataRow = _dataTable.NewRow();
                LoadConnection(subFolder, dataRow);
                _dataTable.Rows.Add(dataRow);
            }

            var graphics = CreateGraphics();

            foreach (DataColumn column in _dataTable.Columns)
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

                    if (_dataTable.Rows.Count > 0)
                    {
                        IEnumerable<DataRow> enumerableDataRow = _dataTable.Rows.Cast<DataRow>();
                        var dataRowSelector = new DataRowSelector(column, graphics, Font);
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

            _dataGrid.DataSource = _dataTable;
            if (colorTheme != null)
            {
                BackColor = colorTheme.BackColor;
                ForeColor = colorTheme.ForeColor;

                ColorThemeApplyer.Apply(_dataGrid, colorTheme);
            }
        }

        public ConnectionProperties ConnectionProperties { get; private set; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (_components != null)
                    _components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._newButton = new System.Windows.Forms.Button();
            this._dataGrid = new DoubleBufferedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this._btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnOk.Location = new System.Drawing.Point(402, 637);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(75, 24);
            this._btnOk.TabIndex = 0;
            this._btnOk.Text = "&Connect";
            this._btnOk.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this._btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(490, 637);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 24);
            this._btnCancel.TabIndex = 7;
            this._btnCancel.Text = "Cancel";
            // 
            // newButton
            // 
            this._newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._newButton.Location = new System.Drawing.Point(12, 637);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new System.Drawing.Size(75, 24);
            this._newButton.TabIndex = 8;
            this._newButton.Text = "&New";
            this._newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // dataGrid
            // 
            this._dataGrid.AllowUserToAddRows = false;
            this._dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dataGrid.Location = new System.Drawing.Point(8, 8);
            this._dataGrid.Name = "_dataGrid";
            this._dataGrid.PublicDoubleBuffered = true;
            this._dataGrid.ReadOnly = true;
            this._dataGrid.Size = new System.Drawing.Size(944, 621);
            this._dataGrid.TabIndex = 6;
            this._dataGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGrid_UserDeletingRow);
            this._dataGrid.DoubleClick += new System.EventHandler(this.dataGrid_DoubleClick);
            this._dataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyDown);
            this._dataGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseClick);
            // 
            // ConnectionForm
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(954, 668);
            this.Controls.Add(this._newButton);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._dataGrid);
            this.Controls.Add(this._btnOk);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to database";
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).EndInit();
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
            var folder = SelectedConfigurationNode;
            Connect(folder);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            var folder = SelectedConfigurationNode;
            Connect(folder);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.Formatting = Formatting.Indented;

            foreach (var node in SelectedConfigurationNodes)
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
                    Add(connectionProperties);
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
                var index = SelectedIndex;
                var selectedFolder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(selectedFolder);
                _dataTable.Rows.RemoveAt(index);
                _isDirty = true;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            var folder = SelectedConfigurationNode;
            var connectionProperties = new ConnectionProperties();
            connectionProperties.Load(folder);
            var form = new ConnectionStringBuilderForm();
            form.ConnectionProperties = connectionProperties;
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                connectionProperties = form.ConnectionProperties;
                connectionProperties.Save(folder);
                var row = _dataTable.DefaultView[_dataGrid.CurrentCell.RowIndex].Row;
                LoadConnection(folder, row);
            }
        }

        private void MoveDown()
        {
            var index = SelectedIndex;
            var connectionsFolder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;

            if (index < connectionsFolder.ChildNodes.Count - 1)
            {
                var folder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(folder);
                connectionsFolder.InsertChildNode(index + 1, folder);

                _dataTable.Rows.RemoveAt(index);
                var row = _dataTable.NewRow();
                LoadConnection(folder, row);
                _dataTable.Rows.InsertAt(row, index + 1);
                _dataGrid.CurrentCell = _dataGrid[0, index + 1];
            }
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void MoveUp()
        {
            var index = SelectedIndex;

            if (index > 0)
            {
                var connectionsFolder = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
                var folder = connectionsFolder.ChildNodes[index];
                connectionsFolder.RemoveChildNode(folder);
                connectionsFolder.InsertChildNode(index - 1, folder);

                _dataTable.Rows.RemoveAt(index);
                var row = _dataTable.NewRow();
                LoadConnection(folder, row);
                _dataTable.Rows.InsertAt(row, index - 1);
                _dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                _dataGrid.CurrentCell = _dataGrid[0, index - 1];
            }
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void dataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = _dataGrid.HitTest(e.X, e.Y);
                var rowIndex = hitTestInfo.RowIndex;
                var contextMenu = new ContextMenuStrip(_components);
                ToolStripMenuItem menuItem;

                if (rowIndex >= 0)
                {
                    _dataGrid.CurrentCell = _dataGrid[0, rowIndex];
                    menuItem = new ToolStripMenuItem("&Edit", null, Edit_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("C&onnect", null, Connect_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("&Copy", null, Copy_Click);
                    contextMenu.Items.Add(menuItem);
                }

                menuItem = new ToolStripMenuItem("&Paste", null, Paste_Click);
                contextMenu.Items.Add(menuItem);

                if (rowIndex >= 0)
                {
                    menuItem = new ToolStripMenuItem("&Delete", null, Delete_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("Move &up", null, MoveUp_Click);
                    contextMenu.Items.Add(menuItem);
                    menuItem = new ToolStripMenuItem("Move &down", null, MoveDown_Click);
                    contextMenu.Items.Add(menuItem);
                }

                contextMenu.Show(this, e.Location);
            }
        }

        private int SelectedIndex
        {
            get
            {
                var index = _dataGrid.CurrentCell.RowIndex;

                if (index >= 0)
                {
                    var dataView = _dataTable.DefaultView;
                    var rowView = dataView[index];
                    var row = rowView.Row;
                    index = _dataTable.Rows.IndexOf(row);
                }

                return index;
            }
        }

        private IEnumerable<int> SelectedIndexes
        {
            get
            {
                var count = _dataTable.Rows.Count;
                var dataView = _dataTable.DefaultView;
                var selectedCount = 0;

                foreach (DataGridViewRow dataGridViewRow in _dataGrid.SelectedRows)
                {
                    var dataRowView = (DataRowView)dataGridViewRow.DataBoundItem;
                    var row = dataRowView.Row;
                    var index = _dataTable.Rows.IndexOf(row);
                    selectedCount++;
                    yield return index;
                }

                if (selectedCount == 0)
                {
                    yield return SelectedIndex;
                }
            }
        }

        private ConfigurationNode SelectedConfigurationNode
        {
            get
            {
                ConfigurationNode folder;
                var index = SelectedIndex;

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
                    from index in SelectedIndexes
                    select ToConfigurationNode(index);

                return configurationNodes;
            }
        }

        private void Connect(ConfigurationNode folder)
        {
            if (_isDirty)
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
                    ConnectionProperties = connectionProperties;
                    DialogResult = DialogResult.OK;
                    Duration = form.Duration;
                }
            }
        }

        private void dataGrid_DoubleClick(object sender, EventArgs e)
        {
            var position = _dataGrid.PointToClient(Cursor.Position);
            var hitTestInfo = _dataGrid.HitTest(position.X, position.Y);

            switch (hitTestInfo.Type)
            {
                case DataGridViewHitTestType.ColumnHeader:
                    break;

                default:
                    var folder = SelectedConfigurationNode;

                    if (folder != null)
                    {
                        Connect(folder);
                    }

                    break;
            }
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Log.Write(LogLevel.Trace, "e.KeyCode: {0}\r\ne.KeyData: {1}", e.KeyCode, e.KeyData);

            if (e.KeyData == (Keys.Alt | Keys.Up))
            {
                e.Handled = true;
                MoveUp();
            }
            else if (e.KeyData == (Keys.Alt | Keys.Down))
            {
                e.Handled = true;
                MoveDown();
            }
            else if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                var node = SelectedConfigurationNode;
                Connect(node);
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
            var row = _dataTable.NewRow();
            LoadConnection(subFolder, row);
            _dataTable.Rows.Add(row);
            _isDirty = true;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            var form = new ConnectionStringBuilderForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                var connectionProperties = form.ConnectionProperties;
                Add(connectionProperties);
            }
        }

        private void dataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Delete();
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