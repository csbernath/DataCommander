using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Application.ResultWriter;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Log;
using Foundation.Windows.Forms;
using Newtonsoft.Json;

namespace DataCommander.Application.Connection;

internal sealed class ConnectionListForm : Form
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly List<ConnectionInfo> _connectionInfos;
    private ConnectionInfo? _connectionInfo;
    private ConnectionBase? _connection;
    private DoubleBufferedDataGridView? _dataGrid;
    private Button? _btnOk;    
    private Button? _btnCancel;
    private Button? _newButton;
    private readonly DataTable _dataTable = new();
    private bool _isDirty;
    private readonly Container _components = new();
    private readonly ColorTheme? _colorTheme;

    public ConnectionListForm(ColorTheme? colorTheme)
    {
        _colorTheme = colorTheme;

        InitializeComponent();

        _dataTable.Columns.Add(ConnectionFormColumnName.ConnectionName, typeof(string));
        _dataTable.Columns.Add(ConnectionFormColumnName.ProviderName, typeof(string));
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

        _connectionInfos = ConnectionInfoRepository.Get().ToList();

        foreach (var connectionProperties in _connectionInfos)
        {
            var dataRow = _dataTable.NewRow();
            LoadConnection(connectionProperties, dataRow);
            _dataTable.Rows.Add(dataRow);
        }

        var graphics = CreateGraphics();

        foreach (DataColumn column in _dataTable.Columns)
        {
            var dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            var columnName = column.ColumnName;
            dataGridViewTextBoxColumn.DataPropertyName = columnName;
            dataGridViewTextBoxColumn.HeaderText = columnName;

            if (columnName == ConnectionFormColumnName.ConnectionName ||
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
            }
        }

        _dataGrid!.DataSource = _dataTable;
        if (colorTheme != null)
        {
            BackColor = colorTheme.BackColor.Value;
            ForeColor = colorTheme.ForeColor.Value;
            colorTheme.Apply(_dataGrid);
        }
    }

    public ConnectionInfo ConnectionInfo => _connectionInfo!;
    public ConnectionBase Connection => _connection!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && _components != null)
            _components.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _btnOk = new Button();
        _btnCancel = new Button();
        _newButton = new Button();
        _dataGrid = new DoubleBufferedDataGridView();
        ((ISupportInitialize)(_dataGrid)).BeginInit();
        SuspendLayout();
        // 
        // btnOK
        // 
        _btnOk.Anchor = AnchorStyles.Bottom;
        _btnOk.Location = new System.Drawing.Point(402, 637);
        _btnOk.Name = "_btnOk";
        _btnOk.Size = new System.Drawing.Size(75, 24);
        _btnOk.TabIndex = 0;
        _btnOk.Text = "&Connect";
        _btnOk.Click += new EventHandler(BtnOK_Click);
        // 
        // btnCancel
        // 
        _btnCancel.Anchor = AnchorStyles.Bottom;
        _btnCancel.DialogResult = DialogResult.Cancel;
        _btnCancel.Location = new System.Drawing.Point(490, 637);
        _btnCancel.Name = "_btnCancel";
        _btnCancel.Size = new System.Drawing.Size(75, 24);
        _btnCancel.TabIndex = 7;
        _btnCancel.Text = "Cancel";
        // 
        // newButton
        // 
        _newButton.Anchor =
            ((AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Left));
        _newButton.Location = new System.Drawing.Point(12, 637);
        _newButton.Name = "_newButton";
        _newButton.Size = new System.Drawing.Size(75, 24);
        _newButton.TabIndex = 8;
        _newButton.Text = "&New";
        _newButton.Click += new EventHandler(NewButton_Click);
        // 
        // dataGrid
        // 
        _dataGrid.AllowUserToAddRows = false;
        _dataGrid.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
                                                                       | AnchorStyles.Left)
                                                                      | AnchorStyles.Right));
        _dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        _dataGrid.Location = new System.Drawing.Point(8, 8);
        _dataGrid.Name = "_dataGrid";
        _dataGrid.PublicDoubleBuffered = true;
        _dataGrid.ReadOnly = true;
        _dataGrid.Size = new System.Drawing.Size(944, 621);
        _dataGrid.TabIndex = 6;
        _dataGrid.UserDeletingRow += new DataGridViewRowCancelEventHandler(DataGrid_UserDeletingRow);
        _dataGrid.DoubleClick += new EventHandler(DataGrid_DoubleClick);
        _dataGrid.KeyDown += new KeyEventHandler(dataGrid_KeyDown);
        _dataGrid.MouseClick += new MouseEventHandler(dataGrid_MouseClick);
        // 
        // ConnectionListForm
        // 
        AcceptButton = _btnOk;
        AutoScaleBaseSize = new System.Drawing.Size(5, 14);
        CancelButton = _btnCancel;
        ClientSize = new System.Drawing.Size(954, 668);
        Controls.Add(_newButton);
        Controls.Add(_btnCancel);
        Controls.Add(_dataGrid);
        Controls.Add(_btnOk);
        Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ConnectionListForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Connect to database";
        ((ISupportInitialize)(_dataGrid)).EndInit();
        ResumeLayout(false);

    }

    public long ElapsedTicks { get; private set; }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (_isDirty)
        {
            const string text = "Do you want to save changes ?";
            const string caption = "Data Commander";
            var dialogResult = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                ConnectionInfoRepository.Save(_connectionInfos);
        }
    }

    private static void LoadConnection(ConnectionInfo connectionInfo, DataRow row)
    {
        row[ConnectionFormColumnName.ConnectionName] = connectionInfo.ConnectionName;

        var providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == connectionInfo.ProviderIdentifier);
        var provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);
        var connectionStringBuilder = provider.CreateConnectionStringBuilder();
        
        row[ConnectionFormColumnName.ProviderName] = providerInfo.Name;
 
        connectionStringBuilder.ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString;

        if (connectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var value))
            row[ConnectionStringKeyword.DataSource] = (string)value!;
        else if (connectionStringBuilder.TryGetValue(ConnectionStringKeyword.Host, out value))
            row[ConnectionStringKeyword.DataSource] = (string)value!;        

        if (connectionStringBuilder.TryGetValue(ConnectionStringKeyword.InitialCatalog, out value))
            row[ConnectionStringKeyword.InitialCatalog] = (string)value!;

        if (connectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out value))
        {
            var integratedSecurity = value switch
            {
                bool boolValue => boolValue,
                _ => bool.Parse((string)value!),
            };
            row[ConnectionStringKeyword.IntegratedSecurity] = integratedSecurity;
        }

        if (connectionInfo.ConnectionStringAndCredential.Credential != null)
            row[ConnectionStringKeyword.UserId] = connectionInfo.ConnectionStringAndCredential.Credential.UserId;
        else if (connectionStringBuilder.TryGetValue(ConnectionStringKeyword.UserId, out value))
            row[ConnectionStringKeyword.UserId] = (string)value!;
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        var connectionInfo = SelectedConnectionInfo!;
        Connect(connectionInfo);
    }

    private void Connect_Click(object? sender, EventArgs e)
    {
        var connectionInfo = SelectedConnectionInfo;
        Connect(connectionInfo);
    }

    private void Copy_Click(object? sender, EventArgs e)
    {
        var connectionPropertiesArray = SelectedIndexes
            .Select(index => _connectionInfos[index].ToConnectionDto());
        var json = JsonConvert.SerializeObject(connectionPropertiesArray);
        Clipboard.SetText(json);
    }

    private void CopyConnectionString_Click(object? sender, EventArgs e)
    {
        var connectionProperties = SelectedConnectionInfo;
        var connectionString = connectionProperties!.ConnectionStringAndCredential.ConnectionString;
        Clipboard.SetText(connectionString);
    }

    private void Paste_Click(object? sender, EventArgs e)
    {
        try
        {
            var s = Clipboard.GetText();
            var connectionDtos = JsonConvert.DeserializeObject<ConnectionDto[]>(s);
            var connectionPropertiesList = connectionDtos
                .Select(connectionDto => connectionDto.ToConnectionProperties());
            foreach (var connectionProperties in connectionPropertiesList)
                Add(connectionProperties);
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
            var index = SelectedIndex;
            _connectionInfos.RemoveAt(index);
            _dataTable.Rows.RemoveAt(index);
            _isDirty = true;
        }
    }

    private void Delete_Click(object? sender, EventArgs e) => Delete();

    private void Edit_Click(object? sender, EventArgs e)
    {
        var form = new ConnectionStringBuilderForm(_colorTheme);
        var connectionInfo = SelectedConnectionInfo!;
        form.ConnectionInfo = connectionInfo;
        var dialogResult = form.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            _connectionInfos[SelectedIndex] = form.ConnectionInfo;
            _isDirty = true;
            var row = _dataTable.DefaultView[_dataGrid!.CurrentCell!.RowIndex].Row;
            LoadConnection(connectionInfo, row);
        }
    }

    private void MoveDown()
    {
        var index = SelectedIndex;
        if (index < _connectionInfos.Count - 1)
        {
            var connectionInfo = _connectionInfos[index];
            _connectionInfos.RemoveAt(index);
            _connectionInfos.Insert(index + 1, connectionInfo);

            _dataTable.Rows.RemoveAt(index);
            var row = _dataTable.NewRow();
            LoadConnection(connectionInfo, row);
            _dataTable.Rows.InsertAt(row, index + 1);
            _dataGrid!.CurrentCell = _dataGrid[0, index + 1];
        }
    }

    private void MoveDown_Click(object? sender, EventArgs e) => MoveDown();

    private void MoveUp()
    {
        var index = SelectedIndex;
        if (index > 0)
        {
            var connectionInfo = _connectionInfos[index];
            _connectionInfos.RemoveAt(index);
            _connectionInfos.Insert(index-1, connectionInfo);

            _dataTable.Rows.RemoveAt(index);
            var row = _dataTable.NewRow();
            LoadConnection(connectionInfo, row);
            _dataTable.Rows.InsertAt(row, index - 1);
            _dataGrid!.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dataGrid.CurrentCell = _dataGrid[0, index - 1];
        }
    }

    private void MoveUp_Click(object? sender, EventArgs e) => MoveUp();

    private void dataGrid_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var hitTestInfo = _dataGrid!.HitTest(e.X, e.Y);
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
                menuItem = new ToolStripMenuItem("Copy connection string to clipboard", null, CopyConnectionString_Click);
                contextMenu.Items.Add(menuItem);
            }

            if (Clipboard.ContainsText())
            {
                menuItem = new ToolStripMenuItem("&Paste", null, Paste_Click);
                contextMenu.Items.Add(menuItem);
            }

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
            var index = _dataGrid!.CurrentCell!.RowIndex;

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

            foreach (DataGridViewRow dataGridViewRow in _dataGrid!.SelectedRows)
            {
                var dataRowView = (DataRowView)dataGridViewRow.DataBoundItem!;
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

    private ConnectionInfo? SelectedConnectionInfo
    {
        get
        {
            var index = SelectedIndex;
            var connectionInfo = index >= 0
                ? _connectionInfos[index]
                : null;
            return connectionInfo;
        }
    }

    private void Connect(ConnectionInfo connectionInfo)
    {
        try
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == connectionInfo.ProviderIdentifier);
                var provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);
                var connectionStringBuilder = provider.CreateConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString;

                var dataSource = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject)
                    ? (string)dataSourceObject
                    : null;
                var host = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.Host, out var hostObject)
                    ? (string)hostObject
                    : null;

                var containsIntegratedSecurity = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurity);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($@"Connection name: {connectionInfo.ConnectionName}
Provider name: {providerInfo.Name}");
                if (dataSource != null)
                    stringBuilder.Append($"\r\n{ConnectionStringKeyword.DataSource}: {dataSource}");
                else if (host != null)
                    stringBuilder.Append($"\r\n{ConnectionStringKeyword.Host}: {host}");
                if (containsIntegratedSecurity)
                    stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
                var credential = connectionInfo.ConnectionStringAndCredential.Credential;
                if (credential != null)
                    stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {credential.UserId}");
                var text = stringBuilder.ToString();
                var connection = provider.CreateConnection(connectionInfo.ConnectionStringAndCredential);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                var cancelableOperationForm =
                    new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1), "Opening connection...", text, _colorTheme);
                var startTimestamp = Stopwatch.GetTimestamp();
                var openConnectionTask = new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken));
                cancelableOperationForm.Execute(openConnectionTask);
                if (openConnectionTask.Exception != null)
                    throw openConnectionTask.Exception;
                ElapsedTicks = Stopwatch.GetTimestamp() - startTimestamp;                
                _connectionInfo = connectionInfo;
                _connection = connection;
                DialogResult = DialogResult.OK;
            }
        }
        catch (Exception exception)
        {
            var text = exception.Message;
            var caption = "Opening connection failed.";
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DataGrid_DoubleClick(object? sender, EventArgs e)
    {
        var position = _dataGrid!.PointToClient(Cursor.Position);
        var hitTestInfo = _dataGrid.HitTest(position.X, position.Y);

        switch (hitTestInfo.Type)
        {
            case DataGridViewHitTestType.ColumnHeader:
                break;

            default:
                var folder = SelectedConnectionInfo;

                if (folder != null)
                {
                    Connect(folder);
                }

                break;
        }
    }

    private void dataGrid_KeyDown(object? sender, KeyEventArgs e)
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
            var node = SelectedConnectionInfo;
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

    private void Add(ConnectionInfo connectionInfo)
    {
        _connectionInfos.Add(connectionInfo);

        var row = _dataTable.NewRow();
        LoadConnection(connectionInfo, row);
        _dataTable.Rows.Add(row);

        _isDirty = true;
    }

    private void NewButton_Click(object? sender, EventArgs e)
    {
        var form = new ConnectionStringBuilderForm(_colorTheme);

        if (form.ShowDialog() == DialogResult.OK)
        {
            var connectionProperties = form.ConnectionInfo;
            Add(connectionProperties);
        }
    }

    private void DataGrid_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
    {
        Delete();
        e.Cancel = true;
    }
}