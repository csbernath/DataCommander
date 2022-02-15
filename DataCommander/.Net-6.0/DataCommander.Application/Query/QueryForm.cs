using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ADODB;
using DataCommander.Application.Connection;
using DataCommander.Application.ResultWriter;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Data.SqlClient.DbQueryBuilding;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;
using Foundation.Threading;
using Foundation.Windows.Forms;
using Newtonsoft.Json;
using Timer = System.Windows.Forms.Timer;

namespace DataCommander.Application.Query;

public sealed class QueryForm : Form, IQueryForm
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    private readonly MainForm _mainForm;
    private MenuStrip _mainMenu;
    private ToolStripMenuItem _menuItem1;
    private StatusStrip _statusBar;
    private ToolStripMenuItem _mnuDescribeParameters;
    private ToolStripMenuItem _mnuCloseTabPage;
    private ToolStripMenuItem _mnuCancel;
    private ToolStripStatusLabel _sbPanelText;
    private ToolStripStatusLabel _sbPanelTimer;
    private ToolStripStatusLabel _sbPanelRows;
    private ToolStripMenuItem _menuItem2;
    private ToolStripMenuItem _mnuDataGrid;
    private ToolStripMenuItem _mnuHtml;
    private ToolStripMenuItem _mnuRtf;
    private ToolStripMenuItem _menuItem7;
    private ToolStripMenuItem _mnuCommandTypeText;
    private ToolStripMenuItem _mnuCommandTypeStoredProcedure;
    private ToolStripMenuItem _mnuText;
    private TreeView _tvObjectExplorer;
    private Splitter _splitterObjectExplorer;
    private ToolStripMenuItem _menuItem3;
    private Splitter _splitterQuery;
    private TabControl _tabControl;
    private ToolStripMenuItem _menuItem8;
    private ToolStripMenuItem _mnuPaste;
    private ToolStripStatusLabel _sbPanelCaretPosition;
    private ToolStripMenuItem _mnuFind;
    private ToolStripMenuItem _mnuFindNext;
    private ToolStripMenuItem _mnuObjectExplorer;
    private ToolStripMenuItem _menuItem9;
    private ToolStripMenuItem _mnuSaveAs;
    private ToolStripMenuItem _mnuExcel;
    private ToolStripMenuItem _mnuCodeCompletion;
    private ToolStripMenuItem _mnuListMembers;
    private ToolStripMenuItem _mnuGotoQueryEditor;
    private ToolStripMenuItem _mnuCloseAllTabPages;
    private ToolStripMenuItem _mnuGotoMessageTabPage;
    private ToolStripMenuItem _mnuClearCache;
    private ToolStripMenuItem _mnuListView;
    private ToolStripMenuItem _mnuExecuteQueryKeyInfo;
    private ToolStripMenuItem _mnuExecuteQuerySchemaOnly;
    private ToolStripMenuItem _mnuExecuteQuerySingleRow;
    private ToolStripMenuItem _mnuShowShemaTable;
    private ToolStripMenuItem _mnuExecuteQueryXml;
    private ToolStripMenuItem _mnuSave;
    private ToolStripMenuItem _mnuCreateInsert;
    private ToolStripMenuItem _menuResultModeFile;
    private ToolStripStatusLabel _sbPanelTableStyle;
    private ToolStripMenuItem _mnuGoTo;
    private ToolStripMenuItem _mnuDuplicateConnection;
    private ToolStripMenuItem _mnuCreateInsertSelect;
    private ToolStripMenuItem _mnuOpenTable;
    private readonly IContainer components = new Container();
    private readonly string _connectionString;
    private IDbTransaction _transaction;
    private SqlParser _sqlStatement;
    private IDbCommand _command;
    private CommandType _commandType = CommandType.Text;
    private IAsyncDataAdapter _dataAdapter;
    private bool _cancel;
    private readonly Timer _timer = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly int _htmlMaxRecords;
    private readonly int _wordMaxRecords;
    private DataSetResultWriter _dataSetResultWriter;
    private bool _showSchemaTable;
    private readonly StatusStrip _parentStatusBar;
    private readonly TextBoxWriter _textBoxWriter;
    private FindTextForm _findTextForm;
    private readonly int _rowBlockSize;
    private readonly StandardOutput _standardOutput;
    private string _database;
    private string _fileName;
    private int _commandTimeout;
    private Font _font;
    private ToolStripMenuItem _mnuRefreshObjectExplorer;
    private ToolStripSeparator _toolStripSeparator2;
    private ToolStripSeparator _toolStripSeparator1;
    private ToolStripSeparator _toolStripSeparator3;
    private ToolStripMenuItem _sQLiteDatabaseToolStripMenuItem;
    private ToolStripMenuItem _exportToolStripMenuItem;
    private ToolStripMenuItem _commitTransactionToolStripMenuItem;
    private ToolStripMenuItem _beginTransactionToolStripMenuItem;
    private ToolStripMenuItem _rollbackTransactionToolStripMenuItem;
    private ToolStripMenuItem _createSqlCeDatabaseToolStripMenuItem;
    private ToolStripMenuItem _insertScriptFileToolStripMenuItem;
    private bool _openTableMode;

    private readonly TabPage _messagesTabPage;
    private readonly RichTextBox _messagesTextBox;

    private readonly TabPage _resultSetsTabPage;
    private readonly TabControl _resultSetsTabControl;
    private ToolStrip _toolStrip;
    private ToolStripSeparator _toolStripSeparator4;
    private ToolStripSplitButton _executeQuerySplitButton;
    private ToolStripMenuItem _executeQueryMenuItem;
    private ToolStripMenuItem _executeQuerySingleRowToolStripMenuItem;
    private ToolStripMenuItem _cToolStripMenuItem;
    private ToolStripButton _cancelQueryButton;
    private ToolStripMenuItem _executeQueryToolStripMenuItem;
    private ToolStripMenuItem _openTableToolStripMenuItem;
    private ToolStripMenuItem _parseToolStripMenuItem;
    private QueryTextBox _queryTextBox;

    private readonly ConcurrentQueue<InfoMessage> _infoMessages = new();
    private int _errorCount;
    private readonly LimitedConcurrencyLevelTaskScheduler _scheduler = new(1);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly EventWaitHandle _enqueueEvent = new(false, EventResetMode.AutoReset);
    private ToolStripMenuItem createCCommandQueryToolStripMenuItem;
    private ToolStripMenuItem undoToolStripMenuItem;
    private ToolStripMenuItem redoToolStripMenuItem;
    private readonly ColorTheme _colorTheme;

    #endregion

    #region Constructors

    static QueryForm()
    {
        NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
    }

    public QueryForm(MainForm mainForm, int index, IProvider provider, string connectionString, ConnectionBase connection, StatusStrip parentStatusBar,
        ColorTheme colorTheme)
    {
        GarbageMonitor.Default.Add("QueryForm", this);

        _mainForm = mainForm;
        Provider = provider;
        _connectionString = connectionString;
        Connection = connection;
        _parentStatusBar = parentStatusBar;
        _colorTheme = colorTheme;
        connection.InfoMessage += Connection_InfoMessage;
        connection.DatabaseChanged += Connection_DatabaseChanged;
        _timer.Tick += Timer_Tick;

        var task = new Task(ConsumeInfoMessages);
        task.Start(_scheduler);

        _messagesTextBox = new RichTextBox();
        components.Add(_messagesTextBox);
        GarbageMonitor.Default.Add("QueryForm.messagesTextBox", _messagesTextBox);
        _messagesTextBox.Multiline = true;
        _messagesTextBox.WordWrap = false;
        _messagesTextBox.Dock = DockStyle.Fill;
        _messagesTextBox.ScrollBars = RichTextBoxScrollBars.Both;

        _messagesTabPage = new TabPage("Messages");
        _messagesTabPage.Controls.Add(_messagesTextBox);

        InitializeComponent();
        GarbageMonitor.Default.Add("queryForm.toolStrip", _toolStrip);
        _mnuFind.Click += mnuFind_Click;
        _mnuFindNext.Click += mnuFindNext_Click;
        _mnuPaste.Click += mnuPaste_Click;
        _mnuGoTo.Click += mnuGoTo_Click;
        _mnuClearCache.Click += mnuClearCache_Click;

        var sqlKeyWords = Settings.CurrentType.Attributes["Sql92ReservedWords"].GetValue<string[]>();
        var providerKeyWords = provider.KeyWords;

        _queryTextBox.SetColorTheme(colorTheme);
        _queryTextBox.AddKeyWords(new[] { "exec" }, colorTheme != null
            ? colorTheme.ExecKeyWordColor
            : Color.Green);
        _queryTextBox.AddKeyWords(sqlKeyWords, colorTheme != null
            ? colorTheme.SqlKeyWordColor
            : Color.Blue);
        _queryTextBox.AddKeyWords(providerKeyWords, colorTheme != null
            ? colorTheme.ProviderKeyWordColor
            : Color.Red);

        _queryTextBox.CaretPositionPanel = _sbPanelCaretPosition;

        SetText();

        _resultSetsTabPage = new TabPage("Results");
        _resultSetsTabControl = new TabControl();
        _resultSetsTabControl.MouseUp += resultSetsTabControl_MouseUp;
        _resultSetsTabControl.Alignment = TabAlignment.Top;
        _resultSetsTabControl.Dock = DockStyle.Fill;
        _resultSetsTabPage.Controls.Add(_resultSetsTabControl);

        _tabControl.TabPages.Add(_resultSetsTabPage);
        _tabControl.TabPages.Add(_messagesTabPage);
        _tabControl.SelectedTab = _messagesTabPage;

        _standardOutput = new StandardOutput(new TextBoxWriter(_messagesTextBox), this);

        _textBoxWriter = new TextBoxWriter(_messagesTextBox);

        var objectExplorer = provider.CreateObjectExplorer();
        if (objectExplorer != null)
        {
            objectExplorer.SetConnection(connectionString, connection.Connection);
            AddNodes(_tvObjectExplorer.Nodes, objectExplorer.GetChildren(true), objectExplorer.Sortable);
        }
        else
        {
            _tvObjectExplorer.Visible = false;
            _splitterObjectExplorer.Visible = false;
            _mnuObjectExplorer.Enabled = false;
        }

        var text = $"&{index + 1} - {Text}";

        _database = connection.Database;
        SetResultWriterType(ResultWriterType.DataGrid);

        var node = Settings.CurrentType;
        var attributes = node.Attributes;
        _rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
        _htmlMaxRecords = attributes["HtmlMaxRecords"].GetValue<int>();
        _wordMaxRecords = attributes["WordMaxRecords"].GetValue<int>();
        _rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
        _timer.Interval = attributes["TimerInterval"].GetValue<int>();

        SettingsChanged(null, null);
        Settings.Changed += SettingsChanged;

        if (colorTheme != null)
        {
            foreach (var menuItem in _mainMenu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>())
            {
                foreach (ToolStripItem x in menuItem.DropDownItems)
                {
                    x.ForeColor = colorTheme.ForeColor;
                    x.BackColor = colorTheme.BackColor;
                }
            }

            _toolStrip.ForeColor = colorTheme.ForeColor;
            _toolStrip.BackColor = colorTheme.BackColor;

            _tvObjectExplorer.ForeColor = colorTheme.ForeColor;
            _tvObjectExplorer.BackColor = colorTheme.BackColor;

            _tabControl.ForeColor = colorTheme.ForeColor;
            _tabControl.BackColor = colorTheme.BackColor;

            foreach (Control control in _tabControl.Controls)
            {
                control.BackColor = colorTheme.BackColor;
                control.ForeColor = colorTheme.ForeColor;
            }

            _resultSetsTabControl.ForeColor = colorTheme.ForeColor;
            _resultSetsTabControl.BackColor = colorTheme.BackColor;

            _messagesTextBox.ForeColor = colorTheme.ForeColor;
            _messagesTextBox.BackColor = colorTheme.BackColor;

            _statusBar.ForeColor = colorTheme.ForeColor;
            _statusBar.BackColor = colorTheme.BackColor;

            foreach (ToolStripItem item in _statusBar.Items)
            {
                item.ForeColor = colorTheme.ForeColor;
                item.BackColor = colorTheme.BackColor;
            }
        }
    }

    private void CloseResultTabPage(TabPage tabPage)
    {
        foreach (Control control in tabPage.Controls)
            control.Dispose();

        tabPage.Controls.Clear();
    }

    private void CloseResultSetTabPage(TabPage tabPage)
    {
        _resultSetsTabControl.TabPages.Remove(tabPage);
        var control = tabPage.Controls[0];
        if (control is TabControl tabControl)
        {
            var tabPages = tabControl.TabPages.Cast<TabPage>().ToList();
            foreach (var subTabPage in tabPages)
            {
                tabControl.TabPages.Remove(subTabPage);
                CloseResultTabPage(subTabPage);
            }
        }
        else
            CloseResultTabPage(tabPage);
    }

    private void CloseResultSetTabPage_Click(object sender, EventArgs e)
    {
        var toolStripMenuItem = (ToolStripMenuItem)sender;
        var tabPage = (TabPage)toolStripMenuItem.Tag;
        CloseResultSetTabPage(tabPage);
        toolStripMenuItem.Tag = null;
    }

    private void resultSetsTabControl_MouseUp(object sender, MouseEventArgs e)
    {
        var hitTestInfo = new Tchittestinfo(e.X, e.Y);
        var index = SendMessage(_resultSetsTabControl.Handle, TcmHittest, IntPtr.Zero, ref hitTestInfo);
        var hotTab = index >= 0 ? _resultSetsTabControl.TabPages[index] : null;

        switch (e.Button)
        {
            case MouseButtons.Middle:
                if (index >= 0)
                    CloseResultSetTabPage(hotTab);
                break;

            case MouseButtons.Right:
                if (index >= 0)
                {
                    var contextMenu = new ContextMenuStrip(components);
                    contextMenu.Items.Add(new ToolStripMenuItem("Close", null, CloseResultSetTabPage_Click)
                    {
                        Tag = hotTab
                    });
                    contextMenu.Items.Add(new ToolStripMenuItem("Close all", null, mnuCloseAllTabPages_Click, Keys.Control | Keys.Shift | Keys.F4));
                    contextMenu.Show(_resultSetsTabControl, e.Location);
                }

                break;
        }
    }

    #endregion

    #region Properties

    public ColorTheme ColorTheme => _colorTheme;

    public CommandState ButtonState { get; private set; }

    public ConnectionBase Connection { get; private set; }

    public override Font Font
    {
        set
        {
            _font = value;
            _queryTextBox.Font = value;
            var size1 = TextRenderer.MeasureText("1", value);
            var size2 = TextRenderer.MeasureText("12", value);
            var width = _queryTextBox.TabSize * (size2.Width - size1.Width);
            var tabs = new int[12];

            for (var i = 0; i < tabs.Length; i++)
                tabs[i] = (i + 1) * width;

            //_queryTextBox.EnableChangeEvent(false);
            _queryTextBox.RichTextBox.Font = value;
            _queryTextBox.RichTextBox.SelectionTabs = tabs;
            //_queryTextBox.EnableChangeEvent(true);

            _messagesTextBox.Font = value;
            _messagesTextBox.SelectionTabs = tabs;
        }
    }

    public static NumberFormatInfo NumberFormat { get; }

    public IProvider Provider { get; }

    private string Query
    {
        get
        {
            var query = _queryTextBox.SelectedText;

            if (query.Length == 0)
                query = _queryTextBox.Text;

            query = query.Replace("\n", "\r\n");
            return query;
        }
    }

    public QueryTextBox QueryTextBox => _queryTextBox;
    internal int ResultSetCount { get; private set; }
    public ResultWriterType TableStyle { get; private set; }

    internal ToolStrip ToolStrip => _toolStrip;

    #endregion

    #region Public Methods

    public void AddDataTable(DataTable dataTable, ResultWriterType tableStyle)
    {
        switch (tableStyle)
        {
            case ResultWriterType.Text:
                ShowDataTableText(dataTable);
                break;

            case ResultWriterType.DataGrid:
                ShowDataTableDataGrid(dataTable);
                break;

            //case ResultWriterType.Html:
            //    ShowDataViewHtml(dataTable.DefaultView);
            //    break;

            case ResultWriterType.Rtf:
                ShowDataTableRtf(dataTable);
                break;

            case ResultWriterType.ListView:
                ShowDataTableListView(dataTable);
                break;

            case ResultWriterType.Excel:
                ShowDataTableExcel(dataTable);
                break;
        }
    }

    public void AppendQueryText(string text)
    {
        QueryTextBox.RichTextBox.AppendText(text);
    }

    public void ShowDataSet(DataSet dataSet)
    {
        using (var log = LogFactory.Instance.GetCurrentMethodLog())
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                GetTableSchemaResult getTableSchemaResult = null;
                string? text;
                if (_openTableMode)
                {
                    var tableName = _sqlStatement.FindTableName();
                    text = tableName;
                    dataSet.Tables[0].TableName = tableName;
                    getTableSchemaResult = Provider.GetTableSchema(Connection.Connection, tableName);
                }
                else
                {
                    ResultSetCount++;
                    text = $"Set {ResultSetCount}";
                }

                var resultSetTabPage = new TabPage(text);
                GarbageMonitor.Default.Add("resultSetTabPage", resultSetTabPage);
                resultSetTabPage.ToolTipText = null; // TODO
                _resultSetsTabControl.TabPages.Add(resultSetTabPage);
                _resultSetsTabControl.SelectedTab = resultSetTabPage;
                if (dataSet.Tables.Count > 1)
                {
                    var tabControl = new TabControl { Dock = DockStyle.Fill };
                    tabControl.MouseUp += DataTableTabControl_MouseUp;
                    
                    var index = 0;
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
                        var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataTable, getTableSchemaResult, TableStyle,
                            !_openTableMode, _sbPanelText, _colorTheme);
                        control.Dock = DockStyle.Fill;
                        text = dataTable.TableName;
                        var tabPage = new TabPage(text);
                        tabPage.ToolTipText = null; // TODO
                        tabPage.Controls.Add(control);
                        tabControl.TabPages.Add(tabPage);
                        index++;
                    }

                    resultSetTabPage.Controls.Add(tabControl);
                }
                else
                {
                    var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
                    var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataSet.Tables[0], getTableSchemaResult, TableStyle,
                        !_openTableMode, _sbPanelText, _colorTheme);
                    control.Dock = DockStyle.Fill;
                    resultSetTabPage.Controls.Add(control);
                }
            }
        }
    }

    private void DataTableTabControl_MouseUp(object? sender, MouseEventArgs e)
    {
        if (sender != null && e.Button == MouseButtons.Middle)
        {
            var tabControl = (TabControl)sender;
            var hitTestInfo = new Tchittestinfo(e.X, e.Y);
            var index = SendMessage(tabControl.Handle, TcmHittest, IntPtr.Zero, ref hitTestInfo);
            if (index >= 0)
            {
                tabControl.TabPages.RemoveAt(index);

                if (tabControl.TabPages.Count == 0)
                {
                    var tabPage = (TabPage)tabControl.Parent;
                    tabControl = (TabControl)tabPage.Parent;
                    tabControl.TabPages.Remove(tabPage);
                }
            }
        }
    }

    public void ShowXml(string tabPageName, string xml)
    {
        var htmlTextBox = new HtmlTextBox();
        htmlTextBox.Dock = DockStyle.Fill;

        var tabPage = new TabPage(tabPageName);
        _tabControl.TabPages.Add(tabPage);
        tabPage.Controls.Add(htmlTextBox);

        htmlTextBox.Xml = xml;
    }

    #endregion

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
        this._mainMenu = new System.Windows.Forms.MenuStrip();
        this._menuItem9 = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuSave = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuDuplicateConnection = new System.Windows.Forms.ToolStripMenuItem();
        this._menuItem8 = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuFind = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuFindNext = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCodeCompletion = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuListMembers = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuClearCache = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuGoTo = new System.Windows.Forms.ToolStripMenuItem();
        this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        this._menuItem7 = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCommandTypeText = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCommandTypeStoredProcedure = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuDescribeParameters = new System.Windows.Forms.ToolStripMenuItem();
        this._toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        this._mnuShowShemaTable = new System.Windows.Forms.ToolStripMenuItem();
        this._executeQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuExecuteQuerySingleRow = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuExecuteQuerySchemaOnly = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuExecuteQueryKeyInfo = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuExecuteQueryXml = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuOpenTable = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCancel = new System.Windows.Forms.ToolStripMenuItem();
        this._parseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        this._menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuText = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuDataGrid = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuHtml = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuRtf = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuListView = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuExcel = new System.Windows.Forms.ToolStripMenuItem();
        this._menuResultModeFile = new System.Windows.Forms.ToolStripMenuItem();
        this._sQLiteDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._insertScriptFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        this._mnuGotoQueryEditor = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuGotoMessageTabPage = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCloseTabPage = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCloseAllTabPages = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCreateInsert = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuCreateInsertSelect = new System.Windows.Forms.ToolStripMenuItem();
        this._createSqlCeDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._beginTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._commitTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._rollbackTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.createCCommandQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuObjectExplorer = new System.Windows.Forms.ToolStripMenuItem();
        this._mnuRefreshObjectExplorer = new System.Windows.Forms.ToolStripMenuItem();
        this._statusBar = new System.Windows.Forms.StatusStrip();
        this._sbPanelText = new System.Windows.Forms.ToolStripStatusLabel();
        this._sbPanelTableStyle = new System.Windows.Forms.ToolStripStatusLabel();
        this._sbPanelTimer = new System.Windows.Forms.ToolStripStatusLabel();
        this._sbPanelRows = new System.Windows.Forms.ToolStripStatusLabel();
        this._sbPanelCaretPosition = new System.Windows.Forms.ToolStripStatusLabel();
        this._tvObjectExplorer = new System.Windows.Forms.TreeView();
        this._splitterObjectExplorer = new System.Windows.Forms.Splitter();
        this._splitterQuery = new System.Windows.Forms.Splitter();
        this._tabControl = new System.Windows.Forms.TabControl();
        this._toolStrip = new System.Windows.Forms.ToolStrip();
        this._toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
        this._executeQuerySplitButton = new System.Windows.Forms.ToolStripSplitButton();
        this._executeQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._executeQuerySingleRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._openTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this._cancelQueryButton = new System.Windows.Forms.ToolStripButton();
        this._queryTextBox = new QueryTextBox();
        this._mainMenu.SuspendLayout();
        this._statusBar.SuspendLayout();
        this._toolStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // _mainMenu
        // 
        this._mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItem9,
            this._menuItem8,
            this._menuItem1,
            this._menuItem3});
        this._mainMenu.Location = new System.Drawing.Point(0, 0);
        this._mainMenu.Name = "_mainMenu";
        this._mainMenu.Size = new System.Drawing.Size(1016, 24);
        this._mainMenu.TabIndex = 0;
        this._mainMenu.Visible = false;
        // 
        // _menuItem9
        // 
        this._menuItem9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuSave,
            this._mnuSaveAs,
            this._mnuDuplicateConnection});
        this._menuItem9.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
        this._menuItem9.MergeIndex = 0;
        this._menuItem9.Name = "_menuItem9";
        this._menuItem9.Size = new System.Drawing.Size(37, 20);
        this._menuItem9.Text = "&File";
        // 
        // _mnuSave
        // 
        this._mnuSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._mnuSave.MergeIndex = 2;
        this._mnuSave.Name = "_mnuSave";
        this._mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
        this._mnuSave.Size = new System.Drawing.Size(230, 22);
        this._mnuSave.Text = "&Save";
        this._mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
        // 
        // _mnuSaveAs
        // 
        this._mnuSaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._mnuSaveAs.MergeIndex = 3;
        this._mnuSaveAs.Name = "_mnuSaveAs";
        this._mnuSaveAs.Size = new System.Drawing.Size(230, 22);
        this._mnuSaveAs.Text = "Save &As";
        this._mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
        // 
        // _mnuDuplicateConnection
        // 
        this._mnuDuplicateConnection.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._mnuDuplicateConnection.MergeIndex = 4;
        this._mnuDuplicateConnection.Name = "_mnuDuplicateConnection";
        this._mnuDuplicateConnection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
        this._mnuDuplicateConnection.Size = new System.Drawing.Size(230, 22);
        this._mnuDuplicateConnection.Text = "Duplicate connection";
        this._mnuDuplicateConnection.Click += new System.EventHandler(this.mnuDuplicateConnection_Click);
        // 
        // _menuItem8
        // 
        this._menuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuPaste,
            this._mnuFind,
            this._mnuFindNext,
            this._mnuCodeCompletion,
            this._mnuGoTo,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
        this._menuItem8.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._menuItem8.MergeIndex = 2;
        this._menuItem8.Name = "_menuItem8";
        this._menuItem8.Size = new System.Drawing.Size(39, 20);
        this._menuItem8.Text = "&Edit";
        // 
        // _mnuPaste
        // 
        this._mnuPaste.Image = ((System.Drawing.Image)(resources.GetObject("_mnuPaste.Image")));
        this._mnuPaste.MergeIndex = 0;
        this._mnuPaste.Name = "_mnuPaste";
        this._mnuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
        this._mnuPaste.Size = new System.Drawing.Size(166, 22);
        this._mnuPaste.Text = "&Paste";
        // 
        // _mnuFind
        // 
        this._mnuFind.Image = ((System.Drawing.Image)(resources.GetObject("_mnuFind.Image")));
        this._mnuFind.MergeIndex = 1;
        this._mnuFind.Name = "_mnuFind";
        this._mnuFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
        this._mnuFind.Size = new System.Drawing.Size(166, 22);
        this._mnuFind.Text = "&Find";
        // 
        // _mnuFindNext
        // 
        this._mnuFindNext.MergeIndex = 2;
        this._mnuFindNext.Name = "_mnuFindNext";
        this._mnuFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
        this._mnuFindNext.Size = new System.Drawing.Size(166, 22);
        this._mnuFindNext.Text = "Find &Next";
        // 
        // _mnuCodeCompletion
        // 
        this._mnuCodeCompletion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuListMembers,
            this._mnuClearCache});
        this._mnuCodeCompletion.MergeIndex = 3;
        this._mnuCodeCompletion.Name = "_mnuCodeCompletion";
        this._mnuCodeCompletion.Size = new System.Drawing.Size(166, 22);
        this._mnuCodeCompletion.Text = "&Code completion";
        // 
        // _mnuListMembers
        // 
        this._mnuListMembers.MergeIndex = 0;
        this._mnuListMembers.Name = "_mnuListMembers";
        this._mnuListMembers.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
        this._mnuListMembers.Size = new System.Drawing.Size(211, 22);
        this._mnuListMembers.Text = "&List Members";
        this._mnuListMembers.Click += new System.EventHandler(this.mnuListMembers_Click);
        // 
        // _mnuClearCache
        // 
        this._mnuClearCache.MergeIndex = 1;
        this._mnuClearCache.Name = "_mnuClearCache";
        this._mnuClearCache.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                         | System.Windows.Forms.Keys.C)));
        this._mnuClearCache.Size = new System.Drawing.Size(211, 22);
        this._mnuClearCache.Text = "&Clear Cache";
        // 
        // _mnuGoTo
        // 
        this._mnuGoTo.MergeIndex = 4;
        this._mnuGoTo.Name = "_mnuGoTo";
        this._mnuGoTo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
        this._mnuGoTo.Size = new System.Drawing.Size(166, 22);
        this._mnuGoTo.Text = "Go To...";
        // 
        // undoToolStripMenuItem
        // 
        this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
        this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
        this.undoToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
        this.undoToolStripMenuItem.Text = "Undo";
        this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
        // 
        // redoToolStripMenuItem
        // 
        this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
        this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
        this.redoToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
        this.redoToolStripMenuItem.Text = "Redo";
        this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
        // 
        // _menuItem1
        // 
        this._menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItem7,
            this._mnuDescribeParameters,
            this._toolStripSeparator2,
            this._mnuShowShemaTable,
            this._executeQueryToolStripMenuItem,
            this._mnuExecuteQuerySingleRow,
            this._mnuExecuteQuerySchemaOnly,
            this._mnuExecuteQueryKeyInfo,
            this._mnuExecuteQueryXml,
            this._mnuOpenTable,
            this._mnuCancel,
            this._parseToolStripMenuItem,
            this._toolStripSeparator1,
            this._menuItem2,
            this._toolStripSeparator3,
            this._mnuGotoQueryEditor,
            this._mnuGotoMessageTabPage,
            this._mnuCloseTabPage,
            this._mnuCloseAllTabPages,
            this._mnuCreateInsert,
            this._mnuCreateInsertSelect,
            this._createSqlCeDatabaseToolStripMenuItem,
            this._exportToolStripMenuItem,
            this._beginTransactionToolStripMenuItem,
            this._commitTransactionToolStripMenuItem,
            this._rollbackTransactionToolStripMenuItem,
            this.createCCommandQueryToolStripMenuItem});
        this._menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._menuItem1.MergeIndex = 3;
        this._menuItem1.Name = "_menuItem1";
        this._menuItem1.Size = new System.Drawing.Size(51, 20);
        this._menuItem1.Text = "&Query";
        // 
        // _menuItem7
        // 
        this._menuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuCommandTypeText,
            this._mnuCommandTypeStoredProcedure});
        this._menuItem7.MergeIndex = 0;
        this._menuItem7.Name = "_menuItem7";
        this._menuItem7.Size = new System.Drawing.Size(298, 22);
        this._menuItem7.Text = "Command&Type";
        // 
        // _mnuCommandTypeText
        // 
        this._mnuCommandTypeText.Checked = true;
        this._mnuCommandTypeText.CheckState = System.Windows.Forms.CheckState.Checked;
        this._mnuCommandTypeText.MergeIndex = 0;
        this._mnuCommandTypeText.Name = "_mnuCommandTypeText";
        this._mnuCommandTypeText.Size = new System.Drawing.Size(165, 22);
        this._mnuCommandTypeText.Text = "Text";
        this._mnuCommandTypeText.Click += new System.EventHandler(this.mnuCommandTypeText_Click);
        // 
        // _mnuCommandTypeStoredProcedure
        // 
        this._mnuCommandTypeStoredProcedure.MergeIndex = 1;
        this._mnuCommandTypeStoredProcedure.Name = "_mnuCommandTypeStoredProcedure";
        this._mnuCommandTypeStoredProcedure.Size = new System.Drawing.Size(165, 22);
        this._mnuCommandTypeStoredProcedure.Text = "Stored Procedure";
        this._mnuCommandTypeStoredProcedure.Click += new System.EventHandler(this.mnuCommandTypeStoredProcedure_Click);
        // 
        // _mnuDescribeParameters
        // 
        this._mnuDescribeParameters.MergeIndex = 1;
        this._mnuDescribeParameters.Name = "_mnuDescribeParameters";
        this._mnuDescribeParameters.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
        this._mnuDescribeParameters.Size = new System.Drawing.Size(298, 22);
        this._mnuDescribeParameters.Text = "Describe &Parameters";
        this._mnuDescribeParameters.Click += new System.EventHandler(this.mnuDescribeParameters_Click);
        // 
        // _toolStripSeparator2
        // 
        this._toolStripSeparator2.Name = "_toolStripSeparator2";
        this._toolStripSeparator2.Size = new System.Drawing.Size(295, 6);
        // 
        // _mnuShowShemaTable
        // 
        this._mnuShowShemaTable.MergeIndex = 3;
        this._mnuShowShemaTable.Name = "_mnuShowShemaTable";
        this._mnuShowShemaTable.Size = new System.Drawing.Size(298, 22);
        this._mnuShowShemaTable.Text = "Show SchemaTable";
        this._mnuShowShemaTable.Click += new System.EventHandler(this.mnuShowShemaTable_Click);
        // 
        // _executeQueryToolStripMenuItem
        // 
        this._executeQueryToolStripMenuItem.Name = "_executeQueryToolStripMenuItem";
        this._executeQueryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
        this._executeQueryToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._executeQueryToolStripMenuItem.Text = "Execute Query";
        this._executeQueryToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
        // 
        // _mnuExecuteQuerySingleRow
        // 
        this._mnuExecuteQuerySingleRow.MergeIndex = 6;
        this._mnuExecuteQuerySingleRow.Name = "_mnuExecuteQuerySingleRow";
        this._mnuExecuteQuerySingleRow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
        this._mnuExecuteQuerySingleRow.Size = new System.Drawing.Size(298, 22);
        this._mnuExecuteQuerySingleRow.Text = "Execute Query (SingleRow)";
        this._mnuExecuteQuerySingleRow.Click += new System.EventHandler(this.mnuSingleRow_Click);
        // 
        // _mnuExecuteQuerySchemaOnly
        // 
        this._mnuExecuteQuerySchemaOnly.MergeIndex = 7;
        this._mnuExecuteQuerySchemaOnly.Name = "_mnuExecuteQuerySchemaOnly";
        this._mnuExecuteQuerySchemaOnly.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
        this._mnuExecuteQuerySchemaOnly.Size = new System.Drawing.Size(298, 22);
        this._mnuExecuteQuerySchemaOnly.Text = "Execute Query (Schema only)";
        this._mnuExecuteQuerySchemaOnly.Click += new System.EventHandler(this.mnuResultSchema_Click);
        // 
        // _mnuExecuteQueryKeyInfo
        // 
        this._mnuExecuteQueryKeyInfo.MergeIndex = 8;
        this._mnuExecuteQueryKeyInfo.Name = "_mnuExecuteQueryKeyInfo";
        this._mnuExecuteQueryKeyInfo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
        this._mnuExecuteQueryKeyInfo.Size = new System.Drawing.Size(298, 22);
        this._mnuExecuteQueryKeyInfo.Text = "Execute Query (&KeyInfo)";
        this._mnuExecuteQueryKeyInfo.Click += new System.EventHandler(this.mnuKeyInfo_Click);
        // 
        // _mnuExecuteQueryXml
        // 
        this._mnuExecuteQueryXml.MergeIndex = 9;
        this._mnuExecuteQueryXml.Name = "_mnuExecuteQueryXml";
        this._mnuExecuteQueryXml.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                              | System.Windows.Forms.Keys.X)));
        this._mnuExecuteQueryXml.Size = new System.Drawing.Size(298, 22);
        this._mnuExecuteQueryXml.Text = "Execute Query (XML)";
        this._mnuExecuteQueryXml.Click += new System.EventHandler(this.mnuXml_Click);
        // 
        // _mnuOpenTable
        // 
        this._mnuOpenTable.MergeIndex = 10;
        this._mnuOpenTable.Name = "_mnuOpenTable";
        this._mnuOpenTable.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                        | System.Windows.Forms.Keys.O)));
        this._mnuOpenTable.Size = new System.Drawing.Size(298, 22);
        this._mnuOpenTable.Text = "Edit Rows";
        this._mnuOpenTable.Click += new System.EventHandler(this.EditRows_Click);
        // 
        // _mnuCancel
        // 
        this._mnuCancel.Enabled = false;
        this._mnuCancel.MergeIndex = 11;
        this._mnuCancel.Name = "_mnuCancel";
        this._mnuCancel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Pause)));
        this._mnuCancel.Size = new System.Drawing.Size(298, 22);
        this._mnuCancel.Text = "&Cancel Executing Query";
        this._mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
        // 
        // _parseToolStripMenuItem
        // 
        this._parseToolStripMenuItem.Name = "_parseToolStripMenuItem";
        this._parseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
        this._parseToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._parseToolStripMenuItem.Text = "Parse";
        this._parseToolStripMenuItem.Click += new System.EventHandler(this.parseToolStripMenuItem_Click);
        // 
        // _toolStripSeparator1
        // 
        this._toolStripSeparator1.Name = "_toolStripSeparator1";
        this._toolStripSeparator1.Size = new System.Drawing.Size(295, 6);
        // 
        // _menuItem2
        // 
        this._menuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuText,
            this._mnuDataGrid,
            this._mnuHtml,
            this._mnuRtf,
            this._mnuListView,
            this._mnuExcel,
            this._menuResultModeFile,
            this._sQLiteDatabaseToolStripMenuItem,
            this._insertScriptFileToolStripMenuItem});
        this._menuItem2.MergeIndex = 13;
        this._menuItem2.Name = "_menuItem2";
        this._menuItem2.Size = new System.Drawing.Size(298, 22);
        this._menuItem2.Text = "Result &Mode";
        // 
        // _mnuText
        // 
        this._mnuText.MergeIndex = 0;
        this._mnuText.Name = "_mnuText";
        this._mnuText.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
        this._mnuText.Size = new System.Drawing.Size(162, 22);
        this._mnuText.Text = "&Text";
        this._mnuText.Click += new System.EventHandler(this.mnuText_Click);
        // 
        // _mnuDataGrid
        // 
        this._mnuDataGrid.MergeIndex = 1;
        this._mnuDataGrid.Name = "_mnuDataGrid";
        this._mnuDataGrid.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
        this._mnuDataGrid.Size = new System.Drawing.Size(162, 22);
        this._mnuDataGrid.Text = "&DataGrid";
        this._mnuDataGrid.Click += new System.EventHandler(this.mnuDataGrid_Click);
        // 
        // _mnuHtml
        // 
        this._mnuHtml.MergeIndex = 2;
        this._mnuHtml.Name = "_mnuHtml";
        this._mnuHtml.Size = new System.Drawing.Size(162, 22);
        this._mnuHtml.Text = "&Html";
        this._mnuHtml.Click += new System.EventHandler(this.mnuHtml_Click);
        // 
        // _mnuRtf
        // 
        this._mnuRtf.MergeIndex = 3;
        this._mnuRtf.Name = "_mnuRtf";
        this._mnuRtf.Size = new System.Drawing.Size(162, 22);
        this._mnuRtf.Text = "&Rtf";
        // 
        // _mnuListView
        // 
        this._mnuListView.MergeIndex = 4;
        this._mnuListView.Name = "_mnuListView";
        this._mnuListView.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
        this._mnuListView.Size = new System.Drawing.Size(162, 22);
        this._mnuListView.Text = "&ListView";
        this._mnuListView.Click += new System.EventHandler(this.mnuListView_Click);
        // 
        // _mnuExcel
        // 
        this._mnuExcel.MergeIndex = 5;
        this._mnuExcel.Name = "_mnuExcel";
        this._mnuExcel.Size = new System.Drawing.Size(162, 22);
        this._mnuExcel.Text = "&Excel";
        this._mnuExcel.Visible = false;
        // 
        // _menuResultModeFile
        // 
        this._menuResultModeFile.MergeIndex = 6;
        this._menuResultModeFile.Name = "_menuResultModeFile";
        this._menuResultModeFile.Size = new System.Drawing.Size(162, 22);
        this._menuResultModeFile.Text = "&File";
        this._menuResultModeFile.Click += new System.EventHandler(this.menuResultModeFile_Click);
        // 
        // _sQLiteDatabaseToolStripMenuItem
        // 
        this._sQLiteDatabaseToolStripMenuItem.Name = "_sQLiteDatabaseToolStripMenuItem";
        this._sQLiteDatabaseToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        this._sQLiteDatabaseToolStripMenuItem.Text = "SQLite database";
        this._sQLiteDatabaseToolStripMenuItem.Click += new System.EventHandler(this.sQLiteDatabaseToolStripMenuItem_Click);
        // 
        // _insertScriptFileToolStripMenuItem
        // 
        this._insertScriptFileToolStripMenuItem.Name = "_insertScriptFileToolStripMenuItem";
        this._insertScriptFileToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        this._insertScriptFileToolStripMenuItem.Text = "Insert Script File";
        this._insertScriptFileToolStripMenuItem.Click += new System.EventHandler(this.insertScriptFileToolStripMenuItem_Click);
        // 
        // _toolStripSeparator3
        // 
        this._toolStripSeparator3.Name = "_toolStripSeparator3";
        this._toolStripSeparator3.Size = new System.Drawing.Size(295, 6);
        // 
        // _mnuGotoQueryEditor
        // 
        this._mnuGotoQueryEditor.MergeIndex = 15;
        this._mnuGotoQueryEditor.Name = "_mnuGotoQueryEditor";
        this._mnuGotoQueryEditor.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                              | System.Windows.Forms.Keys.Q)));
        this._mnuGotoQueryEditor.Size = new System.Drawing.Size(298, 22);
        this._mnuGotoQueryEditor.Text = "Goto &Query Editor";
        this._mnuGotoQueryEditor.Click += new System.EventHandler(this.mnuGotoQueryEditor_Click);
        // 
        // _mnuGotoMessageTabPage
        // 
        this._mnuGotoMessageTabPage.MergeIndex = 16;
        this._mnuGotoMessageTabPage.Name = "_mnuGotoMessageTabPage";
        this._mnuGotoMessageTabPage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
        this._mnuGotoMessageTabPage.Size = new System.Drawing.Size(298, 22);
        this._mnuGotoMessageTabPage.Text = "Goto &Message TabPage";
        this._mnuGotoMessageTabPage.Click += new System.EventHandler(this.mnuGotoMessageTabPage_Click);
        // 
        // _mnuCloseTabPage
        // 
        this._mnuCloseTabPage.MergeIndex = 17;
        this._mnuCloseTabPage.Name = "_mnuCloseTabPage";
        this._mnuCloseTabPage.Size = new System.Drawing.Size(298, 22);
        this._mnuCloseTabPage.Text = "Close Current &TabPage";
        this._mnuCloseTabPage.Click += new System.EventHandler(this.mnuCloseTabPage_Click);
        // 
        // _mnuCloseAllTabPages
        // 
        this._mnuCloseAllTabPages.MergeIndex = 18;
        this._mnuCloseAllTabPages.Name = "_mnuCloseAllTabPages";
        this._mnuCloseAllTabPages.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                               | System.Windows.Forms.Keys.F4)));
        this._mnuCloseAllTabPages.Size = new System.Drawing.Size(298, 22);
        this._mnuCloseAllTabPages.Text = "Close &All TabPages";
        this._mnuCloseAllTabPages.Click += new System.EventHandler(this.mnuCloseAllTabPages_Click);
        // 
        // _mnuCreateInsert
        // 
        this._mnuCreateInsert.MergeIndex = 19;
        this._mnuCreateInsert.Name = "_mnuCreateInsert";
        this._mnuCreateInsert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
        this._mnuCreateInsert.Size = new System.Drawing.Size(298, 22);
        this._mnuCreateInsert.Text = "Create insert statements";
        this._mnuCreateInsert.Click += new System.EventHandler(this.mnuCreateInsert_Click);
        // 
        // _mnuCreateInsertSelect
        // 
        this._mnuCreateInsertSelect.MergeIndex = 20;
        this._mnuCreateInsertSelect.Name = "_mnuCreateInsertSelect";
        this._mnuCreateInsertSelect.Size = new System.Drawing.Size(298, 22);
        this._mnuCreateInsertSelect.Text = "Create \'insert select\' statements";
        this._mnuCreateInsertSelect.Click += new System.EventHandler(this.mnuCreateInsertSelect_Click);
        // 
        // _createSqlCeDatabaseToolStripMenuItem
        // 
        this._createSqlCeDatabaseToolStripMenuItem.Name = "_createSqlCeDatabaseToolStripMenuItem";
        this._createSqlCeDatabaseToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._createSqlCeDatabaseToolStripMenuItem.Text = "Create SQL Server Compact database";
        this._createSqlCeDatabaseToolStripMenuItem.Click += new System.EventHandler(this.createSqlCeDatabaseToolStripMenuItem_Click);
        // 
        // _exportToolStripMenuItem
        // 
        this._exportToolStripMenuItem.Name = "_exportToolStripMenuItem";
        this._exportToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        // 
        // _beginTransactionToolStripMenuItem
        // 
        this._beginTransactionToolStripMenuItem.Name = "_beginTransactionToolStripMenuItem";
        this._beginTransactionToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._beginTransactionToolStripMenuItem.Text = "Begin Transaction";
        this._beginTransactionToolStripMenuItem.Click += new System.EventHandler(this.beginTransactionToolStripMenuItem_Click);
        // 
        // _commitTransactionToolStripMenuItem
        // 
        this._commitTransactionToolStripMenuItem.Enabled = false;
        this._commitTransactionToolStripMenuItem.Name = "_commitTransactionToolStripMenuItem";
        this._commitTransactionToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._commitTransactionToolStripMenuItem.Text = "Commit Transaction";
        this._commitTransactionToolStripMenuItem.Click += new System.EventHandler(this.commitTransactionToolStripMenuItem_Click);
        // 
        // _rollbackTransactionToolStripMenuItem
        // 
        this._rollbackTransactionToolStripMenuItem.Enabled = false;
        this._rollbackTransactionToolStripMenuItem.Name = "_rollbackTransactionToolStripMenuItem";
        this._rollbackTransactionToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this._rollbackTransactionToolStripMenuItem.Text = "Rollback Transaction";
        this._rollbackTransactionToolStripMenuItem.Click += new System.EventHandler(this.rollbackTransactionToolStripMenuItem_Click);
        // 
        // createCCommandQueryToolStripMenuItem
        // 
        this.createCCommandQueryToolStripMenuItem.Name = "createCCommandQueryToolStripMenuItem";
        this.createCCommandQueryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                                               | System.Windows.Forms.Keys.Q)));
        this.createCCommandQueryToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this.createCCommandQueryToolStripMenuItem.Text = "Create C# Command/Query";
        this.createCCommandQueryToolStripMenuItem.Click += new System.EventHandler(this.createCCommandQueryToolStripMenuItem_Click);
        // 
        // _menuItem3
        // 
        this._menuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuObjectExplorer,
            this._mnuRefreshObjectExplorer});
        this._menuItem3.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._menuItem3.MergeIndex = 4;
        this._menuItem3.Name = "_menuItem3";
        this._menuItem3.Size = new System.Drawing.Size(47, 20);
        this._menuItem3.Text = "&Tools";
        // 
        // _mnuObjectExplorer
        // 
        this._mnuObjectExplorer.MergeIndex = 0;
        this._mnuObjectExplorer.Name = "_mnuObjectExplorer";
        this._mnuObjectExplorer.ShortcutKeys = System.Windows.Forms.Keys.F8;
        this._mnuObjectExplorer.Size = new System.Drawing.Size(229, 22);
        this._mnuObjectExplorer.Text = "Object Explorer";
        this._mnuObjectExplorer.Click += new System.EventHandler(this.menuObjectExplorer_Click);
        // 
        // _mnuRefreshObjectExplorer
        // 
        this._mnuRefreshObjectExplorer.MergeIndex = 1;
        this._mnuRefreshObjectExplorer.Name = "_mnuRefreshObjectExplorer";
        this._mnuRefreshObjectExplorer.Size = new System.Drawing.Size(229, 22);
        this._mnuRefreshObjectExplorer.Text = "Refresh Object Explorer\'s root";
        this._mnuRefreshObjectExplorer.Click += new System.EventHandler(this.mnuRefreshObjectExplorer_Click);
        // 
        // _statusBar
        // 
        this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sbPanelText,
            this._sbPanelTableStyle,
            this._sbPanelTimer,
            this._sbPanelRows,
            this._sbPanelCaretPosition});
        this._statusBar.Location = new System.Drawing.Point(300, 543);
        this._statusBar.Name = "_statusBar";
        this._statusBar.Size = new System.Drawing.Size(716, 22);
        this._statusBar.TabIndex = 2;
        // 
        // _sbPanelText
        // 
        this._sbPanelText.AutoSize = false;
        this._sbPanelText.Name = "_sbPanelText";
        this._sbPanelText.Size = new System.Drawing.Size(231, 17);
        this._sbPanelText.Spring = true;
        this._sbPanelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // _sbPanelTableStyle
        // 
        this._sbPanelTableStyle.AutoSize = false;
        this._sbPanelTableStyle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        this._sbPanelTableStyle.Name = "_sbPanelTableStyle";
        this._sbPanelTableStyle.Size = new System.Drawing.Size(100, 17);
        this._sbPanelTableStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this._sbPanelTableStyle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sbPanelTableStyle_MouseUp);
        // 
        // _sbPanelTimer
        // 
        this._sbPanelTimer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        this._sbPanelTimer.AutoSize = false;
        this._sbPanelTimer.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        this._sbPanelTimer.Name = "_sbPanelTimer";
        this._sbPanelTimer.Size = new System.Drawing.Size(70, 17);
        this._sbPanelTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // _sbPanelRows
        // 
        this._sbPanelRows.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        this._sbPanelRows.AutoSize = false;
        this._sbPanelRows.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        this._sbPanelRows.Name = "_sbPanelRows";
        this._sbPanelRows.Size = new System.Drawing.Size(200, 17);
        this._sbPanelRows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // _sbPanelCaretPosition
        // 
        this._sbPanelCaretPosition.AutoSize = false;
        this._sbPanelCaretPosition.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        this._sbPanelCaretPosition.Name = "_sbPanelCaretPosition";
        this._sbPanelCaretPosition.Size = new System.Drawing.Size(100, 17);
        this._sbPanelCaretPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // _tvObjectExplorer
        // 
        this._tvObjectExplorer.Dock = System.Windows.Forms.DockStyle.Left;
        this._tvObjectExplorer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this._tvObjectExplorer.Location = new System.Drawing.Point(0, 24);
        this._tvObjectExplorer.Name = "_tvObjectExplorer";
        this._tvObjectExplorer.Size = new System.Drawing.Size(300, 541);
        this._tvObjectExplorer.TabIndex = 4;
        this._tvObjectExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvObjectBrowser_BeforeExpand);
        this._tvObjectExplorer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvObjectBrowser_ItemDrag);
        this._tvObjectExplorer.DoubleClick += new System.EventHandler(this.tvObjectBrowser_DoubleClick);
        this._tvObjectExplorer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvObjectBrowser_MouseDown);
        this._tvObjectExplorer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvObjectExplorer_MouseUp);
        // 
        // _splitterObjectExplorer
        // 
        this._splitterObjectExplorer.Location = new System.Drawing.Point(300, 24);
        this._splitterObjectExplorer.Name = "_splitterObjectExplorer";
        this._splitterObjectExplorer.Size = new System.Drawing.Size(3, 519);
        this._splitterObjectExplorer.TabIndex = 5;
        this._splitterObjectExplorer.TabStop = false;
        // 
        // _splitterQuery
        // 
        this._splitterQuery.Dock = System.Windows.Forms.DockStyle.Top;
        this._splitterQuery.Location = new System.Drawing.Point(303, 303);
        this._splitterQuery.Name = "_splitterQuery";
        this._splitterQuery.Size = new System.Drawing.Size(713, 2);
        this._splitterQuery.TabIndex = 7;
        this._splitterQuery.TabStop = false;
        // 
        // _tabControl
        // 
        this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this._tabControl.Location = new System.Drawing.Point(303, 305);
        this._tabControl.Name = "_tabControl";
        this._tabControl.SelectedIndex = 0;
        this._tabControl.ShowToolTips = true;
        this._tabControl.Size = new System.Drawing.Size(713, 238);
        this._tabControl.TabIndex = 8;
        // 
        // _toolStrip
        // 
        this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
        this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripSeparator4,
            this._executeQuerySplitButton,
            this._cancelQueryButton});
        this._toolStrip.Location = new System.Drawing.Point(303, 281);
        this._toolStrip.Name = "_toolStrip";
        this._toolStrip.Size = new System.Drawing.Size(73, 25);
        this._toolStrip.TabIndex = 9;
        this._toolStrip.Text = "toolStrip1";
        // 
        // _toolStripSeparator4
        // 
        this._toolStripSeparator4.Name = "_toolStripSeparator4";
        this._toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
        // 
        // _executeQuerySplitButton
        // 
        this._executeQuerySplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._executeQuerySplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._executeQueryMenuItem,
            this._executeQuerySingleRowToolStripMenuItem,
            this._cToolStripMenuItem,
            this._openTableToolStripMenuItem});
        this._executeQuerySplitButton.Image = ((System.Drawing.Image)(resources.GetObject("_executeQuerySplitButton.Image")));
        this._executeQuerySplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._executeQuerySplitButton.Name = "_executeQuerySplitButton";
        this._executeQuerySplitButton.Size = new System.Drawing.Size(32, 22);
        this._executeQuerySplitButton.Text = "Execute Query";
        this._executeQuerySplitButton.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
        // 
        // _executeQueryMenuItem
        // 
        this._executeQueryMenuItem.Name = "_executeQueryMenuItem";
        this._executeQueryMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
        this._executeQueryMenuItem.Size = new System.Drawing.Size(168, 22);
        this._executeQueryMenuItem.Text = "Execute Query";
        this._executeQueryMenuItem.Click += new System.EventHandler(this.aToolStripMenuItem_Click);
        // 
        // _executeQuerySingleRowToolStripMenuItem
        // 
        this._executeQuerySingleRowToolStripMenuItem.Name = "_executeQuerySingleRowToolStripMenuItem";
        this._executeQuerySingleRowToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        this._executeQuerySingleRowToolStripMenuItem.Text = "Single Row";
        this._executeQuerySingleRowToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
        // 
        // _cToolStripMenuItem
        // 
        this._cToolStripMenuItem.Name = "_cToolStripMenuItem";
        this._cToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        this._cToolStripMenuItem.Text = "XML";
        // 
        // _openTableToolStripMenuItem
        // 
        this._openTableToolStripMenuItem.Name = "_openTableToolStripMenuItem";
        this._openTableToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        this._openTableToolStripMenuItem.Text = "Edit Rows2";
        this._openTableToolStripMenuItem.Click += new System.EventHandler(this.editRowsToolStripMenuItem_Click);
        // 
        // _cancelQueryButton
        // 
        this._cancelQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._cancelQueryButton.Enabled = false;
        this._cancelQueryButton.Image = ((System.Drawing.Image)(resources.GetObject("_cancelQueryButton.Image")));
        this._cancelQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._cancelQueryButton.Name = "_cancelQueryButton";
        this._cancelQueryButton.Size = new System.Drawing.Size(23, 22);
        this._cancelQueryButton.Text = "Cancel Executing Query";
        this._cancelQueryButton.Click += new System.EventHandler(this.cancelExecutingQueryButton_Click);
        // 
        // _queryTextBox
        // 
        this._queryTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        this._queryTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this._queryTextBox.Location = new System.Drawing.Point(303, 24);
        this._queryTextBox.Name = "_queryTextBox";
        this._queryTextBox.Size = new System.Drawing.Size(713, 279);
        this._queryTextBox.TabIndex = 1;
        this._queryTextBox.TabSize = 4;
        // 
        // QueryForm
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
        this.ClientSize = new System.Drawing.Size(1016, 565);
        this.Controls.Add(this._toolStrip);
        this.Controls.Add(this._tabControl);
        this.Controls.Add(this._splitterQuery);
        this.Controls.Add(this._queryTextBox);
        this.Controls.Add(this._splitterObjectExplorer);
        this.Controls.Add(this._statusBar);
        this.Controls.Add(this._tvObjectExplorer);
        this.Controls.Add(this._mainMenu);
        this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MainMenuStrip = this._mainMenu;
        this.Name = "QueryForm";
        this._mainMenu.ResumeLayout(false);
        this._mainMenu.PerformLayout();
        this._statusBar.ResumeLayout(false);
        this._statusBar.PerformLayout();
        this._toolStrip.ResumeLayout(false);
        this._toolStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    #region Private Methods

    private void AddNodes(TreeNodeCollection parent, IEnumerable<ITreeNode> children, bool sortable)
    {
        var ticks = Stopwatch.GetTimestamp();
        IEnumerable<ITreeNode> enumerableChildren;

        if (sortable)
        {
            enumerableChildren = from c in children
                orderby c.Name
                select c;
        }
        else
        {
            enumerableChildren = children;
        }

        var count = 0;

        if (children != null)
        {
            foreach (var child in children)
            {
                var treeNode = new TreeNode(child.Name);
                treeNode.Tag = child;
                //treeNode.ImageIndex

                if (!child.IsLeaf)
                    treeNode.Nodes.Add(new TreeNode());

                parent.Add(treeNode);
                count++;
            }
        }

        ticks = Stopwatch.GetTimestamp() - ticks;
        SetStatusbarPanelText($"{count} item(s) added to Object Explorer in {StopwatchTimeSpan.ToString(ticks, 3)}.",
            _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
    }

    public void AddInfoMessage(InfoMessage infoMessage)
    {
        WriteInfoMessageToLog(infoMessage);

        if (infoMessage.Severity == InfoMessageSeverity.Error)
            _errorCount++;

        _infoMessages.Enqueue(infoMessage);
        _enqueueEvent.Set();
    }

    private void AddInfoMessages(IReadOnlyCollection<InfoMessage> infoMessages)
    {
        foreach (var infoMessage in infoMessages)
            WriteInfoMessageToLog(infoMessage);

        var errorCount =
            (from infoMessage in infoMessages
                where infoMessage.Severity == InfoMessageSeverity.Error
                select infoMessage).Count();
        _errorCount += errorCount;

        foreach (var infoMessage in infoMessages)
            _infoMessages.Enqueue(infoMessage);

        _enqueueEvent.Set();
    }

    private void AppendMessageText(DateTime dateTime, InfoMessageSeverity severity, string header, string text)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append('[');
        stringBuilder.Append(dateTime.ToString("HH:mm:ss.fff"));
        if (!string.IsNullOrEmpty(header))
        {
            stringBuilder.Append(' ');
            stringBuilder.Append(header);
        }

        if (severity == InfoMessageSeverity.Error)
            stringBuilder.Append(",Error");
        stringBuilder.Append("] ");
        stringBuilder.Append(text);
        stringBuilder.AppendLine();

        _messagesTextBox.AppendText(stringBuilder.ToString());
    }

    private void Connection_InfoMessage(IReadOnlyCollection<InfoMessage> messages) => AddInfoMessages(messages);

    internal static string DbValue(object value)
    {
        var s = value == DBNull.Value ? "(null)" : value.ToString();
        return s;
    }

    private string GetToolTipText(DataTable dataTable)
    {
        var sb = new StringBuilder();

        if (_command != null)
            sb.Append(_command.CommandText + "\n");

        if (dataTable != null)
            sb.Append(dataTable.Rows.Count + " row(s)");

        return sb.ToString();
    }

    private void SettingsChanged(object sender, EventArgs e)
    {
        var folder = Settings.CurrentType;
        _commandTimeout = folder.Attributes["CommandTimeout"].GetValue<int>();
    }

    private void SetText()
    {
        var sb = new StringBuilder();
        sb.Append(Connection.ConnectionName);
        sb.Append(" - ");
        sb.Append(Connection.Caption);

        if (_fileName != null)
            sb.AppendFormat(" - {0}", _fileName);

        Text = sb.ToString();

        var mainForm = DataCommanderApplication.Instance.MainForm;
        mainForm.ActiveMdiChildToolStripTextBox.Text = sb.ToString();
    }

    private void sbPanelTableStyle_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var contextMenu = new ContextMenuStrip(components);
            var values = Enum.GetValues(typeof(ResultWriterType));

            for (var i = 0; i < values.Length; i++)
            {
                var tableStyle = (ResultWriterType)values.GetValue(i);
                var item = new ToolStripMenuItem();
                item.Text = tableStyle.ToString();
                item.Tag = tableStyle;
                item.Click += TableStyleMenuItem_Click;
                contextMenu.Items.Add(item);
            }

            var bounds = _sbPanelTableStyle.Bounds;
            var location = e.Location;
            contextMenu.Show(_statusBar, bounds.X + location.X, bounds.Y + location.Y);
        }
    }

    private void bToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ExecuteQuerySingleRow();
    }

    private void ExecuteQuery()
    {
        var message = new string('-', 80);
        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, string.Empty, message));

        var query = Query;
        if (string.IsNullOrWhiteSpace(query))
            return;

        Log.Trace("ExecuteQuery...");

        Cursor = Cursors.AppStarting;
        SetGui(CommandState.Cancel);

        if (_dataAdapter != null)
            Log.Error("this.dataAdapter == null failed");

        Assert.IsTrue(_dataAdapter == null);

        Log.Trace("ThreadMonitor:\r\n{0}", ThreadMonitor.ToStringTableString());
        ThreadMonitor.Join(0);
        Log.Trace(GarbageMonitor.Default.State);
        _openTableMode = false;
        _cancel = false;

        try
        {
            SetStatusbarPanelText("Executing query...", _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
            var statements = Provider.GetStatements(query);
            Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
            IReadOnlyCollection<AsyncDataAdapterCommand> commands;

            if (statements.Count == 1)
            {
                IDbCommand command;

                var getQueryConfigurationResult = GetQueryConfiguration(statements[0].CommandText);
                if (getQueryConfigurationResult.Succeeded)
                {
                    command = Connection.CreateCommand();
                    command.CommandText = statements[0].CommandText;
                    command.CommandTimeout = _commandTimeout;
                }
                else
                {
                    _sqlStatement = new SqlParser(statements[0].CommandText);
                    command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
                }

                command.Transaction = _transaction;
                commands = new AsyncDataAdapterCommand(_fileName, 0, command,
                    getQueryConfigurationResult.Succeeded ? getQueryConfigurationResult.Query : null,
                    getQueryConfigurationResult.Succeeded ? getQueryConfigurationResult.Parameters : null,
                    getQueryConfigurationResult.Succeeded ? getQueryConfigurationResult.CommandText : null).ItemToArray();
            }
            else
                commands =
                    (
                        from statement in statements
                        select new AsyncDataAdapterCommand(null, statement.LineIndex,
                            Connection.Connection.CreateCommand(new CreateCommandRequest(statement.CommandText, null, CommandType.Text, _commandTimeout,
                                _transaction)), null, null, null)
                    )
                    .ToReadOnlyCollection();

            int maxRecords;
            IResultWriter resultWriter = null;

            switch (TableStyle)
            {
                case ResultWriterType.DataGrid:
                case ResultWriterType.ListView:
                    maxRecords = int.MaxValue;
                    _dataSetResultWriter = new DataSetResultWriter(AddInfoMessage, _showSchemaTable);
                    resultWriter = _dataSetResultWriter;
                    break;

                case ResultWriterType.DataGridView:
                    maxRecords = int.MaxValue;
                    resultWriter = new DataGridViewResultWriter();
                    break;

                case ResultWriterType.Excel:
                    maxRecords = int.MaxValue;
                    resultWriter = new ExcelResultWriter(Provider, AddInfoMessage);
                    _tabControl.SelectedTab = _messagesTabPage;
                    break;

                case ResultWriterType.File:
                    maxRecords = int.MaxValue;
                    resultWriter = new FileResultWriter(_textBoxWriter);
                    _tabControl.SelectedTab = _messagesTabPage;
                    break;

                case ResultWriterType.ForJsonAuto:
                    maxRecords = int.MaxValue;
                    resultWriter = new ForJsonAutoResultWriter(AddInfoMessage);
                    break;

                case ResultWriterType.Html:
                    maxRecords = _htmlMaxRecords;
                    _dataSetResultWriter = new DataSetResultWriter(AddInfoMessage, _showSchemaTable);
                    resultWriter = _dataSetResultWriter;
                    break;

                case ResultWriterType.HtmlFile:
                    maxRecords = int.MaxValue;
                    resultWriter = new HtmlResultWriter(AddInfoMessage);
                    break;

                case ResultWriterType.JsonFile:
                    maxRecords = int.MaxValue;
                    resultWriter = new JsonResultWriter(AddInfoMessage);
                    break;

                case ResultWriterType.InsertScriptFile:
                {
                    maxRecords = int.MaxValue;
                    var tableName = _sqlStatement.FindTableName();
                    resultWriter = new InsertScriptFileWriter(tableName, _textBoxWriter);
                    _tabControl.SelectedTab = _messagesTabPage;
                }
                    break;

                case ResultWriterType.Log:
                    maxRecords = int.MaxValue;
                    resultWriter = new LogResultWriter(AddInfoMessage);
                    _tabControl.SelectedTab = _messagesTabPage;
                    break;

                case ResultWriterType.Rtf:
                    maxRecords = _wordMaxRecords;
                    _dataSetResultWriter = new DataSetResultWriter(AddInfoMessage, _showSchemaTable);
                    resultWriter = _dataSetResultWriter;
                    break;

                case ResultWriterType.SqLite:
                {
                    maxRecords = int.MaxValue;
                    var tableName = _sqlStatement.FindTableName();
                    resultWriter = new SqLiteResultWriter(_textBoxWriter, tableName);
                    _tabControl.SelectedTab = _messagesTabPage;
                }
                    break;

                default:
                    maxRecords = int.MaxValue;
                    var textBox = new RichTextBox();
                    GarbageMonitor.Default.Add("ExecuteQuery.textBox", textBox);
                    textBox.MaxLength = int.MaxValue;
                    textBox.Multiline = true;
                    textBox.WordWrap = false;
                    textBox.Font = _font;
                    textBox.Dock = DockStyle.Fill;
                    textBox.ScrollBars = RichTextBoxScrollBars.Both;
                    textBox.SelectionChanged += textBox_SelectionChanged;

                    var resultSetTabPage = new TabPage("TextResult");
                    resultSetTabPage.Controls.Add(textBox);
                    _resultSetsTabControl.TabPages.Add(resultSetTabPage);
                    _resultSetsTabControl.SelectedTab = resultSetTabPage;

                    var textWriter = new TextBoxWriter(textBox);
                    resultWriter = new TextResultWriter(AddInfoMessage, textWriter, this);
                    break;
            }

            _stopwatch.Start();
            _timer.Start();
            ShowTimer();

            _errorCount = 0;
            _dataAdapter = new AsyncDataAdapter(Provider, commands, maxRecords, _rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
            _dataAdapter.Start();
        }
        catch (Exception ex)
        {
            WriteEnd(_dataAdapter);
            EndFill(_dataAdapter, ex);
        }
    }

    private sealed class GetQueryConfigurationResult
    {
        public readonly bool Succeeded;
        public readonly Api.QueryConfiguration.Query Query;
        public readonly ReadOnlyCollection<DbRequestParameter> Parameters;
        public readonly string CommandText;

        public GetQueryConfigurationResult(bool succeeded, Api.QueryConfiguration.Query query, ReadOnlyCollection<DbRequestParameter> parameters,
            string commandText)
        {
            Succeeded = succeeded;
            Query = query;
            Parameters = parameters;
            CommandText = commandText;
        }
    }

    private static GetQueryConfigurationResult GetQueryConfiguration(string commandText)
    {
        Assert.IsNotNull(commandText);

        var succeeded = false;
        Api.QueryConfiguration.Query query = null;
        ReadOnlyCollection<DbRequestParameter> parameters = null;
        string resultCommandText = null;

        var configurationStart = commandText.IndexOf("/* Query Configuration");
        if (configurationStart >= 0)
        {
            var configurationEnd = commandText.IndexOf("*/", configurationStart);
            if (configurationEnd > 0)
            {
                configurationStart = commandText.IndexOf("{", configurationStart);
                configurationEnd = commandText.LastIndexOf("}", configurationEnd);
                var configuration = commandText.Substring(configurationStart, configurationEnd - configurationStart + 1);
                query = JsonConvert.DeserializeObject<Api.QueryConfiguration.Query>(configuration);
                var commentEnd = commandText.IndexOf("*/", configurationEnd);
                if (commentEnd >= 0)
                {
                    var parametersStart = commentEnd + 4;
                    var parametersEnd = commandText.IndexOf("-- CommandText\r\n", parametersStart);
                    if (parametersStart < parametersEnd)
                    {
                        var parametersCommandText = commandText.Substring(parametersStart, parametersEnd - parametersStart + 1);
                        var tokens = SqlParser.Tokenize(parametersCommandText);
                        parameters = ToDbQueryParameters(tokens);
                    }
                    else
                        parameters = EmptyReadOnlyCollection<DbRequestParameter>.Value;

                    resultCommandText = commandText.Substring(parametersEnd + 16);
                    succeeded = true;
                }
            }
        }

        return new GetQueryConfigurationResult(succeeded, query, parameters, resultCommandText);
    }

    private static DbRequestParameter ToDbRequestParameter(List<Token> declaration)
    {
        var name = declaration[1].Value;
        name = name.Substring(1);
        var dataType = declaration[2].Value;
        var dataTypeLower = dataType.ToLower();
        SqlDbType sqlDbType;
        var size = 0;
        bool isNullable;
        string csharpValue = null;

        var sqlDataType = SqlDataTypeArray.SqlDataTypes.FirstOrDefault(i => i.SqlDataTypeName == dataTypeLower);
        if (sqlDataType != null)
        {
            sqlDbType = sqlDataType.SqlDbType;
            if (declaration.Count > 5 && declaration[3].Value == "(" && declaration[5].Value == ")")
            {
                var sizeString = declaration[4].Value;
                if (sizeString.ToLower() == "max")
                    size = -1;
                else
                    size = int.Parse(sizeString);
            }

            isNullable = true;

            for (var i = 3; i < declaration.Count; ++i)
            {
                if (declaration[i].Value == "/" && declaration[i + 1].Value == "*" &&
                    declaration[i + 2].Value.ToLower() == "not" && declaration[i + 3].Value.ToLower() == "null" &&
                    declaration[i + 4].Value == "*" && declaration[i + 5].Value == "/")
                {
                    isNullable = false;
                    break;
                }
            }
        }
        else
        {
            sqlDbType = SqlDbType.Structured;
            isNullable = false;
            csharpValue = $"query.{name}.Select(i => i.ToSqlDataRecord()).ToReadOnlyCollection()";
        }

        return new DbRequestParameter(name, dataType, sqlDbType, size, isNullable, csharpValue);
    }

    private static ReadOnlyCollection<DbRequestParameter> ToDbQueryParameters(List<Token> tokens)
    {
        var declarations = GetDeclarations(tokens);
        return declarations.Select(ToDbRequestParameter).ToReadOnlyCollection();
    }

    private static List<List<Token>> GetDeclarations(List<Token> tokens)
    {
        var declarations = new List<List<Token>>();
        List<Token> declaration = null;
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.KeyWord && token.Value.ToLower() == "declare")
            {
                if (declaration != null)
                    declarations.Add(declaration);

                declaration = new List<Token>();
            }

            declaration.Add(token);
        }

        if (declaration != null)
            declarations.Add(declaration);
        return declarations;
    }

    private void ShowDataTableText(DataTable dataTable)
    {
        var textBox = new RichTextBox();
        GarbageMonitor.Default.Add("ShowDataTableText.textBox", textBox);
        textBox.MaxLength = int.MaxValue;
        textBox.Multiline = true;
        textBox.WordWrap = false;
        textBox.Font = _font;
        textBox.ScrollBars = RichTextBoxScrollBars.Both;

        ShowTabPage("TextResult", GetToolTipText(null), textBox);

        TextWriter textWriter = new TextBoxWriter(textBox);
        var resultWriter = (IResultWriter)new TextResultWriter(AddInfoMessage, textWriter, this);

        resultWriter.Begin(Provider);

        var schemaTable = new DataTable();

        schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
        schemaTable.Columns.Add("ColumnSize", typeof(int));
        schemaTable.Columns.Add("DataType", typeof(Type));
        schemaTable.Columns.Add("NumericPrecision", typeof(short));
        schemaTable.Columns.Add("NumericScale", typeof(short));

        foreach (DataColumn column in dataTable.Columns)
        {
            var type = column.DataType;
            var typeCode = Type.GetTypeCode(type);
            int columnSize;
            const short numericPrecision = 0;
            const short numericScale = 0;

            switch (typeCode)
            {
                case TypeCode.String:
                    var maxLength = 0;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        var length = dataRow[column].ToString().Length;

                        if (length > maxLength)
                        {
                            maxLength = length;
                        }
                    }

                    columnSize = maxLength;
                    break;

                default:
                    columnSize = 0;
                    break;
            }

            schemaTable.Rows.Add(new object[]
            {
                column.ColumnName,
                columnSize,
                column.DataType,
                numericPrecision,
                numericScale
            });
        }

        resultWriter.WriteTableBegin(schemaTable);

        var colCount = dataTable.Columns.Count;

        for (var i = 0; i < dataTable.Rows.Count; i++)
        {
            var dataRow = dataTable.Rows[i];
            var values = new object[colCount];

            for (var j = 0; j < colCount; j++)
            {
                values[j] = dataRow[j];
            }

            resultWriter.WriteRows(new[] { values }, 1);
        }

        resultWriter.WriteTableEnd();

        resultWriter.End();
    }

    private void ShowDataTableDataGrid(DataTable dataTable)
    {
        var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
        var dataTableEditor = new DataTableEditor(commandBuilder, _colorTheme);
        dataTableEditor.StatusBarPanel = _sbPanelText;
        dataTableEditor.ReadOnly = !_openTableMode;

        if (_openTableMode)
        {
            var tableName = _sqlStatement.FindTableName();
            dataTableEditor.TableName = tableName;
            var getTableSchemaResult = Provider.GetTableSchema(Connection.Connection, tableName);
            dataTableEditor.TableSchema = getTableSchemaResult;
        }

        GarbageMonitor.Default.Add("dataTableEditor", dataTableEditor);
        dataTableEditor.StatusBarPanel = _sbPanelText;
        dataTableEditor.DataTable = dataTable;
        ShowTabPage(dataTable.TableName, GetToolTipText(dataTable), dataTableEditor);
    }

    //private void ShowDataViewHtml(DataView dataView)
    //{
    //    var fileName = Path.GetTempFileName();
    //    using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
    //    {
    //        using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
    //        {
    //            var columnIndexes = new int[dataView.Table.Columns.Count];
    //            for (var i = 0; i < columnIndexes.Length; i++)
    //            {
    //                columnIndexes[i] = i;
    //            }

    //            HtmlFormatter.Write(dataView, columnIndexes, streamWriter);
    //        }
    //    }

    //    var dataTable = dataView.Table;
    //    var tabPage = new TabPage(dataTable.TableName);
    //    tabPage.ToolTipText = GetToolTipText(dataTable);
    //    _tabControl.TabPages.Add(tabPage);

    //    var htmlTextBox = new HtmlTextBox();
    //    htmlTextBox.Dock = DockStyle.Fill;
    //    tabPage.Controls.Add(htmlTextBox);

    //    _tabControl.SelectedTab = tabPage;

    //    htmlTextBox.Navigate(fileName);

    //    _sbPanelRows.Text = dataTable.Rows.Count + " row(s).";
    //}

    private void ShowDataTableRtf(DataTable dataTable)
    {
        try
        {
            SetStatusbarPanelText("Creating Word document...", SystemColors.ControlText);

            var fileName = WordDocumentCreator.CreateWordDocument(dataTable);

            var richTextBox = new RichTextBox();
            GarbageMonitor.Default.Add("ShowDataTableRtf.richTextBox", richTextBox);
            richTextBox.WordWrap = false;
            richTextBox.LoadFile(fileName);
            File.Delete(fileName);

            SetStatusbarPanelText("Word document created.", SystemColors.ControlText);

            ShowTabPage(dataTable.TableName, GetToolTipText(dataTable), richTextBox);
        }
        catch (Exception e)
        {
            ShowMessage(e);
        }
    }

    private void ShowDataTableExcel(DataTable dataTable)
    {
    }

    private void ShowDataTableListView(DataTable dataTable)
    {
        var listView = new ListView
        {
            View = View.Details,
            GridLines = true,
            AllowColumnReorder = true,
            Font = new Font("Tahoma", 8),
            FullRowSelect = true
        };

        foreach (DataColumn dataColumn in dataTable.Columns)
        {
            var columnHeader = new ColumnHeader
            {
                Text = dataColumn.ColumnName,
                Width = -2
            };

            var type = (Type)dataColumn.ExtendedProperties[0];

            if (type == null)
                type = dataColumn.DataType;

            columnHeader.TextAlign = QueryFormStaticMethods.GetHorizontalAlignment(type);

            listView.Columns.Add(columnHeader);
        }

        var count = dataTable.Columns.Count;
        var items = new string[count];

        foreach (DataRow dataRow in dataTable.Rows)
        {
            for (var i = 0; i < count; ++i)
            {
                var value = dataRow[i];
                items[i] = value == DBNull.Value ? "(null)" : value.ToString();
            }

            var listViewItem = new ListViewItem(items);
            listView.Items.Add(listViewItem);
        }

        ShowTabPage(dataTable.TableName, null, listView);
    }

    private void ShowTabPage(string tabPageName, string toolTipText, Control control)
    {
        control.Dock = DockStyle.Fill;
        var tabPage = new TabPage(tabPageName);
        tabPage.ToolTipText = toolTipText;
        _tabControl.TabPages.Add(tabPage);
        tabPage.Controls.Add(control);
        _tabControl.SelectedTab = tabPage;
        // tabPage.Refresh();
    }

    public void ShowMessage(Exception exception)
    {
        var infoMessages = Provider.ToInfoMessages(exception);
        AddInfoMessages(infoMessages);

        _tabControl.SelectedTab = _messagesTabPage;

        SetStatusbarPanelText("Query batch completed with errors.", Color.Red);
        _sbPanelRows.Text = null;
    }

    private void Connection_DatabaseChanged(object sender, DatabaseChangedEventArgs args)
    {
        if (InvokeRequired)
        {
            this.Invoke(() => Connection_DatabaseChanged(sender, args));
        }
        else
        {
            if (_database != args.Database)
            {
                var message = $"[DatabaseChanged] Database changed from {_database} to {_database}";
                var infoMessage = InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message);
                AddInfoMessage(infoMessage);

                _database = args.Database;
                SetText();
            }
        }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                //var now = LocalTime2.Default.Now;
                //foreach (IComponent component in components.Components)
                //    GarbageMonitor.Default.SetDisposeTime(component, now);

                components.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private void FocusControl(Control control) => control.Focus();

    private void ShowResultWriterDataSet()
    {
        if (_dataSetResultWriter != null)
        {
            var dataSet = _dataSetResultWriter.DataSet;
            ShowDataSet(dataSet);
        }
    }

    private void EndFill(IAsyncDataAdapter dataAdapter, Exception e)
    {
        try
        {
            if (e != null)
                ShowMessage(e);

            if (Connection.State == ConnectionState.Open && Connection.Database != _database)
            {
                _database = Connection.Database;
                SetText();
            }

            switch (TableStyle)
            {
                case ResultWriterType.DataGrid:
                case ResultWriterType.Html:
                case ResultWriterType.Rtf:
                case ResultWriterType.ListView:
                    Invoke(new MethodInvoker(ShowResultWriterDataSet));
                    break;

                case ResultWriterType.DataGridView:
                    var dataGridViewResultWriter = (DataGridViewResultWriter)dataAdapter.ResultWriter;
                    const string text = "TODO";
                    var resultSetTabPage = new TabPage(text);
                    resultSetTabPage.ToolTipText = null; // TODO
                    _resultSetsTabControl.TabPages.Add(resultSetTabPage);
                    _resultSetsTabControl.SelectedTab = resultSetTabPage;
                    var tabControl = new TabControl();
                    tabControl.Dock = DockStyle.Fill;
                    var index = 0;
                    foreach (var dataGridView in dataGridViewResultWriter.DataGridViews)
                    {
                        dataGridView.Dock = DockStyle.Fill;
                        //text = dataTable.TableName;
                        var tabPage = new TabPage(text);
                        tabPage.ToolTipText = null; // TODO
                        tabPage.Controls.Add(dataGridView);
                        tabControl.TabPages.Add(tabPage);
                        index++;
                    }

                    resultSetTabPage.Controls.Add(tabControl);
                    break;
            }

            if (e != null)
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Connection is closed. Opening connection..."));

                    var connectionStringBuilder = Provider.CreateConnectionStringBuilder();
                    connectionStringBuilder.ConnectionString = _connectionString;
                    connectionStringBuilder.SetValue(ConnectionStringKeyword.InitialCatalog, _database);

                    var connectionProperties = new ConnectionProperties(null, Provider.Name, Provider);
                    connectionProperties.ConnectionString = connectionStringBuilder.ConnectionString;
                    
                    var openConnectionForm = new OpenConnectionForm(connectionProperties);
                    if (openConnectionForm.ShowDialog() == DialogResult.OK)
                    {
                        Connection.Connection.Dispose();
                        Connection = connectionProperties.Connection;
                        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Opening connection succeeded."));
                    }
                    else
                        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Opening connection canceled."));
                }
            }

            if (e != null || dataAdapter.TableCount == 0)
                _tabControl.SelectedTab = _messagesTabPage;
            else
            {
                switch (TableStyle)
                {
                    case ResultWriterType.DataGrid:
                    case ResultWriterType.DataGridView:
                    case ResultWriterType.Html:
                    case ResultWriterType.ListView:
                    case ResultWriterType.Rtf:
                    case ResultWriterType.SqLite:
                    case ResultWriterType.Text:
                        _tabControl.SelectedTab = _resultSetsTabPage;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            this.Invoke(() => EndFillHandleException(ex));
        }
    }

    private void WriteEnd(IAsyncDataAdapter dataAdapter)
    {
        _timer.Stop();

        if (dataAdapter != null)
            WriteRows(dataAdapter.RowCount, 3);

        _stopwatch.Reset();

        if (_cancel)
        {
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Query was cancelled by user."));
            SetStatusbarPanelText("Query was cancelled by user.", _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
            _cancel = false;
        }
        else
        {
            if (_errorCount == 0)
                SetStatusbarPanelText("Query executed successfully.", _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
            else
                SetStatusbarPanelText("Query completed with errors.", _colorTheme != null ? _colorTheme.ProviderKeyWordColor : Color.Red);
        }

        _dataAdapter = null;
        _dataSetResultWriter = null;

        SetGui(CommandState.Execute);
        FocusControl(QueryTextBox);
        Cursor = Cursors.Default;

        this.Invoke(() => _mainForm.UpdateTotalMemory());
    }

    private void EndFillInvoker(IAsyncDataAdapter dataAdapter, Exception e) => this.Invoke(() => EndFill(dataAdapter, e));

    private void WriteEndInvoker(IAsyncDataAdapter dataAdapter) => this.Invoke(() => WriteEnd(dataAdapter));

    private void EndFillHandleException(Exception ex)
    {
        QueryTextBox.Focus();
        Cursor = Cursors.Default;
        MessageBox.Show(ex.ToString());
    }

    private void SetGui(CommandState buttonState)
    {
        var ok = (buttonState & CommandState.Execute) != 0;
        var cancel = (buttonState & CommandState.Cancel) != 0;

        _mnuCancel.Enabled = cancel;

        ButtonState = buttonState;

        _executeQueryToolStripMenuItem.Enabled = ok;
        _executeQueryMenuItem.Enabled = ok;
        _mnuExecuteQuerySingleRow.Enabled = ok;
        _mnuExecuteQuerySchemaOnly.Enabled = ok;
        _mnuExecuteQueryKeyInfo.Enabled = ok;
        _mnuExecuteQueryXml.Enabled = ok;

        Log.Trace("this.executeQuerySplitButton.Enabled = {0};", ok);
        _executeQuerySplitButton.Enabled = ok;
        _cancelQueryButton.Enabled = cancel;
    }

    private void EditRows_Click(object sender, EventArgs e) => EditRows(Query);

    private static void WriteInfoMessageToLog(InfoMessage infoMessage)
    {
        LogLevel logLevel;
        switch (infoMessage.Severity)
        {
            case InfoMessageSeverity.Error:
                logLevel = LogLevel.Error;
                break;

            case InfoMessageSeverity.Information:
                logLevel = LogLevel.Information;
                break;

            case InfoMessageSeverity.Verbose:
                logLevel = LogLevel.Trace;
                break;

            default:
                throw new Exception();
        }

        Log.Write(logLevel, infoMessage.Message);
    }

    private void ConsumeInfoMessages()
    {
        var waitHandles = new[]
        {
            _enqueueEvent,
            _cancellationTokenSource.Token.WaitHandle
        };

        while (true)
        {
            var hasElements = false;
            while (_infoMessages.Count > 0 && IsHandleCreated)
            {
                hasElements = true;
                var infoMessages = new InfoMessage[_infoMessages.Count];
                var count = _infoMessages.Take(infoMessages);
                for (var i = 0; i < count; i++)
                {
                    this.Invoke(() =>
                    {
                        var message = infoMessages[i];
                        var color = _messagesTextBox.SelectionColor;

                        switch (message.Severity)
                        {
                            case InfoMessageSeverity.Error:
                                _messagesTextBox.SelectionColor = _colorTheme != null ? _colorTheme.ProviderKeyWordColor : Color.Red;
                                break;

                            case InfoMessageSeverity.Information:
                                _messagesTextBox.SelectionColor = _colorTheme != null ? _colorTheme.SqlKeyWordColor : Color.Blue;
                                break;
                        }

                        AppendMessageText(message.CreationTime, message.Severity, message.Header, message.Message);

                        switch (message.Severity)
                        {
                            case InfoMessageSeverity.Error:
                            case InfoMessageSeverity.Information:
                                _messagesTextBox.SelectionColor = color;
                                break;
                        }
                    });
                }
            }

            if (hasElements)
                this.Invoke(() =>
                {
                    _messagesTextBox.ScrollToCaret();
                    _messagesTextBox.Update();
                });

            if (_infoMessages.Count == 0)
            {
                var w = WaitHandle.WaitAny(waitHandles, 1000);
                if (w == 1)
                    break;
            }
        }
    }

    private void textBox_SelectionChanged(object sender, EventArgs e)
    {
        var richTextBox = (RichTextBox)sender;
        var charIndex = richTextBox.SelectionStart;
        var line = richTextBox.GetLineFromCharIndex(charIndex) + 1;
        var lineIndex = QueryTextBox.GetLineIndex(richTextBox, -1);
        var col = charIndex - lineIndex + 1;
        _sbPanelCaretPosition.Text = "Ln " + line + " Col " + col;
    }

    private void AddTable(OleDbConnection oleDbConnection, DataSet dataSet, Guid guid, string name)
    {
        try
        {
            var dataTable = oleDbConnection.GetOleDbSchemaTable(guid, null);
            dataTable.TableName = name;
            dataSet.Tables.Add(dataTable);
        }
        catch (Exception e)
        {
            _messagesTextBox.Text += e.ToString();
        }
    }

    private void mnuDescribeParameters_Click(object sender, EventArgs e)
    {
        try
        {
            Cursor = Cursors.WaitCursor;

            var oldDbConnection = Connection.Connection as OleDbConnection;

            if (oldDbConnection != null && string.IsNullOrEmpty(Query))
            {
                var dataSet = new DataSet();
                AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Provider_Types, "Provider Types");
                AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.DbInfoLiterals, "DbInfoLiterals");

                var c2 = new ConnectionClass();
                c2.Open(_connectionString, null, null, 0);
                var rs = c2.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                var dataTable = OleDbHelper.Convert(rs);
                c2.Close();
                dataSet.Tables.Add(dataTable);

                AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Sql_Languages, "Sql Languages");
                ShowDataSet(dataSet);
            }
            else
            {
                _sqlStatement = new SqlParser(Query);
                _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

                if (_command != null)
                {
                    AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, _command.ToLogString()));
                    var dataTable = Provider.GetParameterTable(_command.Parameters);

                    if (dataTable != null)
                    {
                        dataTable.TableName = "Parameters";

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var value = row["Value"];
                            var type = value.GetType();
                            var typeCode = Type.GetTypeCode(type);

                            switch (typeCode)
                            {
                                case TypeCode.DateTime:
                                    var dateTime = (DateTime)value;
                                    var ticks = dateTime.Ticks;

                                    if (ticks % StopwatchConstants.TicksPerDay == 0)
                                        row["Value"] = dateTime.ToString("yyyy-MM-dd");
                                    else
                                        row["Value"] = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    break;
                            }
                        }

                        var dataSet = new DataSet();
                        dataSet.Tables.Add(dataTable);
                        ShowDataSet(dataSet);
                    }
                }
            }

            Cursor = Cursors.Default;
        }
        catch (Exception ex)
        {
            Cursor = Cursors.Default;
            ShowMessage(ex);
        }
    }

    private void mnuCloseTabPage_Click(object sender, EventArgs e)
    {
        var tabPage = _tabControl.SelectedTab;
            
        if (tabPage != null && tabPage != _messagesTabPage && tabPage != _resultSetsTabPage)
            CloseResultSetTabPage(tabPage);
    }

    private void CloseResultSetTabPages()
    {
        var tabPages = _resultSetsTabControl.TabPages.Cast<TabPage>().ToArray();
            
        foreach (var tabPage in tabPages)
            CloseResultSetTabPage(tabPage);

        ResultSetCount = 0;
    }

    private void mnuCloseAllTabPages_Click(object sender, EventArgs e)
    {
        CloseResultSetTabPages();

        _tabControl.SelectedTab = _messagesTabPage;
        _messagesTextBox.Clear();
        SetStatusbarPanelText(null, SystemColors.ControlText);

        if (_dataAdapter == null)
        {
            _sbPanelRows.Text = null;
            _sbPanelTimer.Text = null;
        }

        this.Invoke(() => FocusControl(QueryTextBox));
    }

    public void CancelCommandQuery()
    {
        Log.Trace(ThreadMonitor.ToStringTableString());
        const string message = "Cancelling command...";
        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, message));
        SetStatusbarPanelText("Cancel Executing Command/Query...", _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
        _cancel = true;
        SetGui(CommandState.None);
        _dataAdapter.Cancel();
    }

    private void mnuCancel_Click(object sender, EventArgs e) => CancelCommandQuery();

    private void WriteRows(long rowCount, int scale)
    {
        var ticks = _stopwatch.ElapsedTicks;
        _sbPanelTimer.Text = StopwatchTimeSpan.ToString(ticks, scale);
        var text = rowCount + " rows.";
        if (rowCount > 0)
        {
            var seconds = (double)ticks / Stopwatch.Frequency;
            text += " (" + Math.Round(rowCount / seconds, 0) + " rows/sec)";
        }

        _sbPanelRows.Text = text;
    }

    private void ShowTimer()
    {
        if (_dataAdapter != null)
        {
            var rowCount = _dataAdapter.RowCount;
            WriteRows(rowCount, 0);
        }
    }

    private void Timer_Tick(object o, EventArgs e) => this.Invoke(ShowTimer);

    protected override void OnClosing(CancelEventArgs e)
    {
        var text = QueryTextBox.RichTextBox.Text;
        if (Connection != null)
            Log.Write(LogLevel.Trace, "Saving text before closing form(connectionName: {0}):\r\n{1}", Connection.ConnectionName, text);

        if (_dataAdapter == null)
        {
            bool hasTransactions;
            if (_transaction != null)
                hasTransactions = true;
            else if (Connection != null && Connection.State == ConnectionState.Open)
            {
                try
                {
                    hasTransactions = Connection.TransactionCount > 0;
                }
                catch (Exception ex)
                {
                    var message = Provider.GetExceptionMessage(ex);
                    var color = _messagesTextBox.SelectionColor;
                    _messagesTextBox.SelectionColor = Color.Red;
                    AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, message));
                    _messagesTextBox.SelectionColor = color;
                    hasTransactions = false;
                }
            }
            else
                hasTransactions = false;

            if (hasTransactions)
            {
                text = "There are uncommitted transactions. Do you wish to commit these transactions before closing the window?";
                var caption = DataCommanderApplication.Instance.Name;
                var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        // TODO
                        // this.connection.COmmit();
                        e.Cancel = true;
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            if (!e.Cancel)
            {
                var length = QueryTextBox.Text.Length;

                if (length > 0)
                {
                    text = $"The text in {Text} has been changed.\r\nDo you want to save the changes?";
                    var caption = DataCommanderApplication.Instance.Name;
                    var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (_fileName != null)
                                Save(_fileName);
                            else
                                ShowSaveFileDialog();

                            break;

                        case DialogResult.No:
                            break;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }
        else
        {
            text = "Are you sure you wish to cancel this query?";
            var caption = DataCommanderApplication.Instance.Name;
            var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                CancelCommandQuery();
                _timer.Enabled = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        if (!e.Cancel)
        {
            _cancellationTokenSource.Cancel();

            if (Connection != null)
            {
                var dataSource = Connection.DataSource;
                _parentStatusBar.Items[0].Text = $"Closing connection to data source {dataSource}'....";
                Connection.Close();
                _parentStatusBar.Items[0].Text = $"Connection to data source {dataSource} closed.";
                Connection.Connection.Dispose();
                Connection = null;
            }

            if (_toolStrip != null)
            {
                _toolStrip.Dispose();
                _toolStrip = null;
            }
        }
    }

    private void SetResultWriterType(ResultWriterType tableStyle)
    {
        TableStyle = tableStyle;
        _sbPanelTableStyle.Text = tableStyle.ToString();
    }

    private void mnuText_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Text);
    private void mnuDataGrid_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.DataGrid);
    private void mnuHtml_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Html);
    private void mnuRtf_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Rtf);
    private void mnuListView_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.ListView);
    private void mnuExcel_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Excel);
    private void menuResultModeFile_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.File);

    private void mnuCommandTypeText_Click(object sender, EventArgs e)
    {
        _mnuCommandTypeText.Checked = true;
        _mnuCommandTypeStoredProcedure.Checked = false;
        _commandType = CommandType.Text;
    }

    private void mnuCommandTypeStoredProcedure_Click(object sender, EventArgs e)
    {
        _mnuCommandTypeText.Checked = false;
        _mnuCommandTypeStoredProcedure.Checked = true;
        _commandType = CommandType.StoredProcedure;
    }

    private void menuObjectExplorer_Click(object sender, EventArgs e)
    {
        var visible = !_tvObjectExplorer.Visible;
        _tvObjectExplorer.Visible = visible;
        _splitterObjectExplorer.Visible = visible;
    }

    private void tvObjectBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        var treeNode = e.Node;

        if (treeNode.Nodes.Count > 0)
        {
            var treeNode2 = (ITreeNode)treeNode.Nodes[0].Tag;

            if (treeNode2 == null)
            {
                Cursor = Cursors.WaitCursor;

                try
                {
                    treeNode.Nodes.Clear();
                    treeNode2 = (ITreeNode)treeNode.Tag;
                    IEnumerable<ITreeNode> children = null;

                    try
                    {
                        children = treeNode2.GetChildren(false);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex);
                    }

                    if (children != null)
                        AddNodes(treeNode.Nodes, children, treeNode2.Sortable);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                var count = treeNode.GetNodeCount(false);
                SetStatusbarPanelText(treeNode.Text + " node has " + count + " children.", SystemColors.ControlText);
            }
        }
    }

    private void tvObjectBrowser_MouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Right:
                var treeNode = _tvObjectExplorer.GetNodeAt(e.X, e.Y);
                if (treeNode != null)
                {
                    var treeNode2 = (ITreeNode)treeNode.Tag;

                    if (e.Button != MouseButtons.Left)
                        _tvObjectExplorer.SelectedNode = treeNode;

                    var text = treeNode.Text;
                }

                break;

            default:
                break;
        }
    }

    private void mnuRefresh_Click(object sender, EventArgs e)
    {
        var treeNodeV = _tvObjectExplorer.SelectedNode;
        if (treeNodeV != null)
        {
            var treeNode = (ITreeNode)treeNodeV.Tag;
            treeNodeV.Nodes.Clear();
            AddNodes(treeNodeV.Nodes, treeNode.GetChildren(true), treeNode.Sortable);
        }
    }

    private void mnuRefreshObjectExplorer_Click(object sender, EventArgs e)
    {
        var objectExplorer = Provider.CreateObjectExplorer();
        if (objectExplorer != null)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var rootNodes = _tvObjectExplorer.Nodes;
                rootNodes.Clear();
                var treeNodes = objectExplorer.GetChildren(true);
                AddNodes(rootNodes, treeNodes, objectExplorer.Sortable);
            }
        }
    }

    private void tvObjectExplorer_MouseUp(object sender, MouseEventArgs e)
    {
        try
        {
            if (e.Button == MouseButtons.Right)
            {
                var treeNodeV = _tvObjectExplorer.SelectedNode;
                if (treeNodeV != null)
                {
                    var treeNode = (ITreeNode)treeNodeV.Tag;
                    var contextMenu = GetContextMenu(treeNode);

                    if (!treeNode.IsLeaf)
                    {
                        if (contextMenu == null)
                            contextMenu = new ContextMenuStrip(components);

                        contextMenu.Items.Add(new ToolStripMenuItem("Refresh", null, mnuRefresh_Click));
                    }

                    if (contextMenu != null)
                    {
                        if (_colorTheme != null)
                        {
                            contextMenu.ForeColor = _colorTheme.ForeColor;
                            contextMenu.BackColor = _colorTheme.BackColor;
                        }

                        var contains = components.Components.Cast<IComponent>().Contains(contextMenu);
                        if (!contains)
                        {
                            components.Add(contextMenu);
                            GarbageMonitor.Default.Add("contextMenu", contextMenu);
                        }

                        var pos = new Point(e.X, e.Y);
                        contextMenu.Show(_tvObjectExplorer, pos);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private ContextMenuStrip? GetContextMenu(ITreeNode treeNode)
    {
        
        var contextMenu = treeNode.GetContextMenu();
        var contextMenuStrip = contextMenu != null
            ? ToContextMenuStrip(contextMenu)
            : null;
        return contextMenuStrip;
    }

    private ContextMenuStrip ToContextMenuStrip(ContextMenu contextMenu)
    {
        ToolStripItem[] menuItems = contextMenu.MenuItems
            .Select(ToToolStripMenuItem)
            .ToArray();

        var contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.AddRange(menuItems);

        return contextMenuStrip;
    }

    private ToolStripMenuItem ToToolStripMenuItem(MenuItem source)
    {
        var item = new ToolStripMenuItem(source.Text, null, (sender, args) => source.OnClick(this, args));
        var dropdownItems = source.DropDownItems
            .Select(ToToolStripMenuItem)
            .Cast<ToolStripItem>()
            .ToArray();
        item.DropDownItems.AddRange(dropdownItems);
        return item;
    }

    private void tvObjectBrowser_DoubleClick(object sender, EventArgs e)
    {
        var selectedNode = _tvObjectExplorer.SelectedNode;
        if (selectedNode != null)
        {
            var treeNode = (ITreeNode)selectedNode.Tag;

            try
            {
                Cursor = Cursors.WaitCursor;
                var query = treeNode.Query;
                if (query != null)
                {
                    var text0 = QueryTextBox.Text;
                    string append = null;
                    var selectionStart = QueryTextBox.RichTextBox.TextLength;

                    if (!string.IsNullOrEmpty(text0))
                    {
                        append = Environment.NewLine + Environment.NewLine;
                        selectionStart += 2;
                    }

                    append += query;

                    QueryTextBox.RichTextBox.AppendText(append);
                    QueryTextBox.RichTextBox.SelectionStart = selectionStart;
                    QueryTextBox.RichTextBox.SelectionLength = query.Length;

                    QueryTextBox.Focus();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }

    private void mnuPaste_Click(object sender, EventArgs e) => QueryTextBox.Paste();

    private void mnuGoTo_Click(object sender, EventArgs e)
    {
        var control = ActiveControl;
        var richTextBox = control as RichTextBox;
        if (richTextBox == null)
            richTextBox = QueryTextBox.RichTextBox;

        var charIndex = richTextBox.SelectionStart;
        var currentLineNumber = richTextBox.GetLineFromCharIndex(charIndex) + 1;
        var form = new GotoLineForm();
        var maxLineNumber = richTextBox.Lines.Length;
        form.Init(currentLineNumber, maxLineNumber);

        if (form.ShowDialog(this) == DialogResult.OK)
        {
            var lineNumber = form.LineNumber;
            charIndex = NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int)NativeMethods.Message.EditBox.LineIndex, lineNumber - 1, 0);
            richTextBox.SelectionStart = charIndex;
        }
    }

    private TreeNode FindTreeNode(TreeNode parent, IStringMatcher matcher)
    {
        TreeNode found = null;

        if (matcher.IsMatch(parent.Text))
            found = parent;
        else
        {
            foreach (TreeNode child in parent.Nodes)
            {
                found = FindTreeNode(child, matcher);

                if (found != null)
                    break;
            }
        }

        return found;
    }

    private void FindText(string text)
    {
        var found = false;
        var control = ActiveControl;

        try
        {
            Cursor = Cursors.WaitCursor;
            SetStatusbarPanelText($"Finding {text}...", SystemColors.ControlText);
            StringComparison comparison;
            var options = _findTextForm.RichTextBoxFinds;
            switch (options)
            {
                case RichTextBoxFinds.None:
                    comparison = StringComparison.InvariantCultureIgnoreCase;
                    break;

                case RichTextBoxFinds.MatchCase:
                    comparison = StringComparison.InvariantCulture;
                    break;

                case RichTextBoxFinds.WholeWord:
                    // TODO
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }

            IStringMatcher matcher = new StringMatcher(text, comparison);
            var treeView = control as TreeView;

            if (treeView != null)
            {
                var treeNode2 = treeView.SelectedNode.FirstNode;

                if (treeNode2 == null || treeNode2.Tag == null)
                    treeNode2 = treeView.SelectedNode.NextNode;

                var treeNode = treeNode2;

                while (treeNode != null)
                {
                    treeNode2 = FindTreeNode(treeNode, matcher);

                    if (treeNode2 != null)
                    {
                        treeView.SelectedNode = treeNode2;
                        found = true;
                        break;
                    }
                    else
                        treeNode = treeNode.NextNode;
                }
            }
            else
            {
                var dataTableEditor = control as DataTableEditor;

                if (dataTableEditor == null)
                    dataTableEditor = control.Parent as DataTableEditor;

                if (dataTableEditor != null)
                {
                    var dataTable = dataTableEditor.DataTable;

                    if (dataTable != null)
                    {
                        if (text.StartsWith("RowFilter="))
                        {
                            var rowFilter = text.Substring(5);
                            var dataView = dataTable.DefaultView;
                            dataView.RowFilter = rowFilter;
                            var count = dataView.Count;
                            found = count > 0;
                            SetStatusbarPanelText($"{count} rows found. RowFilter: {rowFilter}", SystemColors.ControlText);
                        }
                        else if (text.StartsWith("Sort="))
                        {
                            var sort = text.Substring(5);
                            var dataView = dataTable.DefaultView;
                            dataView.Sort = sort;
                            SetStatusbarPanelText($"Rows sorted by {sort}.", SystemColors.ControlText);
                        }
                        else
                        {
                            var dataGrid = dataTableEditor.DataGrid;
                            var cell = dataGrid.CurrentCell;
                            var rowIndex = cell.RowIndex;
                            var columnIndex = cell.ColumnIndex;
                            found = QueryFormStaticMethods.FindText(dataTable.DefaultView, matcher, ref rowIndex, ref columnIndex);

                            if (found)
                            {
                                dataGrid.CurrentCell = dataGrid[columnIndex, rowIndex];
                            }
                        }
                    }
                    else
                        found = false;
                }
                else
                {
                    var richTextBox = control as RichTextBox;

                    if (richTextBox == null)
                    {
                        richTextBox = QueryTextBox.RichTextBox;
                    }

                    var start = richTextBox.SelectionStart + richTextBox.SelectionLength;
                    var location = richTextBox.Find(text, start, options);
                    found = location >= 0;
                }
            }
        }
        finally
        {
            SetStatusbarPanelText(null, SystemColors.ControlText);
            Cursor = Cursors.Default;
        }

        if (!found)
        {
            var message = $"The specified text was not found.\r\n\r\nText: {text}\r\nControl: {control.Name}";
            MessageBox.Show(this, message, DataCommanderApplication.Instance.Name);
        }
    }

    private void mnuFind_Click(object sender, EventArgs e)
    {
        try
        {
            if (_findTextForm == null)
                _findTextForm = new FindTextForm();

            var control = ActiveControl;
            var dataTableViewer = control as DataTableEditor;

            if (dataTableViewer == null)
            {
                control = control.Parent;
                dataTableViewer = control as DataTableEditor;
            }

            if (dataTableViewer != null)
            {
                var dataTable = dataTableViewer.DataTable;
                var name = dataTable.TableName;
                _findTextForm.Text = $"Find (DataTable: {name})";
            }
            else
                _findTextForm.Text = "Find";

            if (_findTextForm.ShowDialog() == DialogResult.OK)
                FindText(_findTextForm.FindText);
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuFindNext_Click(object sender, EventArgs e)
    {
        if (_findTextForm != null)
        {
            var text = _findTextForm.FindText;
            if (text != null)
                FindText(text);
        }
    }

    private void Save(string fileName)
    {
        Cursor = Cursors.WaitCursor;

        try
        {
            SetStatusbarPanelText($"Saving file {fileName}...", SystemColors.ControlText);
            const RichTextBoxStreamType type = RichTextBoxStreamType.UnicodePlainText;
            var encoding = Encoding.Unicode;

            using (var stream = File.Create(fileName))
            {
                var preamble = encoding.GetPreamble();
                stream.Write(preamble, 0, preamble.Length);
                QueryTextBox.RichTextBox.SaveFile(stream, type);
            }

            _fileName = fileName;
            SetText();
            SetStatusbarPanelText($"File {fileName} saved successfully.", SystemColors.ControlText);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void ShowSaveFileDialog()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "Save Query",
            Filter = "Query Files (*.sql)|*.sql",
            AddExtension = true,
            OverwritePrompt = true,
            DefaultExt = "sql"
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            var fileName = saveFileDialog.FileName;
            Save(fileName);
        }
    }

    public void Save()
    {
        if (_fileName != null)
            Save(_fileName);
        else
            ShowSaveFileDialog();
    }

    private void mnuSave_Click(object sender, EventArgs e)
    {
        Save();
    }

    private void mnuSaveAs_Click(object sender, EventArgs e)
    {
        ShowSaveFileDialog();
    }

    private void mnuGotoQueryEditor_Click(object sender, EventArgs e)
    {
        QueryTextBox.Select();
    }

    private void mnuGotoMessageTabPage_Click(object sender, EventArgs e)
    {
        _tabControl.SelectedTab = _messagesTabPage;
    }

    private GetCompletionResponse GetCompletion()
    {
        var textBox = QueryTextBox.RichTextBox;
        var text = textBox.Text;
        var position = textBox.SelectionStart;

        var ticks = Stopwatch.GetTimestamp();
        var response = Provider.GetCompletion(Connection, _transaction, text, position);
        var from = response.FromCache ? "cache" : "data source";
        ticks = Stopwatch.GetTimestamp() - ticks;
        var length = response.Items != null ? response.Items.Count : 0;
        SetStatusbarPanelText($"GetCompletion returned {length} items from {@from} in {StopwatchTimeSpan.ToString(ticks, 3)} seconds.",
            _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
        return response;
    }

    private void mnuListMembers_Click(object sender, EventArgs e)
    {
        if (QueryTextBox.KeyboardHandler == null)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var response = GetCompletion();
                if (response.Items != null)
                {
                    var completionForm = new CompletionForm(this);
                    completionForm.Initialize(QueryTextBox, response, _colorTheme);
                    completionForm.ItemSelected += completionForm_ItemSelected;
                    completionForm.Show(this);
                    QueryTextBox.RichTextBox.Focus();
                }
            }
        }
    }

    private void completionForm_ItemSelected(object sender, ItemSelectedEventArgs e)
    {
        var textBox = QueryTextBox;

        var intPtr = textBox.RichTextBox.Handle;
        var hWnd = intPtr.ToInt32();
        NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

        var objectName = e.ObjectName.QuotedName;

        textBox.RichTextBox.SelectionStart = e.StartIndex;
        textBox.RichTextBox.SelectionLength = e.Length;
        textBox.RichTextBox.SelectedText = objectName;
        textBox.RichTextBox.SelectionStart = e.StartIndex + objectName.Length;

        NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
    }

    private void mnuClearCache_Click(object sender, EventArgs e)
    {
        Provider.ClearCompletionCache();
    }

    private async void ExecuteReader(CommandBehavior commandBehavior)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

            if (_command != null)
            {
                IDataReader dataReader = null;

                try
                {
                    while (true)
                    {
                        try
                        {
                            dataReader = _command.ExecuteReader(commandBehavior);
                            break;
                        }
                        catch
                        {
                            if (Connection.State != ConnectionState.Open)
                            {
                                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Opening connection..."));
                                await Connection.OpenAsync(CancellationToken.None);
                                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Connection opened successfully."));
                            }
                            else
                                throw;
                        }
                    }

                    DataSet dataSet = null;
                    var i = 1;

                    do
                    {
                        var dataTable = Provider.GetSchemaTable(dataReader);

                        if (dataTable != null)
                        {
                            if (dataSet == null)
                                dataSet = new DataSet();

                            dataTable.TableName = "SchemaTable" + i;
                            dataSet.Tables.Add(dataTable);
                            i++;
                        }
                    } while (dataReader.NextResult());

                    ShowDataSet(dataSet);
                    _tabControl.SelectedTab = _resultSetsTabPage;
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }
            }

            Cursor = Cursors.Default;
        }
        catch (Exception ex)
        {
            Cursor = Cursors.Default;
            ShowMessage(ex);
        }
    }

    private void mnuResultSchema_Click(object sender, EventArgs e)
    {
        ExecuteReader(CommandBehavior.SchemaOnly);
    }

    private void mnuKeyInfo_Click(object sender, EventArgs e)
    {
        ExecuteReader(CommandBehavior.KeyInfo);
    }

    private void ExecuteQuerySingleRow()
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
            var dataSet = new DataSet();
            using (var dataReader = _command.ExecuteReader())
            {
                var tableIndex = 0;

                while (true)
                {
                    var schemaTable = Provider.GetSchemaTable(dataReader);
                    var dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    var rowIndex = 0;

                    while (dataReader.Read())
                    {
                        var values = new object[dataReader.FieldCount];
                        dataReaderHelper.GetValues(values);

                        var dataTable = new DataTable($"Table[{tableIndex}].Rows[{rowIndex}]");
                        dataTable.Columns.Add(" ", typeof(int));
                        dataTable.Columns.Add("Name", typeof(string));
                        dataTable.Columns.Add("Value");
                        var count = schemaTable.Rows.Count;

                        for (var i = 0; i < count; ++i)
                        {
                            var schemaRow = schemaTable.Rows[i];
                            var columnName = schemaRow["Name"].ToString();

                            var dataRow = dataTable.NewRow();
                            dataRow[0] = i + 1;
                            dataRow[1] = columnName;
                            dataRow[2] = values[i];
                            dataTable.Rows.Add(dataRow);
                        }

                        dataSet.Tables.Add(dataTable);

                        if (rowIndex == 100)
                            break;

                        ++rowIndex;
                    }

                    if (!dataReader.NextResult())
                        break;

                    ++tableIndex;
                }
            }

            ShowDataSet(dataSet);
            _tabControl.SelectedTab = _resultSetsTabPage;
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuSingleRow_Click(object sender, EventArgs e)
    {
        ExecuteQuerySingleRow();
    }

    private void mnuShowShemaTable_Click(object sender, EventArgs e)
    {
        _mnuShowShemaTable.Checked = !_mnuShowShemaTable.Checked;
        _showSchemaTable = !_showSchemaTable;
    }

    private void mnuXml_Click(object sender, EventArgs e)
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

            using (var dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    var writer = new StringWriter();
                    var read = false;

                    while (dataReader.Read())
                    {
                        if (!read)
                        {
                            read = true;
                        }

                        var fragment = (string)dataReader[0];
                        writer.Write(fragment);
                    }

                    if (read)
                    {
                        var xml = writer.ToString();
                        var xmlDocument = new XmlDocument();
                        var path = Path.GetTempFileName() + ".xml";

                        try
                        {
                            xmlDocument.LoadXml(xml);
                            xmlDocument.Save(path);
                        }
                        catch
                        {
                            var xmlWriter = new XmlTextWriter(path, Encoding.UTF8);
                            xmlWriter.WriteStartElement("DataCommanderRoot");
                            xmlWriter.WriteRaw(xml);
                            xmlWriter.WriteEndElement();
                            xmlWriter.Close();
                        }

                        var resultSetTabPage = new TabPage("Xml");
                        _resultSetsTabControl.TabPages.Add(resultSetTabPage);

                        var htmlTextBox = new HtmlTextBox();
                        htmlTextBox.Dock = DockStyle.Fill;

                        resultSetTabPage.Controls.Add(htmlTextBox);

                        htmlTextBox.Navigate(path);
                        _resultSetsTabControl.SelectedTab = resultSetTabPage;
                        _tabControl.SelectedTab = _resultSetsTabPage;
                    }

                    if (!dataReader.NextResult())
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuCreateInsert_Click(object sender, EventArgs e)
    {
        try
        {
            var textWriter = _standardOutput.TextWriter;

            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, CommandType.Text, _commandTimeout);
            var tableName = _sqlStatement.FindTableName();
            var tableIndex = 0;

            using (var dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    if (tableIndex > 0)
                    {
                        tableName = $"Table{tableIndex}";
                    }

                    var dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    var schemaTable = dataReader.GetSchemaTable();
                    var sb = new StringBuilder();

                    if (schemaTable != null)
                    {
                        if (tableName != null)
                            schemaTable.TableName = tableName;
                        else
                            tableName = schemaTable.TableName;

                        _standardOutput.WriteLine(InsertScriptFileWriter.GetCreateTableStatement(schemaTable));
                        var schemaRows = schemaTable.Rows;
                        var columnCount = schemaRows.Count;
                        sb.AppendFormat("insert into {0}(", tableName);

                        for (var i = 0; i < columnCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            var schemaRow = schemaRows[i];
                            var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                            sb.Append(columnName);
                        }
                    }

                    sb.Append(") values(");
                    var insertInto = sb.ToString();
                    var fieldCount = dataReader.FieldCount;
                    sb.Length = 0;
                    var statementCount = 0;

                    while (dataReader.Read())
                    {
                        var values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb.Append(insertInto);

                        for (var i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            var s = InsertScriptFileWriter.ToString(values[i]);
                            sb.Append(s);
                        }

                        sb.AppendLine(");");
                        ++statementCount;

                        if (statementCount % 100 == 0)
                        {
                            _standardOutput.Write(sb);
                            sb.Length = 0;
                        }
                    }

                    if (statementCount % 100 != 0)
                        _standardOutput.Write(sb);

                    if (!dataReader.NextResult())
                    {
                        break;
                    }

                    tableIndex++;
                }
            }

            _tabControl.SelectedTab = _messagesTabPage;

        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuCreateInsertSelect_Click(object sender, EventArgs e)
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, CommandType.Text, _commandTimeout);
            var tableName = _sqlStatement.FindTableName();

            if (tableName != null)
            {
                using (var dataReader = _command.ExecuteReader())
                {
                    var dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    var schemaTable = dataReader.GetSchemaTable();
                    var schemaRows = schemaTable.Rows;
                    var columnCount = schemaRows.Count;
                    var sb = new StringBuilder();
                    sb.AppendFormat("insert into {0}(", tableName);

                    for (var i = 0; i < columnCount; ++i)
                    {
                        if (i > 0)
                            sb.Append(',');

                        var schemaRow = schemaRows[i];
                        var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                        sb.Append(columnName);
                    }

                    sb.Append(")\r\nselect\r\n");
                    var insertInto = sb.ToString();
                    var fieldCount = dataReader.FieldCount;

                    while (dataReader.Read())
                    {
                        var values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb = new StringBuilder();
                        sb.Append(insertInto);

                        for (var i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(",\r\n");

                            var s = InsertScriptFileWriter.ToString(values[i]);
                            sb.AppendFormat("    {0} as {1}", s, dataReader.GetName(i));
                        }

                        _standardOutput.WriteLine(sb);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    public void LoadFile(string path)
    {
        string text;

        using (var reader = new StreamReader(path, Encoding.Default, true))
        {
            Log.Write(LogLevel.Trace, "reader.CurrentEncoding.EncodingName: {0}", reader.CurrentEncoding.EncodingName);
            text = reader.ReadToEnd();
        }

        QueryTextBox.Text = text;
        _fileName = path;
        SetText();
        SetStatusbarPanelText($"File {_fileName} loaded successfully.", SystemColors.ControlText);
    }

    private void tvObjectBrowser_ItemDrag(object sender, ItemDragEventArgs e)
    {
        var treeNode = (TreeNode)e.Item;
        var treeNode2 = (ITreeNode)treeNode.Tag;
        var text = treeNode.Text;
        _tvObjectExplorer.DoDragDrop(text, DragDropEffects.All);
    }

    private async void mnuDuplicateConnection_Click(object sender, EventArgs e)
    {
        var mainForm = DataCommanderApplication.Instance.MainForm;
        var index = mainForm.MdiChildren.Length;

        var connection = Provider.CreateConnection(_connectionString);
        connection.ConnectionName = Connection.ConnectionName;
        await connection.OpenAsync(CancellationToken.None);
        var database = Connection.Database;

        if (connection.Database != Connection.Database)
            connection.Connection.ChangeDatabase(database);

        var queryForm = new QueryForm(_mainForm, index, Provider, _connectionString, connection, mainForm.StatusBar, _colorTheme);

        if (mainForm.SelectedFont != null)
            queryForm.Font = mainForm.SelectedFont;

        queryForm.MdiParent = mainForm;
        queryForm.WindowState = WindowState;
        queryForm.Show();
    }

    public void EditRows(string query)
    {
        try
        {
            Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
            _sqlStatement = new SqlParser(query);
            _commandType = CommandType.Text;
            _openTableMode = true;
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Executing query..."));
            _stopwatch.Start();
            _timer.Start();
            const int maxRecords = int.MaxValue;
            _dataSetResultWriter = new DataSetResultWriter(AddInfoMessage, _showSchemaTable);
            IResultWriter resultWriter = _dataSetResultWriter;
            _dataAdapter = new AsyncDataAdapter(Provider, new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray(), maxRecords,
                _rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
            _dataAdapter.Start();
        }
        catch (Exception ex)
        {
            EndFill(_dataAdapter, ex);
        }
    }

    private void sQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SetResultWriterType(ResultWriterType.SqLite);
    }

    private void createSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var sqlStatement = new SqlParser(Query);
        _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
        var maxRecords = int.MaxValue;
        var tableName = sqlStatement.FindTableName();
        var sqlCeResultWriter = new SqlCeResultWriter(_textBoxWriter, tableName);
        IAsyncDataAdapter asyncDataAdatper = new AsyncDataAdapter(
            Provider,
            new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray(),
            maxRecords, _rowBlockSize, sqlCeResultWriter, EndFillInvoker, WriteEndInvoker);
        asyncDataAdatper.Start();
    }

    private void SetTransaction(IDbTransaction transaction)
    {
        if (_transaction == null && transaction != null)
        {
            _transaction = transaction;
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction created successfully."));
            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = false;
            _commitTransactionToolStripMenuItem.Enabled = true;
            _rollbackTransactionToolStripMenuItem.Enabled = true;
        }
    }

    private void InvokeSetTransaction(IDbTransaction transaction)
    {
        Invoke(new Action<IDbTransaction>(SetTransaction), transaction);
    }

    private void beginTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction == null)
        {
            var transaction = Connection.Connection.BeginTransaction();
            SetTransaction(transaction);
        }
    }

    private void commitTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;

            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction commited successfully."));

            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = true;
            _commitTransactionToolStripMenuItem.Enabled = false;
            _rollbackTransactionToolStripMenuItem.Enabled = false;
        }
    }

    private void rollbackTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction != null)
        {
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction rolled back successfully."));
            }
            catch (Exception ex)
            {
                var message = $"Rollback failed. Exception:\r\n{ex.ToLogString()}";
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Error, null, message));
            }

            _transaction = null;
            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = true;
            _commitTransactionToolStripMenuItem.Enabled = false;
            _rollbackTransactionToolStripMenuItem.Enabled = false;
        }
    }

    internal void ScriptQueryAsCreateTable()
    {
        var sqlStatement = new SqlParser(Query);
        var command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

        var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
        var index = Array.IndexOf(forms, this);
        IProvider destinationProvider;

        if (index < forms.Length - 1)
        {
            var nextQueryForm = (QueryForm)forms[index + 1];
            destinationProvider = nextQueryForm.Provider;
        }
        else
            destinationProvider = Provider;

        DataTable schemaTable;
        string[] dataTypeNames;

        using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
        {
            schemaTable = dataReader.GetSchemaTable();
            dataTypeNames = new string[dataReader.FieldCount];

            for (var i = 0; i < dataReader.FieldCount; i++)
                dataTypeNames[i] = dataReader.GetDataTypeName(i);
        }

        var tableName = command.CommandType == CommandType.StoredProcedure ? command.CommandText : sqlStatement.FindTableName();
        var createTable = new StringBuilder();
        createTable.AppendFormat("create table [{0}]\r\n(\r\n", tableName);
        var stringTable = new StringTable(3);
        var last = schemaTable.Rows.Count - 1;

        for (var i = 0; i <= last; i++)
        {
            var dataRow = schemaTable.Rows[i];
            var schemaRow = FoundationDbColumnFactory.Create(dataRow);
            var row = stringTable.NewRow();
            var typeName = destinationProvider.GetColumnTypeName(Provider, dataRow, dataTypeNames[i]);
            row[1] = schemaRow.ColumnName;
            row[2] = typeName;
            var allowDbNull = schemaRow.AllowDbNull;

            if (allowDbNull == false)
                row[2] += " not null";

            if (i < last)
                row[2] += ',';

            stringTable.Rows.Add(row);
        }

        createTable.Append(stringTable.ToString(4));
        createTable.Append(')');
        var commandText = createTable.ToString();

        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "\r\n" + commandText));
    }

    internal void CopyTable()
    {
        var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
        var index = Array.IndexOf(forms, this);
        if (index < forms.Length - 1)
        {
            var nextQueryForm = (QueryForm)forms[index + 1];
            var destinationProvider = nextQueryForm.Provider;
            var destinationConnection = nextQueryForm.Connection;
            var sqlStatement = new SqlParser(Query);
            _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
            var tableName = _command.CommandType == CommandType.StoredProcedure ? _command.CommandText : sqlStatement.FindTableName();

            if (tableName[0] == '[' && destinationProvider.Name == "System.Data.OracleClient")
                tableName = tableName.Substring(1, tableName.Length - 2);

            IResultWriter resultWriter = new CopyResultWriter(AddInfoMessage, destinationProvider, destinationConnection, tableName,
                nextQueryForm.InvokeSetTransaction, CancellationToken.None);
            var maxRecords = int.MaxValue;
            var rowBlockSize = 10000;
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, "Copying table..."));
            SetStatusbarPanelText("Copying table...", _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText);
            SetGui(CommandState.Cancel);
            _errorCount = 0;
            _stopwatch.Start();
            _timer.Start();
            _dataAdapter = new AsyncDataAdapter(Provider, new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray(), maxRecords,
                rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
            _dataAdapter.Start();
        }
        else
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Please open a destination connection."));
    }

    internal void CopyTableWithSqlBulkCopy()
    {
        // var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
        // var index = Array.IndexOf(forms, this);
        // if (index < forms.Length - 1)
        // {
        //     var nextQueryForm = (QueryForm)forms[index + 1];
        //     var destinationProvider = nextQueryForm.Provider;
        //     var destinationConnection = (SqlConnection)nextQueryForm.Connection.Connection;
        //     var destionationTransaction = (SqlTransaction)nextQueryForm._transaction;
        //     var sqlStatement = new SqlParser(Query);
        //     _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
        //     string tableName;
        //     if (_command.CommandType == CommandType.StoredProcedure)
        //         tableName = _command.CommandText;
        //     else
        //         tableName = sqlStatement.FindTableName();
        //
        //     //IResultWriter resultWriter = new SqlBulkCopyResultWriter( this.AddInfoMessage, destinationProvider, destinationConnection, tableName, nextQueryForm.InvokeSetTransaction );
        //     //var maxRecords = int.MaxValue;
        //     //var rowBlockSize = 10000;
        //     AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, "Copying table..."));
        //     SetStatusbarPanelText("Copying table...", SystemColors.ControlText);
        //     SetGui(CommandState.Cancel);
        //     _errorCount = 0;
        //     _stopwatch.Start();
        //     _timer.Start();
        //     _dataAdapter = new SqlBulkCopyAsyncDataAdapter(destinationConnection, destionationTransaction, tableName, AddInfoMessage);
        //     //Provider,
        //     //new[]
        //     //{
        //     //    new AsyncDataAdapterCommand
        //     //    {
        //     //        LineIndex = 0,
        //     //        Command = _command
        //     //    }
        //     //},
        //     //maxRecords, rowBlockSize, null, EndFillInvoker, WriteEndInvoker);
        //     _dataAdapter.Start();
        // }
        // else
        //     AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Please open a destination connection."));
    }

    private void insertScriptFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SetResultWriterType(ResultWriterType.InsertScriptFile);
    }

    private void TableStyleMenuItem_Click(object sender, EventArgs e)
    {
        var item = (ToolStripMenuItem)sender;
        var tableStyle = (ResultWriterType)item.Tag;
        SetResultWriterType(tableStyle);
    }

    public void ShowText(string text)
    {
        var mainForm = DataCommanderApplication.Instance.MainForm;
        mainForm.Cursor = Cursors.WaitCursor;
            
        try
        {
            var selectionStart = _queryTextBox.RichTextBox.TextLength;
            var append = text;
            _queryTextBox.RichTextBox.AppendText(append);
            _queryTextBox.RichTextBox.SelectionStart = selectionStart;
            _queryTextBox.RichTextBox.SelectionLength = append.Length;

            _queryTextBox.Focus();
        }
        catch (Exception e)
        {
            ShowMessage(e);
        }
        finally
        {
            mainForm.Cursor = Cursors.Default;
        }
    }

    public void ClipboardSetText(string text) => Clipboard.SetText(text);

    [Flags]
    private enum Tchittestflags
    {
        TchtNowhere = 1,
        TchtOnitemicon = 2,
        TchtOnitemlabel = 4,
        TchtOnitem = TchtOnitemicon | TchtOnitemlabel
    }

    private const int TcmHittest = 0x130D;

    [StructLayout(LayoutKind.Sequential)]
    private struct Tchittestinfo
    {
        public readonly Point pt;
        public readonly Tchittestflags flags;

        public Tchittestinfo(int x, int y)
        {
            pt = new Point(x, y);
            flags = Tchittestflags.TchtOnitem;
        }
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref Tchittestinfo lParam);

    private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e) => ExecuteQuery();
    private void aToolStripMenuItem_Click(object sender, EventArgs e) => ExecuteQuery();
    private void cancelExecutingQueryButton_Click(object sender, EventArgs e) => CancelCommandQuery();
    private void toolStripMenuItem1_Click(object sender, EventArgs e) => ExecuteQuery();
    private void editRowsToolStripMenuItem_Click(object sender, EventArgs e) => EditRows(Query);

    private void parseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var executor = Connection.Connection.CreateCommandExecutor();
        var on = false;
        try
        {
            executor.ExecuteNonQuery(new CreateCommandRequest("SET PARSEONLY ON"));
            on = true;
            var query = Query;
            bool succeeded;

            try
            {
                executor.ExecuteNonQuery(new CreateCommandRequest(query));
                succeeded = _infoMessages.Count == 0;
            }
            catch (Exception exception)
            {
                succeeded = false;
                var infoMessages = Provider.ToInfoMessages(exception);
                AddInfoMessages(infoMessages);
            }

            if (succeeded)
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Command(s) completed successfully."));
        }
        catch (Exception exception)
        {
            var infoMessages = Provider.ToInfoMessages(exception);
            AddInfoMessages(infoMessages);
        }

        if (on)
            executor.ExecuteNonQuery(new CreateCommandRequest("SET PARSEONLY OFF"));
    }

    public void SetStatusbarPanelText(string text, Color color)
    {
        _sbPanelText.Text = text;
        _sbPanelText.ForeColor = color;
        //Refresh();
    }

    #endregion

    private void createCCommandQueryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        const string text = @"/* Query Configuration
{
  ""Using"": ""using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;"",
  ""Namespace"": ""Company.Product.CommandOrQueryName"",
  ""Name"": ""CommandOrQueryName"",
  ""Results"": [
    ""Result1Item(s)"",
    ""Result2Item(s)"",
  ]
}
*/
declare @int int = 0
declare @date date = getdate()
-- CommandText
select
    @int as [Int],
    @date as [Date]
	
select
    @int as [Int],
    @date as [Date]";

        AppendQueryText(text);
    }

    private void undoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //var canUndo = _queryTextBox.RichTextBox.CanUndo;
        //if (canUndo)
        //{
        //    var actionName = _queryTextBox.RichTextBox.UndoActionName;
        //    Trace.WriteLine($"UndoActionName:{actionName}");
        //    _queryTextBox.RichTextBox.Undo();
        //    _queryTextBox.RichTextBox.ClearUndo();
        //}

        _queryTextBox.Undo();
    }

    private void redoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //var canRedo = _queryTextBox.RichTextBox.CanRedo;
        //if (canRedo)
        //{
        //    var actionName = _queryTextBox.RichTextBox.RedoActionName;
        //    Trace.WriteLine($"RedoActionName:{actionName}");
        //    _queryTextBox.RichTextBox.Redo();
        //}

        _queryTextBox.Redo();
    }
}