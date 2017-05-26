using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
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
using DataCommander.Providers.Connection;
using DataCommander.Providers.ResultWriter;
using Foundation;
using Foundation.Configuration;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Foundation.Linq;
using Foundation.Text;
using Foundation.Threading;
using Foundation.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace DataCommander.Providers.Query
{
    /// <summary>
    /// Summary description for QueryForm.
    /// </summary>
    public sealed class QueryForm : Form
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
        private SqlStatement _sqlStatement;
        private IDbCommand _command;
        private CommandType _commandType = CommandType.Text;
        private IAsyncDataAdapter _dataAdapter;
        private bool _cancel;
        private readonly Timer _timer = new Timer();
        private readonly Stopwatch _stopwatch = new Stopwatch();
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

        private readonly ConcurrentQueue<InfoMessage> _infoMessages = new ConcurrentQueue<InfoMessage>();
        private int _errorCount;
        private readonly LimitedConcurrencyLevelTaskScheduler _scheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly EventWaitHandle _enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly ColorTheme _colorTheme;

        #endregion

        #region Constructors

        static QueryForm()
        {
            NumberFormat = new NumberFormatInfo {NumberDecimalSeparator = "."};
        }

        public QueryForm(MainForm mainForm,int index,IProvider provider,string connectionString,ConnectionBase connection,StatusStrip parentStatusBar,ColorTheme colorTheme)
        {
            GarbageMonitor.Add("QueryForm", this);

            this._mainForm = mainForm;
            this.Provider = provider;
            this._connectionString = connectionString;
            this.Connection = connection;
            this._parentStatusBar = parentStatusBar;
            this._colorTheme = colorTheme;
            connection.InfoMessage += this.Connection_InfoMessage;
            connection.DatabaseChanged += this.Connection_DatabaseChanged;
            this._timer.Tick += this.Timer_Tick;

            var task = new Task(this.ConsumeInfoMessages);
            task.Start(this._scheduler);

            this._messagesTextBox = new RichTextBox();
            this.components.Add(this._messagesTextBox);
            GarbageMonitor.Add("QueryForm.messagesTextBox", this._messagesTextBox);
            this._messagesTextBox.Multiline = true;
            this._messagesTextBox.WordWrap = false;
            this._messagesTextBox.Dock = DockStyle.Fill;
            this._messagesTextBox.ScrollBars = RichTextBoxScrollBars.Both;

            this._messagesTabPage = new TabPage("Messages");
            this._messagesTabPage.Controls.Add(this._messagesTextBox);

            this.InitializeComponent();
            GarbageMonitor.Add("queryForm.toolStrip", this._toolStrip);
            this._mnuFind.Click += this.mnuFind_Click;
            this._mnuFindNext.Click += this.mnuFindNext_Click;
            this._mnuPaste.Click += this.mnuPaste_Click;
            this._mnuGoTo.Click += this.mnuGoTo_Click;
            this._mnuClearCache.Click += this.mnuClearCache_Click;

            var sqlKeyWords = Settings.CurrentType.Attributes["Sql92ReservedWords"].GetValue<string[]>();
            var providerKeyWords = provider.KeyWords;

            this.QueryTextBox.AddKeyWords(new string[] {"exec"}, colorTheme != null
                ? colorTheme.ExecKeyWordColor
                : Color.Green);
            this.QueryTextBox.AddKeyWords(sqlKeyWords, colorTheme != null
                ? colorTheme.SqlKeyWordColor
                : Color.Blue);
            this.QueryTextBox.AddKeyWords(providerKeyWords, colorTheme != null
                ? colorTheme.ProviderKeyWordColor
                : Color.Red);

            this.QueryTextBox.CaretPositionPanel = this._sbPanelCaretPosition;

            this.SetText();

            this._resultSetsTabPage = new TabPage("Results");
            this._resultSetsTabControl = new TabControl();
            this._resultSetsTabControl.MouseUp += this.resultSetsTabControl_MouseUp;
            this._resultSetsTabControl.Alignment = TabAlignment.Top;
            this._resultSetsTabControl.Dock = DockStyle.Fill;
            this._resultSetsTabPage.Controls.Add(this._resultSetsTabControl);

            this._tabControl.TabPages.Add(this._resultSetsTabPage);
            this._tabControl.TabPages.Add(this._messagesTabPage);
            this._tabControl.SelectedTab = this._messagesTabPage;

            this._standardOutput = new StandardOutput(new TextBoxWriter(this._messagesTextBox), this);

            this._textBoxWriter = new TextBoxWriter(this._messagesTextBox);

            var objectExplorer = provider.ObjectExplorer;
            if (objectExplorer != null)
            {
                objectExplorer.SetConnection(connectionString, connection.Connection);
                this.AddNodes(this._tvObjectExplorer.Nodes, objectExplorer.GetChildren(true), objectExplorer.Sortable);
            }
            else
            {
                this._tvObjectExplorer.Visible = false;
                this._splitterObjectExplorer.Visible = false;
                this._mnuObjectExplorer.Enabled = false;
            }

            var text = $"&{index + 1} - {this.Text}";

            this._database = connection.Database;
            this.SetResultWriterType(ResultWriterType.DataGrid);

            var node = Settings.CurrentType;
            var attributes = node.Attributes;
            this._rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
            this._htmlMaxRecords = attributes["HtmlMaxRecords"].GetValue<int>();
            this._wordMaxRecords = attributes["WordMaxRecords"].GetValue<int>();
            this._rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
            this._timer.Interval = attributes["TimerInterval"].GetValue<int>();

            this.SettingsChanged(null, null);
            Settings.Changed += this.SettingsChanged;

            if (colorTheme != null)
            {
                foreach (var menuItem in _mainMenu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>())
                {
                    foreach (ToolStripItem x in menuItem.DropDownItems)
                    {
                        x.BackColor = colorTheme.BackColor;
                        x.ForeColor = colorTheme.ForeColor;
                    }
                }

                _toolStrip.BackColor = colorTheme.BackColor;
                _toolStrip.ForeColor = colorTheme.ForeColor;

                _tvObjectExplorer.BackColor = colorTheme.BackColor;
                _tvObjectExplorer.ForeColor = colorTheme.ForeColor;

                _tabControl.BackColor = colorTheme.BackColor;
                _tabControl.ForeColor = colorTheme.ForeColor;

                foreach (Control control in _tabControl.Controls)
                {
                    control.BackColor = colorTheme.BackColor;
                    control.ForeColor = colorTheme.ForeColor;
                }

                _resultSetsTabControl.BackColor = colorTheme.BackColor;
                _resultSetsTabControl.ForeColor = colorTheme.ForeColor;

                _messagesTextBox.BackColor = colorTheme.BackColor;
                _messagesTextBox.ForeColor = colorTheme.ForeColor;

                _statusBar.BackColor = colorTheme.BackColor;
                _statusBar.ForeColor = colorTheme.ForeColor;

                foreach (ToolStripItem item in _statusBar.Items)
                {
                    item.BackColor = colorTheme.BackColor;
                    item.ForeColor = colorTheme.ForeColor;
                }
            }
        }

        private void CloseResultTabPage(TabPage tabPage)
        {
            foreach (Control control in tabPage.Controls)
            {
                control.Dispose();
            }

            tabPage.Controls.Clear();

            //GarbageMonitor.SetDisposeTime(control, LocalTime.Default.Now);
            //control.Dispose();
        }

        private void CloseResultSetTabPage(TabPage tabPage)
        {
            this._resultSetsTabControl.TabPages.Remove(tabPage);
            var control = tabPage.Controls[0];
            var tabControl = control as TabControl;
            if (tabControl != null)
            {
                var tabPages = tabControl.TabPages.Cast<TabPage>().ToList();
                foreach (var subTabPage in tabPages)
                {
                    tabControl.TabPages.Remove(subTabPage);
                    this.CloseResultTabPage(subTabPage);
                    // GarbageMonitor.SetDisposeTime(subTabPage, LocalTime.Default.Now);
                    // subTabPage.Dispose();
                }
                // GarbageMonitor.SetDisposeTime(tabControl, LocalTime.Default.Now);
                // tabControl.Dispose();
                // GarbageMonitor.SetDisposeTime(tabPage, LocalTime.Default.Now);
                // tabPage.Dispose();
            }
            else
            {
                this.CloseResultTabPage(tabPage);
            }
        }

        private void CloseResultSetTabPage_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            var tabPage = (TabPage)toolStripMenuItem.Tag;
            this.CloseResultSetTabPage(tabPage);
            toolStripMenuItem.Tag = null;
        }

        private void resultSetsTabControl_MouseUp(object sender, MouseEventArgs e)
        {
            var hitTestInfo = new Tchittestinfo(e.X, e.Y);
            var index = SendMessage(this._resultSetsTabControl.Handle, TcmHittest, IntPtr.Zero, ref hitTestInfo);
            var hotTab = index >= 0 ? this._resultSetsTabControl.TabPages[index] : null;

            switch (e.Button)
            {
                case MouseButtons.Middle:
                    if (index >= 0)
                    {
                        this.CloseResultSetTabPage(hotTab);
                    }
                    break;

                case MouseButtons.Right:
                    if (index >= 0)
                    {
                        var contextMenu = new ContextMenuStrip(this.components);
                        contextMenu.Items.Add(new ToolStripMenuItem("Close", null, this.CloseResultSetTabPage_Click)
                        {
                            Tag = hotTab
                        });
                        contextMenu.Items.Add(new ToolStripMenuItem("Close all", null, this.mnuCloseAllTabPages_Click, Keys.Control | Keys.Shift | Keys.F4));
                        contextMenu.Show(this._resultSetsTabControl, e.Location);
                    }
                    break;
            }
        }

        #endregion

        #region Properties

        public CommandState ButtonState { get; private set; }

        public ConnectionBase Connection { get; private set; }

        public override Font Font
        {
            set
            {
                this._font = value;
                this.QueryTextBox.Font = value;
                var size1 = TextRenderer.MeasureText("1", value);
                var size2 = TextRenderer.MeasureText("12", value);
                var width = this.QueryTextBox.TabSize*(size2.Width - size1.Width);
                var tabs = new int[12];

                for (var i = 0; i < tabs.Length; i++)
                    tabs[i] = (i + 1) * width;

                this.QueryTextBox.RichTextBox.Font = value;
                this.QueryTextBox.RichTextBox.SelectionTabs = tabs;

                this._messagesTextBox.Font = value;
                this._messagesTextBox.SelectionTabs = tabs;
            }
        }

        public static NumberFormatInfo NumberFormat { get; }

        public IProvider Provider { get; }

        private string Query
        {
            get
            {
                var query = this.QueryTextBox.SelectedText;

                if (query.Length == 0)
                {
                    query = this.QueryTextBox.Text;
                }

                query = query.Replace("\n", "\r\n");
                return query;
            }
        }

        public QueryTextBox QueryTextBox { get; private set; }

        internal int ResultSetCount { get; private set; }

        public ResultWriterType TableStyle { get; private set; }

        internal ToolStrip ToolStrip => this._toolStrip;

        #endregion

        #region Public Methods

        public void AddDataTable(DataTable dataTable, ResultWriterType tableStyle)
        {
            switch (tableStyle)
            {
                case ResultWriterType.Text:
                    this.ShowDataTableText(dataTable);
                    break;

                case ResultWriterType.DataGrid:
                    this.ShowDataTableDataGrid(dataTable);
                    break;

                case ResultWriterType.Html:
                    this.ShowDataViewHtml(dataTable.DefaultView);
                    break;

                case ResultWriterType.Rtf:
                    this.ShowDataTableRtf(dataTable);
                    break;

                case ResultWriterType.ListView:
                    this.ShowDataTableListView(dataTable);
                    break;

                case ResultWriterType.Excel:
                    this.ShowDataTableExcel(dataTable);
                    break;
            }
        }

        public void AppendQueryText(string text)
        {
            this.QueryTextBox.RichTextBox.AppendText(text);
        }

        public void ShowDataSet(DataSet dataSet)
        {
            using (var log = LogFactory.Instance.GetCurrentMethodLog())
            {
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataSet tableSchema = null;
                    string text;
                    if (this._openTableMode)
                    {
                        var tableName = this._sqlStatement.FindTableName();
                        text = tableName;
                        dataSet.Tables[0].TableName = tableName;
                        tableSchema = this.Provider.GetTableSchema(this.Connection.Connection, tableName);
                    }
                    else
                    {
                        this.ResultSetCount++;
                        text = $"Set {this.ResultSetCount}";
                    }
                    var resultSetTabPage = new TabPage(text);
                    GarbageMonitor.Add("resultSetTabPage", resultSetTabPage);
                    resultSetTabPage.ToolTipText = null; // TODO
                    this._resultSetsTabControl.TabPages.Add(resultSetTabPage);
                    this._resultSetsTabControl.SelectedTab = resultSetTabPage;
                    if (dataSet.Tables.Count > 1)
                    {
                        var tabControl = new TabControl {Dock = DockStyle.Fill};
                        var index = 0;
                        foreach (DataTable dataTable in dataSet.Tables)
                        {
                            var commandBuilder = this.Provider.DbProviderFactory.CreateCommandBuilder();
                            var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataTable, tableSchema, this.TableStyle,
                                !this._openTableMode, this._sbPanelText, _colorTheme);
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
                        var commandBuilder = this.Provider.DbProviderFactory.CreateCommandBuilder();
                        var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataSet.Tables[0], tableSchema, this.TableStyle,
                            !this._openTableMode, this._sbPanelText, _colorTheme);
                        control.Dock = DockStyle.Fill;
                        resultSetTabPage.Controls.Add(control);
                    }
                }
            }
        }

        public void ShowXml(string tabPageName, string xml)
        {
            var htmlTextBox = new HtmlTextBox();
            htmlTextBox.Dock = DockStyle.Fill;

            var tabPage = new TabPage(tabPageName);
            this._tabControl.TabPages.Add(tabPage);
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof (QueryForm));
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
            this.QueryTextBox = new QueryTextBox(_colorTheme);
            this._mainMenu.SuspendLayout();
            this._statusBar.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this._mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._menuItem9,
                this._menuItem8,
                this._menuItem1,
                this._menuItem3
            });
            this._mainMenu.Location = new System.Drawing.Point(0, 0);
            this._mainMenu.Name = "_mainMenu";
            this._mainMenu.Size = new System.Drawing.Size(1016, 24);
            this._mainMenu.TabIndex = 0;
            this._mainMenu.Visible = false;
            // 
            // menuItem9
            // 
            this._menuItem9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuSave,
                this._mnuSaveAs,
                this._mnuDuplicateConnection
            });
            this._menuItem9.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this._menuItem9.MergeIndex = 0;
            this._menuItem9.Name = "_menuItem9";
            this._menuItem9.Size = new System.Drawing.Size(37, 20);
            this._menuItem9.Text = "&File";
            // 
            // mnuSave
            // 
            this._mnuSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._mnuSave.MergeIndex = 2;
            this._mnuSave.Name = "_mnuSave";
            this._mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._mnuSave.Size = new System.Drawing.Size(230, 22);
            this._mnuSave.Text = "&Save";
            this._mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this._mnuSaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._mnuSaveAs.MergeIndex = 3;
            this._mnuSaveAs.Name = "_mnuSaveAs";
            this._mnuSaveAs.Size = new System.Drawing.Size(230, 22);
            this._mnuSaveAs.Text = "Save &As";
            this._mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // mnuDuplicateConnection
            // 
            this._mnuDuplicateConnection.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._mnuDuplicateConnection.MergeIndex = 4;
            this._mnuDuplicateConnection.Name = "_mnuDuplicateConnection";
            this._mnuDuplicateConnection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this._mnuDuplicateConnection.Size = new System.Drawing.Size(230, 22);
            this._mnuDuplicateConnection.Text = "Duplicate connection";
            this._mnuDuplicateConnection.Click += new System.EventHandler(this.mnuDuplicateConnection_Click);
            // 
            // menuItem8
            // 
            this._menuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuPaste,
                this._mnuFind,
                this._mnuFindNext,
                this._mnuCodeCompletion,
                this._mnuGoTo
            });
            this._menuItem8.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._menuItem8.MergeIndex = 2;
            this._menuItem8.Name = "_menuItem8";
            this._menuItem8.Size = new System.Drawing.Size(39, 20);
            this._menuItem8.Text = "&Edit";
            // 
            // mnuPaste
            // 
            this._mnuPaste.Image = ((System.Drawing.Image)(resources.GetObject("mnuPaste.Image")));
            this._mnuPaste.MergeIndex = 0;
            this._mnuPaste.Name = "_mnuPaste";
            this._mnuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._mnuPaste.Size = new System.Drawing.Size(166, 22);
            this._mnuPaste.Text = "&Paste";
            // 
            // mnuFind
            // 
            this._mnuFind.Image = ((System.Drawing.Image)(resources.GetObject("mnuFind.Image")));
            this._mnuFind.MergeIndex = 1;
            this._mnuFind.Name = "_mnuFind";
            this._mnuFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this._mnuFind.Size = new System.Drawing.Size(166, 22);
            this._mnuFind.Text = "&Find";
            // 
            // mnuFindNext
            // 
            this._mnuFindNext.MergeIndex = 2;
            this._mnuFindNext.Name = "_mnuFindNext";
            this._mnuFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this._mnuFindNext.Size = new System.Drawing.Size(166, 22);
            this._mnuFindNext.Text = "Find &Next";
            // 
            // mnuCodeCompletion
            // 
            this._mnuCodeCompletion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuListMembers,
                this._mnuClearCache
            });
            this._mnuCodeCompletion.MergeIndex = 3;
            this._mnuCodeCompletion.Name = "_mnuCodeCompletion";
            this._mnuCodeCompletion.Size = new System.Drawing.Size(166, 22);
            this._mnuCodeCompletion.Text = "&Code completion";
            // 
            // mnuListMembers
            // 
            this._mnuListMembers.MergeIndex = 0;
            this._mnuListMembers.Name = "_mnuListMembers";
            this._mnuListMembers.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this._mnuListMembers.Size = new System.Drawing.Size(211, 22);
            this._mnuListMembers.Text = "&List Members";
            this._mnuListMembers.Click += new System.EventHandler(this.mnuListMembers_Click);
            // 
            // mnuClearCache
            // 
            this._mnuClearCache.MergeIndex = 1;
            this._mnuClearCache.Name = "_mnuClearCache";
            this._mnuClearCache.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                            | System.Windows.Forms.Keys.C)));
            this._mnuClearCache.Size = new System.Drawing.Size(211, 22);
            this._mnuClearCache.Text = "&Clear Cache";
            // 
            // mnuGoTo
            // 
            this._mnuGoTo.MergeIndex = 4;
            this._mnuGoTo.Name = "_mnuGoTo";
            this._mnuGoTo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this._mnuGoTo.Size = new System.Drawing.Size(166, 22);
            this._mnuGoTo.Text = "Go To...";
            // 
            // menuItem1
            // 
            this._menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
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
                this._rollbackTransactionToolStripMenuItem
            });
            this._menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._menuItem1.MergeIndex = 3;
            this._menuItem1.Name = "_menuItem1";
            this._menuItem1.Size = new System.Drawing.Size(51, 20);
            this._menuItem1.Text = "&Query";
            // 
            // menuItem7
            // 
            this._menuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuCommandTypeText,
                this._mnuCommandTypeStoredProcedure
            });
            this._menuItem7.MergeIndex = 0;
            this._menuItem7.Name = "_menuItem7";
            this._menuItem7.Size = new System.Drawing.Size(269, 22);
            this._menuItem7.Text = "Command&Type";
            // 
            // mnuCommandTypeText
            // 
            this._mnuCommandTypeText.Checked = true;
            this._mnuCommandTypeText.CheckState = System.Windows.Forms.CheckState.Checked;
            this._mnuCommandTypeText.MergeIndex = 0;
            this._mnuCommandTypeText.Name = "_mnuCommandTypeText";
            this._mnuCommandTypeText.Size = new System.Drawing.Size(165, 22);
            this._mnuCommandTypeText.Text = "Text";
            this._mnuCommandTypeText.Click += new System.EventHandler(this.mnuCommandTypeText_Click);
            // 
            // mnuCommandTypeStoredProcedure
            // 
            this._mnuCommandTypeStoredProcedure.MergeIndex = 1;
            this._mnuCommandTypeStoredProcedure.Name = "_mnuCommandTypeStoredProcedure";
            this._mnuCommandTypeStoredProcedure.Size = new System.Drawing.Size(165, 22);
            this._mnuCommandTypeStoredProcedure.Text = "Stored Procedure";
            this._mnuCommandTypeStoredProcedure.Click += new System.EventHandler(this.mnuCommandTypeStoredProcedure_Click);
            // 
            // mnuDescribeParameters
            // 
            this._mnuDescribeParameters.MergeIndex = 1;
            this._mnuDescribeParameters.Name = "_mnuDescribeParameters";
            this._mnuDescribeParameters.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this._mnuDescribeParameters.Size = new System.Drawing.Size(269, 22);
            this._mnuDescribeParameters.Text = "Describe &Parameters";
            this._mnuDescribeParameters.Click += new System.EventHandler(this.mnuDescribeParameters_Click);
            // 
            // toolStripSeparator2
            // 
            this._toolStripSeparator2.Name = "_toolStripSeparator2";
            this._toolStripSeparator2.Size = new System.Drawing.Size(266, 6);
            // 
            // mnuShowShemaTable
            // 
            this._mnuShowShemaTable.MergeIndex = 3;
            this._mnuShowShemaTable.Name = "_mnuShowShemaTable";
            this._mnuShowShemaTable.Size = new System.Drawing.Size(269, 22);
            this._mnuShowShemaTable.Text = "Show SchemaTable";
            this._mnuShowShemaTable.Click += new System.EventHandler(this.mnuShowShemaTable_Click);
            // 
            // executeQueryToolStripMenuItem
            // 
            this._executeQueryToolStripMenuItem.Name = "_executeQueryToolStripMenuItem";
            this._executeQueryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this._executeQueryToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._executeQueryToolStripMenuItem.Text = "Execute Query";
            this._executeQueryToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // mnuExecuteQuerySingleRow
            // 
            this._mnuExecuteQuerySingleRow.MergeIndex = 6;
            this._mnuExecuteQuerySingleRow.Name = "_mnuExecuteQuerySingleRow";
            this._mnuExecuteQuerySingleRow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this._mnuExecuteQuerySingleRow.Size = new System.Drawing.Size(269, 22);
            this._mnuExecuteQuerySingleRow.Text = "Execute Query (SingleRow)";
            this._mnuExecuteQuerySingleRow.Click += new System.EventHandler(this.mnuSingleRow_Click);
            // 
            // mnuExecuteQuerySchemaOnly
            // 
            this._mnuExecuteQuerySchemaOnly.MergeIndex = 7;
            this._mnuExecuteQuerySchemaOnly.Name = "_mnuExecuteQuerySchemaOnly";
            this._mnuExecuteQuerySchemaOnly.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this._mnuExecuteQuerySchemaOnly.Size = new System.Drawing.Size(269, 22);
            this._mnuExecuteQuerySchemaOnly.Text = "Execute Query (Schema only)";
            this._mnuExecuteQuerySchemaOnly.Click += new System.EventHandler(this.mnuResultSchema_Click);
            // 
            // mnuExecuteQueryKeyInfo
            // 
            this._mnuExecuteQueryKeyInfo.MergeIndex = 8;
            this._mnuExecuteQueryKeyInfo.Name = "_mnuExecuteQueryKeyInfo";
            this._mnuExecuteQueryKeyInfo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this._mnuExecuteQueryKeyInfo.Size = new System.Drawing.Size(269, 22);
            this._mnuExecuteQueryKeyInfo.Text = "Execute Query (&KeyInfo)";
            this._mnuExecuteQueryKeyInfo.Click += new System.EventHandler(this.mnuKeyInfo_Click);
            // 
            // mnuExecuteQueryXml
            // 
            this._mnuExecuteQueryXml.MergeIndex = 9;
            this._mnuExecuteQueryXml.Name = "_mnuExecuteQueryXml";
            this._mnuExecuteQueryXml.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                                 | System.Windows.Forms.Keys.X)));
            this._mnuExecuteQueryXml.Size = new System.Drawing.Size(269, 22);
            this._mnuExecuteQueryXml.Text = "Execute Query (XML)";
            this._mnuExecuteQueryXml.Click += new System.EventHandler(this.mnuXml_Click);
            // 
            // mnuOpenTable
            // 
            this._mnuOpenTable.MergeIndex = 10;
            this._mnuOpenTable.Name = "_mnuOpenTable";
            this._mnuOpenTable.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                           | System.Windows.Forms.Keys.O)));
            this._mnuOpenTable.Size = new System.Drawing.Size(269, 22);
            this._mnuOpenTable.Text = "Open Table";
            this._mnuOpenTable.Click += new System.EventHandler(this.mnuOpenTable_Click);
            // 
            // mnuCancel
            // 
            this._mnuCancel.Enabled = false;
            this._mnuCancel.MergeIndex = 11;
            this._mnuCancel.Name = "_mnuCancel";
            this._mnuCancel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Pause)));
            this._mnuCancel.Size = new System.Drawing.Size(269, 22);
            this._mnuCancel.Text = "&Cancel Executing Query";
            this._mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // parseToolStripMenuItem
            // 
            this._parseToolStripMenuItem.Name = "_parseToolStripMenuItem";
            this._parseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this._parseToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._parseToolStripMenuItem.Text = "Parse";
            this._parseToolStripMenuItem.Click += new System.EventHandler(this.parseToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            this._toolStripSeparator1.Size = new System.Drawing.Size(266, 6);
            // 
            // menuItem2
            // 
            this._menuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuText,
                this._mnuDataGrid,
                this._mnuHtml,
                this._mnuRtf,
                this._mnuListView,
                this._mnuExcel,
                this._menuResultModeFile,
                this._sQLiteDatabaseToolStripMenuItem,
                this._insertScriptFileToolStripMenuItem
            });
            this._menuItem2.MergeIndex = 13;
            this._menuItem2.Name = "_menuItem2";
            this._menuItem2.Size = new System.Drawing.Size(269, 22);
            this._menuItem2.Text = "Result &Mode";
            // 
            // mnuText
            // 
            this._mnuText.MergeIndex = 0;
            this._mnuText.Name = "_mnuText";
            this._mnuText.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this._mnuText.Size = new System.Drawing.Size(221, 22);
            this._mnuText.Text = "&Text";
            this._mnuText.Click += new System.EventHandler(this.mnuText_Click);
            // 
            // mnuDataGrid
            // 
            this._mnuDataGrid.MergeIndex = 1;
            this._mnuDataGrid.Name = "_mnuDataGrid";
            this._mnuDataGrid.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this._mnuDataGrid.Size = new System.Drawing.Size(221, 22);
            this._mnuDataGrid.Text = "&DataGrid";
            this._mnuDataGrid.Click += new System.EventHandler(this.mnuDataGrid_Click);
            // 
            // mnuHtml
            // 
            this._mnuHtml.MergeIndex = 2;
            this._mnuHtml.Name = "_mnuHtml";
            this._mnuHtml.Size = new System.Drawing.Size(221, 22);
            this._mnuHtml.Text = "&Html";
            this._mnuHtml.Click += new System.EventHandler(this.mnuHtml_Click);
            // 
            // mnuRtf
            // 
            this._mnuRtf.MergeIndex = 3;
            this._mnuRtf.Name = "_mnuRtf";
            this._mnuRtf.Size = new System.Drawing.Size(221, 22);
            this._mnuRtf.Text = "&Rtf";
            // 
            // mnuListView
            // 
            this._mnuListView.MergeIndex = 4;
            this._mnuListView.Name = "_mnuListView";
            this._mnuListView.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this._mnuListView.Size = new System.Drawing.Size(221, 22);
            this._mnuListView.Text = "&ListView";
            this._mnuListView.Click += new System.EventHandler(this.mnuListView_Click);
            // 
            // mnuExcel
            // 
            this._mnuExcel.MergeIndex = 5;
            this._mnuExcel.Name = "_mnuExcel";
            this._mnuExcel.Size = new System.Drawing.Size(221, 22);
            this._mnuExcel.Text = "&Excel";
            this._mnuExcel.Visible = false;
            // 
            // menuResultModeFile
            // 
            this._menuResultModeFile.MergeIndex = 6;
            this._menuResultModeFile.Name = "_menuResultModeFile";
            this._menuResultModeFile.Size = new System.Drawing.Size(221, 22);
            this._menuResultModeFile.Text = "&File";
            this._menuResultModeFile.Click += new System.EventHandler(this.menuResultModeFile_Click);
            // 
            // sQLiteDatabaseToolStripMenuItem
            // 
            this._sQLiteDatabaseToolStripMenuItem.Name = "_sQLiteDatabaseToolStripMenuItem";
            this._sQLiteDatabaseToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this._sQLiteDatabaseToolStripMenuItem.Text = "SQLite database";
            this._sQLiteDatabaseToolStripMenuItem.Click += new System.EventHandler(this.sQLiteDatabaseToolStripMenuItem_Click);
            // 
            // insertScriptFileToolStripMenuItem
            // 
            this._insertScriptFileToolStripMenuItem.Name = "_insertScriptFileToolStripMenuItem";
            this._insertScriptFileToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this._insertScriptFileToolStripMenuItem.Text = "Insert Script File";
            this._insertScriptFileToolStripMenuItem.Click += new System.EventHandler(this.insertScriptFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this._toolStripSeparator3.Name = "_toolStripSeparator3";
            this._toolStripSeparator3.Size = new System.Drawing.Size(266, 6);
            // 
            // mnuGotoQueryEditor
            // 
            this._mnuGotoQueryEditor.MergeIndex = 15;
            this._mnuGotoQueryEditor.Name = "_mnuGotoQueryEditor";
            this._mnuGotoQueryEditor.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                                 | System.Windows.Forms.Keys.Q)));
            this._mnuGotoQueryEditor.Size = new System.Drawing.Size(269, 22);
            this._mnuGotoQueryEditor.Text = "Goto &Query Editor";
            this._mnuGotoQueryEditor.Click += new System.EventHandler(this.mnuGotoQueryEditor_Click);
            // 
            // mnuGotoMessageTabPage
            // 
            this._mnuGotoMessageTabPage.MergeIndex = 16;
            this._mnuGotoMessageTabPage.Name = "_mnuGotoMessageTabPage";
            this._mnuGotoMessageTabPage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this._mnuGotoMessageTabPage.Size = new System.Drawing.Size(269, 22);
            this._mnuGotoMessageTabPage.Text = "Goto &Message TabPage";
            this._mnuGotoMessageTabPage.Click += new System.EventHandler(this.mnuGotoMessageTabPage_Click);
            // 
            // mnuCloseTabPage
            // 
            this._mnuCloseTabPage.MergeIndex = 17;
            this._mnuCloseTabPage.Name = "_mnuCloseTabPage";
            //this.mnuCloseTabPage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this._mnuCloseTabPage.Size = new System.Drawing.Size(269, 22);
            this._mnuCloseTabPage.Text = "Close Current &TabPage";
            this._mnuCloseTabPage.Click += new System.EventHandler(this.mnuCloseTabPage_Click);
            // 
            // mnuCloseAllTabPages
            // 
            this._mnuCloseAllTabPages.MergeIndex = 18;
            this._mnuCloseAllTabPages.Name = "_mnuCloseAllTabPages";
            this._mnuCloseAllTabPages.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                                                                  | System.Windows.Forms.Keys.F4)));
            this._mnuCloseAllTabPages.Size = new System.Drawing.Size(269, 22);
            this._mnuCloseAllTabPages.Text = "Close &All TabPages";
            this._mnuCloseAllTabPages.Click += new System.EventHandler(this.mnuCloseAllTabPages_Click);
            // 
            // mnuCreateInsert
            // 
            this._mnuCreateInsert.MergeIndex = 19;
            this._mnuCreateInsert.Name = "_mnuCreateInsert";
            this._mnuCreateInsert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this._mnuCreateInsert.Size = new System.Drawing.Size(269, 22);
            this._mnuCreateInsert.Text = "Create insert statements";
            this._mnuCreateInsert.Click += new System.EventHandler(this.mnuCreateInsert_Click);
            // 
            // mnuCreateInsertSelect
            // 
            this._mnuCreateInsertSelect.MergeIndex = 20;
            this._mnuCreateInsertSelect.Name = "_mnuCreateInsertSelect";
            this._mnuCreateInsertSelect.Size = new System.Drawing.Size(269, 22);
            this._mnuCreateInsertSelect.Text = "Create \'insert select\' statements";
            this._mnuCreateInsertSelect.Click += new System.EventHandler(this.mnuCreateInsertSelect_Click);
            // 
            // createSqlCeDatabaseToolStripMenuItem
            // 
            this._createSqlCeDatabaseToolStripMenuItem.Name = "_createSqlCeDatabaseToolStripMenuItem";
            this._createSqlCeDatabaseToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._createSqlCeDatabaseToolStripMenuItem.Text = "Create SQL Server Compact database";
            this._createSqlCeDatabaseToolStripMenuItem.Click += new System.EventHandler(this.createSqlCeDatabaseToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this._exportToolStripMenuItem.Name = "_exportToolStripMenuItem";
            this._exportToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            // 
            // beginTransactionToolStripMenuItem
            // 
            this._beginTransactionToolStripMenuItem.Name = "_beginTransactionToolStripMenuItem";
            this._beginTransactionToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._beginTransactionToolStripMenuItem.Text = "Begin Transaction";
            this._beginTransactionToolStripMenuItem.Click += new System.EventHandler(this.beginTransactionToolStripMenuItem_Click);
            // 
            // commitTransactionToolStripMenuItem
            // 
            this._commitTransactionToolStripMenuItem.Enabled = false;
            this._commitTransactionToolStripMenuItem.Name = "_commitTransactionToolStripMenuItem";
            this._commitTransactionToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._commitTransactionToolStripMenuItem.Text = "Commit Transaction";
            this._commitTransactionToolStripMenuItem.Click += new System.EventHandler(this.commitTransactionToolStripMenuItem_Click);
            // 
            // rollbackTransactionToolStripMenuItem
            // 
            this._rollbackTransactionToolStripMenuItem.Enabled = false;
            this._rollbackTransactionToolStripMenuItem.Name = "_rollbackTransactionToolStripMenuItem";
            this._rollbackTransactionToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this._rollbackTransactionToolStripMenuItem.Text = "Rollback Transaction";
            this._rollbackTransactionToolStripMenuItem.Click += new System.EventHandler(this.rollbackTransactionToolStripMenuItem_Click);
            // 
            // menuItem3
            // 
            this._menuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._mnuObjectExplorer,
                this._mnuRefreshObjectExplorer
            });
            this._menuItem3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._menuItem3.MergeIndex = 4;
            this._menuItem3.Name = "_menuItem3";
            this._menuItem3.Size = new System.Drawing.Size(48, 20);
            this._menuItem3.Text = "&Tools";
            // 
            // mnuObjectExplorer
            // 
            this._mnuObjectExplorer.MergeIndex = 0;
            this._mnuObjectExplorer.Name = "_mnuObjectExplorer";
            this._mnuObjectExplorer.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this._mnuObjectExplorer.Size = new System.Drawing.Size(229, 22);
            this._mnuObjectExplorer.Text = "Object Explorer";
            this._mnuObjectExplorer.Click += new System.EventHandler(this.menuObjectExplorer_Click);
            // 
            // mnuRefreshObjectExplorer
            // 
            this._mnuRefreshObjectExplorer.MergeIndex = 1;
            this._mnuRefreshObjectExplorer.Name = "_mnuRefreshObjectExplorer";
            this._mnuRefreshObjectExplorer.Size = new System.Drawing.Size(229, 22);
            this._mnuRefreshObjectExplorer.Text = "Refresh Object Explorer\'s root";
            this._mnuRefreshObjectExplorer.Click += new System.EventHandler(this.mnuRefreshObjectExplorer_Click);
            // 
            // statusBar
            // 
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._sbPanelText,
                this._sbPanelTableStyle,
                this._sbPanelTimer,
                this._sbPanelRows,
                this._sbPanelCaretPosition
            });
            this._statusBar.Location = new System.Drawing.Point(300, 543);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Size = new System.Drawing.Size(716, 22);
            this._statusBar.TabIndex = 2;
            // 
            // sbPanelText
            // 
            this._sbPanelText.AutoSize = false;
            this._sbPanelText.Name = "_sbPanelText";
            this._sbPanelText.Size = new System.Drawing.Size(231, 17);
            this._sbPanelText.Spring = true;
            this._sbPanelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sbPanelTableStyle
            // 
            this._sbPanelTableStyle.AutoSize = false;
            this._sbPanelTableStyle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._sbPanelTableStyle.Name = "_sbPanelTableStyle";
            this._sbPanelTableStyle.Size = new System.Drawing.Size(100, 17);
            this._sbPanelTableStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._sbPanelTableStyle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sbPanelTableStyle_MouseUp);
            // 
            // sbPanelTimer
            // 
            this._sbPanelTimer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._sbPanelTimer.AutoSize = false;
            this._sbPanelTimer.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._sbPanelTimer.Name = "_sbPanelTimer";
            this._sbPanelTimer.Size = new System.Drawing.Size(70, 17);
            this._sbPanelTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sbPanelRows
            // 
            this._sbPanelRows.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._sbPanelRows.AutoSize = false;
            this._sbPanelRows.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._sbPanelRows.Name = "_sbPanelRows";
            this._sbPanelRows.Size = new System.Drawing.Size(200, 17);
            this._sbPanelRows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sbPanelCaretPosition
            // 
            this._sbPanelCaretPosition.AutoSize = false;
            this._sbPanelCaretPosition.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._sbPanelCaretPosition.Name = "_sbPanelCaretPosition";
            this._sbPanelCaretPosition.Size = new System.Drawing.Size(100, 17);
            this._sbPanelCaretPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tvObjectExplorer
            // 
            this._tvObjectExplorer.Dock = System.Windows.Forms.DockStyle.Left;
            this._tvObjectExplorer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                ((byte)(238)));
            this._tvObjectExplorer.Location = new System.Drawing.Point(0, 0);
            this._tvObjectExplorer.Name = "_tvObjectExplorer";
            this._tvObjectExplorer.Size = new System.Drawing.Size(300, 565);
            this._tvObjectExplorer.TabIndex = 4;
            this._tvObjectExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvObjectBrowser_BeforeExpand);
            this._tvObjectExplorer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvObjectBrowser_ItemDrag);
            this._tvObjectExplorer.DoubleClick += new System.EventHandler(this.tvObjectBrowser_DoubleClick);
            this._tvObjectExplorer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvObjectBrowser_MouseDown);
            this._tvObjectExplorer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvObjectExplorer_MouseUp);
            // 
            // splitterObjectExplorer
            // 
            this._splitterObjectExplorer.Location = new System.Drawing.Point(300, 0);
            this._splitterObjectExplorer.Name = "_splitterObjectExplorer";
            this._splitterObjectExplorer.Size = new System.Drawing.Size(3, 543);
            this._splitterObjectExplorer.TabIndex = 5;
            this._splitterObjectExplorer.TabStop = false;
            // 
            // splitterQuery
            // 
            this._splitterQuery.Dock = System.Windows.Forms.DockStyle.Top;
            this._splitterQuery.Location = new System.Drawing.Point(303, 279);
            this._splitterQuery.Name = "_splitterQuery";
            this._splitterQuery.Size = new System.Drawing.Size(713, 2);
            this._splitterQuery.TabIndex = 7;
            this._splitterQuery.TabStop = false;
            // 
            // tabControl
            // 
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(303, 281);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.ShowToolTips = true;
            this._tabControl.Size = new System.Drawing.Size(713, 262);
            this._tabControl.TabIndex = 8;
            // 
            // toolStrip
            // 
            this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._toolStripSeparator4,
                this._executeQuerySplitButton,
                this._cancelQueryButton
            });
            this._toolStrip.Location = new System.Drawing.Point(303, 281);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(73, 25);
            this._toolStrip.TabIndex = 9;
            this._toolStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator4
            // 
            this._toolStripSeparator4.Name = "_toolStripSeparator4";
            this._toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // executeQuerySplitButton
            // 
            this._executeQuerySplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._executeQuerySplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._executeQueryMenuItem,
                this._executeQuerySingleRowToolStripMenuItem,
                this._cToolStripMenuItem,
                this._openTableToolStripMenuItem
            });
            this._executeQuerySplitButton.Image = ((System.Drawing.Image)(resources.GetObject("executeQuerySplitButton.Image")));
            this._executeQuerySplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._executeQuerySplitButton.Name = "_executeQuerySplitButton";
            this._executeQuerySplitButton.Size = new System.Drawing.Size(32, 22);
            this._executeQuerySplitButton.Text = "Execute Query";
            this._executeQuerySplitButton.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // executeQueryMenuItem
            // 
            this._executeQueryMenuItem.Name = "_executeQueryMenuItem";
            this._executeQueryMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this._executeQueryMenuItem.Size = new System.Drawing.Size(168, 22);
            this._executeQueryMenuItem.Text = "Execute Query";
            this._executeQueryMenuItem.Click += new System.EventHandler(this.aToolStripMenuItem_Click);
            // 
            // executeQuerySingleRowToolStripMenuItem
            // 
            this._executeQuerySingleRowToolStripMenuItem.Name = "_executeQuerySingleRowToolStripMenuItem";
            this._executeQuerySingleRowToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this._executeQuerySingleRowToolStripMenuItem.Text = "Single Row";
            this._executeQuerySingleRowToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // cToolStripMenuItem
            // 
            this._cToolStripMenuItem.Name = "_cToolStripMenuItem";
            this._cToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this._cToolStripMenuItem.Text = "XML";
            // 
            // openTableToolStripMenuItem
            // 
            this._openTableToolStripMenuItem.Name = "_openTableToolStripMenuItem";
            this._openTableToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this._openTableToolStripMenuItem.Text = "Open Table";
            this._openTableToolStripMenuItem.Click += new System.EventHandler(this.openTableToolStripMenuItem_Click);
            // 
            // cancelQueryButton
            // 
            this._cancelQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._cancelQueryButton.Enabled = false;
            this._cancelQueryButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelQueryButton.Image")));
            this._cancelQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._cancelQueryButton.Name = "_cancelQueryButton";
            this._cancelQueryButton.Size = new System.Drawing.Size(23, 22);
            this._cancelQueryButton.Text = "Cancel Executing Query";
            this._cancelQueryButton.Click += new System.EventHandler(this.cancelExecutingQueryButton_Click);
            // 
            // queryTextBox
            // 
            this.QueryTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.QueryTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.QueryTextBox.Location = new System.Drawing.Point(303, 0);
            this.QueryTextBox.Name = "QueryTextBox";
            this.QueryTextBox.Size = new System.Drawing.Size(713, 279);
            this.QueryTextBox.TabIndex = 1;
            this.QueryTextBox.TabSize = 4;
            // 
            // QueryForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            this.ClientSize = new System.Drawing.Size(1016, 565);
            this.Controls.Add(this._toolStrip);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._splitterQuery);
            this.Controls.Add(this.QueryTextBox);
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
                    {
                        treeNode.Nodes.Add(new TreeNode());
                    }

                    parent.Add(treeNode);
                    count++;
                }
            }

            ticks = Stopwatch.GetTimestamp() - ticks;
            this._sbPanelText.Text = $"{count} item(s) added to Object Explorer in {StopwatchTimeSpan.ToString(ticks, 3)}.";
            this._sbPanelText.ForeColor = SystemColors.ControlText;
        }

        public void AddInfoMessage(InfoMessage infoMessage)
        {
            WriteInfoMessageToLog(infoMessage);

            if (infoMessage.Severity == InfoMessageSeverity.Error)
            {
                this._errorCount++;
            }

            this._infoMessages.Enqueue(infoMessage);
            this._enqueueEvent.Set();
        }

        private void AddInfoMessages(IEnumerable<InfoMessage> infoMessages)
        {
            foreach (var infoMessage in infoMessages)
            {
                WriteInfoMessageToLog(infoMessage);
            }

            var errorCount =
                (from infoMessage in infoMessages
                    where infoMessage.Severity == InfoMessageSeverity.Error
                    select infoMessage).Count();
            this._errorCount += errorCount;

            foreach (var infoMessage in infoMessages)
            {
                this._infoMessages.Enqueue(infoMessage);
            }

            this._enqueueEvent.Set();
        }

        private void AppendMessageText(
            DateTime dateTime,
            InfoMessageSeverity severity,
            string text)
        {
            var s = "[" + dateTime.ToString("HH:mm:ss.fff");

            if (severity == InfoMessageSeverity.Error)
            {
                s += ",Error";
            }

            s += "] " + text + "\r\n";
            this._messagesTextBox.AppendText(s);
        }

        private void AddTabPage(
            TabControl tabControl,
            string tabPageName,
            string tooltipText,
            Control control)
        {
            var tabPage = new TabPage(tabPageName);
            tabPage.ToolTipText = tooltipText;
            tabPage.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            tabControl.TabPages.Add(tabPage);
        }

        private void Connection_InfoMessage(IEnumerable<InfoMessage> messages)
        {
            this.AddInfoMessages(messages);
        }

        internal static string DbValue(object value)
        {
            string s;

            if (value == DBNull.Value)
            {
                s = "(null)";
            }
            else
            {
                s = value.ToString();
            }

            return s;
        }

        private string GetToolTipText(DataTable dataTable)
        {
            var sb = new StringBuilder();

            if (this._command != null)
            {
                sb.Append(this._command.CommandText + "\n");
            }

            if (dataTable != null)
            {
                sb.Append(dataTable.Rows.Count + " row(s)");
            }

            return sb.ToString();
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            var folder = Settings.CurrentType;
            this._commandTimeout = folder.Attributes["CommandTimeout"].GetValue<int>();
        }

        private void SetText()
        {
            var sb = new StringBuilder();
            sb.Append(this.Connection.ConnectionName);
            sb.Append(" - ");
            sb.Append(this.Connection.Caption);

            if (this._fileName != null)
            {
                sb.AppendFormat(" - {0}", this._fileName);
            }

            this.Text = sb.ToString();

            var mainForm = DataCommanderApplication.Instance.MainForm;
            mainForm.ActiveMdiChildToolStripTextBox.Text = sb.ToString();
        }

        private void sbPanelTableStyle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip(this.components);
                var values = Enum.GetValues(typeof (ResultWriterType));

                for (var i = 0; i < values.Length; i++)
                {
                    var tableStyle = (ResultWriterType)values.GetValue(i);
                    var item = new ToolStripMenuItem();
                    item.Text = tableStyle.ToString();
                    item.Tag = tableStyle;
                    item.Click += this.TableStyleMenuItem_Click;
                    contextMenu.Items.Add(item);
                }

                var bounds = this._sbPanelTableStyle.Bounds;
                var location = e.Location;
                contextMenu.Show(this._statusBar, bounds.X + location.X, bounds.Y + location.Y);
            }
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteQuerySingleRow();
        }

        private void ExecuteQuery()
        {
            Log.Write(LogLevel.Trace, "ExecuteQuery...");

            this.Cursor = Cursors.AppStarting;
            this.SetGui(CommandState.Cancel);

            if (this._dataAdapter != null)
            {
                Log.Write(LogLevel.Error, "this.dataAdapter == null failed");
            }

#if CONTRACTS_FULL
            Contract.Assert(this.dataAdapter == null);
#endif

            Log.Trace("ThreadMonitor:\r\n{0}", ThreadMonitor.ToStringTableString());
            ThreadMonitor.Join(0);
            Log.Write(LogLevel.Trace, GarbageMonitor.State);
            this._openTableMode = false;
            this._dataAdapter = new AsyncDataAdapter();
            this._cancel = false;

            try
            {
                this._sbPanelText.Text = "Executing query...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                var query = this.Query;
                var statements = this.Provider.GetStatements(query);
                Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
                IEnumerable<AsyncDataAdapterCommand> commands;

                if (statements.Count == 1)
                {
                    this._sqlStatement = new SqlStatement(statements[0].CommandText);
                    var command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
                    command.Transaction = this._transaction;
                    commands = new[]
                    {
                        new AsyncDataAdapterCommand
                        {
                            LineIndex = 0,
                            Command = command
                        }
                    };
                }
                else
                {
                    var transactionScope = new DbTransactionScope(this.Connection.Connection, this._transaction);
                    commands =
                        from statement in statements
                        select new AsyncDataAdapterCommand
                        {
                            LineIndex = statement.LineIndex,
                            Command = transactionScope.CreateCommand(new CommandDefinition
                            {
                                CommandText = statement.CommandText,
                                CommandTimeout = this._commandTimeout
                            })
                        };
                }

                int maxRecords;
                IResultWriter resultWriter = null;

                switch (this.TableStyle)
                {
                    case ResultWriterType.DataGrid:
                    case ResultWriterType.ListView:
                        maxRecords = int.MaxValue;
                        this._dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this._showSchemaTable);
                        resultWriter = this._dataSetResultWriter;
                        break;

                    case ResultWriterType.DataGridView:
                        maxRecords = int.MaxValue;
                        resultWriter = new DataGridViewResultWriter();
                        break;

                    case ResultWriterType.Html:
                        maxRecords = this._htmlMaxRecords;
                        this._dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this._showSchemaTable);
                        resultWriter = this._dataSetResultWriter;
                        break;

                    case ResultWriterType.Rtf:
                        maxRecords = this._wordMaxRecords;
                        this._dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this._showSchemaTable);
                        resultWriter = this._dataSetResultWriter;
                        break;

                    case ResultWriterType.File:
                        maxRecords = int.MaxValue;
                        resultWriter = new FileResultWriter(this._textBoxWriter);
                        this._tabControl.SelectedTab = this._messagesTabPage;
                        break;

                    case ResultWriterType.SqLite:
                        maxRecords = int.MaxValue;
                        var tableName = this._sqlStatement.FindTableName();
                        resultWriter = new SQLiteResultWriter(this._textBoxWriter, tableName);
                        this._tabControl.SelectedTab = this._messagesTabPage;
                        break;

                    case ResultWriterType.InsertScriptFile:
                        maxRecords = int.MaxValue;
                        tableName = this._sqlStatement.FindTableName();
                        resultWriter = new InsertScriptFileWriter(tableName, this._textBoxWriter);
                        this._tabControl.SelectedTab = this._messagesTabPage;
                        break;

                    case ResultWriterType.Excel:
                        maxRecords = int.MaxValue;
                        resultWriter = new ExcelResultWriter(this.Provider, this.AddInfoMessage);
                        this._tabControl.SelectedTab = this._messagesTabPage;
                        break;

                    case ResultWriterType.Log:
                        maxRecords = int.MaxValue;
                        resultWriter = new LogResultWriter(this.AddInfoMessage);
                        this._tabControl.SelectedTab = this._messagesTabPage;
                        break;

                    default:
                        maxRecords = int.MaxValue;
                        var textBox = new RichTextBox();
                        GarbageMonitor.Add("ExecuteQuery.textBox", textBox);
                        textBox.MaxLength = int.MaxValue;
                        textBox.Multiline = true;
                        textBox.WordWrap = false;
                        textBox.Font = this._font;
                        textBox.Dock = DockStyle.Fill;
                        textBox.ScrollBars = RichTextBoxScrollBars.Both;
                        textBox.SelectionChanged += this.textBox_SelectionChanged;

                        var resultSetTabPage = new TabPage("TextResult");
                        resultSetTabPage.Controls.Add(textBox);
                        this._resultSetsTabControl.TabPages.Add(resultSetTabPage);
                        this._resultSetsTabControl.SelectedTab = resultSetTabPage;

                        var textWriter = new TextBoxWriter(textBox);
                        resultWriter = new TextResultWriter(this.AddInfoMessage, textWriter, this);
                        break;
                }

                this._stopwatch.Start();
                this._timer.Start();
                this.ShowTimer();

                this._errorCount = 0;
                this._dataAdapter.BeginFill(this.Provider, commands, maxRecords, this._rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            catch (Exception ex)
            {
                this.EndFill(this._dataAdapter, ex);
            }
        }

        private void ShowDataTableText(DataTable dataTable)
        {
            var textBox = new RichTextBox();
            GarbageMonitor.Add("ShowDataTableText.textBox", textBox);
            textBox.MaxLength = int.MaxValue;
            textBox.Multiline = true;
            textBox.WordWrap = false;
            textBox.Font = this._font;
            textBox.ScrollBars = RichTextBoxScrollBars.Both;

            this.ShowTabPage("TextResult", this.GetToolTipText(null), textBox);

            TextWriter textWriter = new TextBoxWriter(textBox);
            var resultWriter = (IResultWriter)new TextResultWriter(this.AddInfoMessage, textWriter, this);

            resultWriter.Begin(this.Provider);

            var schemaTable = new DataTable();

            schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof (string));
            schemaTable.Columns.Add("ColumnSize", typeof (int));
            schemaTable.Columns.Add("DataType", typeof (Type));
            schemaTable.Columns.Add("NumericPrecision", typeof (short));
            schemaTable.Columns.Add("NumericScale", typeof (short));

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

                resultWriter.WriteRows(new object[][] {values}, 1);
            }

            resultWriter.WriteTableEnd();

            resultWriter.End();
        }

        private void ShowDataTableDataGrid(DataTable dataTable)
        {
            var commandBuilder = this.Provider.DbProviderFactory.CreateCommandBuilder();
            var dataTableEditor = new DataTableEditor(commandBuilder, _colorTheme);
            dataTableEditor.StatusBarPanel = this._sbPanelText;
            dataTableEditor.ReadOnly = !this._openTableMode;

            if (this._openTableMode)
            {
                var tableName = this._sqlStatement.FindTableName();
                dataTableEditor.TableName = tableName;
                var dataSet = this.Provider.GetTableSchema(this.Connection.Connection, tableName);

                foreach (DataTable schemaTable in dataSet.Tables)
                {
                    Log.Write(LogLevel.Trace, "tableSchema:\r\n{0}", schemaTable.ToStringTableString());
                }

                dataTableEditor.TableSchema = dataSet;
            }

            GarbageMonitor.Add("dataTableEditor", dataTableEditor);
            dataTableEditor.StatusBarPanel = this._sbPanelText;
            dataTableEditor.DataTable = dataTable;
            this.ShowTabPage(dataTable.TableName, this.GetToolTipText(dataTable), dataTableEditor);
        }

        private void ShowDataViewHtml(DataView dataView)
        {
            var fileName = Path.GetTempFileName();
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    var columnIndexes = new int[dataView.Table.Columns.Count];
                    for (var i = 0; i < columnIndexes.Length; i++)
                    {
                        columnIndexes[i] = i;
                    }
                    HtmlFormatter.Write(dataView, columnIndexes, streamWriter);
                }
            }

            var dataTable = dataView.Table;
            var tabPage = new TabPage(dataTable.TableName);
            tabPage.ToolTipText = this.GetToolTipText(dataTable);
            this._tabControl.TabPages.Add(tabPage);

            var htmlTextBox = new HtmlTextBox();
            htmlTextBox.Dock = DockStyle.Fill;
            tabPage.Controls.Add(htmlTextBox);

            this._tabControl.SelectedTab = tabPage;

            htmlTextBox.Navigate(fileName);

            this._sbPanelRows.Text = dataTable.Rows.Count + " row(s).";
        }

        private void ShowDataTableRtf(DataTable dataTable)
        {
            try
            {
                this._sbPanelText.Text = "Creating Word document...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;

                this._sbPanelText.Text = "Word document created.";
                this._sbPanelText.ForeColor = SystemColors.ControlText;

                var fileName = WordDocumentCreator.CreateWordDocument(dataTable);

                var richTextBox = new RichTextBox();
                GarbageMonitor.Add("ShowDataTableRtf.richTextBox", richTextBox);
                richTextBox.WordWrap = false;
                richTextBox.LoadFile(fileName);
                File.Delete(fileName);

                this.ShowTabPage(dataTable.TableName, this.GetToolTipText(dataTable), richTextBox);
            }
            catch (Exception e)
            {
                this.ShowMessage(e);
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
                {
                    type = dataColumn.DataType;
                }

                columnHeader.TextAlign = QueryFormStaticMethods.GetHorizontalAlignment(type);

                listView.Columns.Add(columnHeader);
            }

            var count = dataTable.Columns.Count;
            var items = new string[count];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (var i = 0; i < count; i++)
                {
                    var value = dataRow[i];

                    if (value == DBNull.Value)
                    {
                        items[i] = "(null)";
                    }
                    else
                    {
                        items[i] = value.ToString();
                    }
                }

                var listViewItem = new ListViewItem(items);
                listView.Items.Add(listViewItem);
            }

            this.ShowTabPage(dataTable.TableName, null, listView);
        }

        private void ShowTabPage(
            string tabPageName,
            string toolTipText,
            Control control)
        {
            control.Dock = DockStyle.Fill;
            var tabPage = new TabPage(tabPageName);
            tabPage.ToolTipText = toolTipText;
            this._tabControl.TabPages.Add(tabPage);
            tabPage.Controls.Add(control);
            this._tabControl.SelectedTab = tabPage;
            // tabPage.Refresh();
        }

        public void ShowMessage(Exception e)
        {
            var message = this.Provider.GetExceptionMessage(e);
            var infoMessage = new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Error, message);
            this.AddInfoMessage(infoMessage);

            this._tabControl.SelectedTab = this._messagesTabPage;

            this._sbPanelText.Text = "Query batch completed with errors.";
            this._sbPanelRows.Text = null;
        }

        private void TabPage_Close(object sender, EventArgs e)
        {
            var tabPage = (TabPage)sender;
            var tabControl = (TabControl)tabPage.Parent;
            tabControl.TabPages.Remove(tabPage);
        }

        private void Connection_DatabaseChanged(object sender, DatabaseChangedEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => this.Connection_DatabaseChanged(sender, args));
            }
            else
            {
                if (this._database != args.database)
                {
                    var message = $"[DatabaseChanged] Database changed from {this._database} to {this._database}";
                    var infoMessage = new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message);
                    this.AddInfoMessage(infoMessage);

                    this._database = args.database;
                    this.SetText();
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
                if (this.components != null)
                {
                    var now = LocalTime.Default.Now;
                    foreach (IComponent component in this.components.Components)
                    {
                        GarbageMonitor.SetDisposeTime(component, now);
                    }

                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void FocusControl(Control control)
        {
            control.Focus();
        }

        private void ShowResultWriterDataSet()
        {
            if (this._dataSetResultWriter != null)
            {
                var dataSet = this._dataSetResultWriter.DataSet;
                this.ShowDataSet(dataSet);
            }
        }

        private void EndFill(IAsyncDataAdapter dataAdapter, Exception e)
        {
            try
            {
                if (e != null)
                    this.ShowMessage(e);

                if (this.Connection.State == ConnectionState.Open && this.Connection.Database != this._database)
                {
                    this._database = this.Connection.Database;
                    this.SetText();
                }

                switch (this.TableStyle)
                {
                    case ResultWriterType.Text:
                    default:
                        break;

                    case ResultWriterType.DataGrid:
                    case ResultWriterType.Html:
                    case ResultWriterType.Rtf:
                    case ResultWriterType.ListView:
                        this.Invoke(new MethodInvoker(this.ShowResultWriterDataSet));
                        break;

                    case ResultWriterType.DataGridView:
                        var dataGridViewResultWriter = (DataGridViewResultWriter)dataAdapter.ResultWriter;
                        const string text = "TODO";
                        var resultSetTabPage = new TabPage(text);
                        resultSetTabPage.ToolTipText = null; // TODO
                        this._resultSetsTabControl.TabPages.Add(resultSetTabPage);
                        this._resultSetsTabControl.SelectedTab = resultSetTabPage;
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
                    if (this.Connection.State == ConnectionState.Closed)
                    {
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information,
                            "Connection is closed. Opening connection..."));

                        var csb = new SqlConnectionStringBuilder(this._connectionString);
                        csb.InitialCatalog = this._database;

                        var connectionProperties = new ConnectionProperties
                        {
                            Provider = this.Provider,
                            ConnectionString = csb.ConnectionString
                        };
                        var openConnectionForm = new OpenConnectionForm(connectionProperties);
                        if (openConnectionForm.ShowDialog() == DialogResult.OK)
                        {
                            this.Connection.Connection.Dispose();
                            this.Connection = connectionProperties.Connection;
                            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Opening connection succeeded."));
                        }
                        else
                        {
                            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Opening connection canceled."));
                        }
                    }
                }

                if (e != null || dataAdapter.TableCount == 0)
                {
                    this._tabControl.SelectedTab = this._messagesTabPage;
                }
                else
                {
                    switch (this.TableStyle)
                    {
                        case ResultWriterType.DataGrid:
                        case ResultWriterType.DataGridView:
                        case ResultWriterType.Html:
                        case ResultWriterType.ListView:
                        case ResultWriterType.Rtf:
                        case ResultWriterType.SqLite:
                        case ResultWriterType.Text:
                            this._tabControl.SelectedTab = this._resultSetsTabPage;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(() => this.EndFillHandleException(ex));
            }
        }

        private void WriteEnd(IAsyncDataAdapter dataAdapter)
        {
            this._timer.Stop();
            this.WriteRows(dataAdapter.RowCount, 3);
            this._stopwatch.Reset();

            if (this._cancel)
            {
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Query was cancelled by user."));
                this._sbPanelText.Text = "Query was cancelled by user.";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                this._cancel = false;
            }
            else
            {
                if (this._errorCount == 0)
                {
                    this._sbPanelText.ForeColor = _colorTheme != null
                        ? _colorTheme.ForeColor
                        : SystemColors.ControlText;
                    this._sbPanelText.Text = "Query executed successfully.";
                }
                else
                {
                    this._sbPanelText.ForeColor = Color.Red;
                    this._sbPanelText.Text = "Query completed with errors.";
                }
            }

            this._dataAdapter = null;
            this._dataSetResultWriter = null;

            this.SetGui(CommandState.Execute);
            this.FocusControl(this.QueryTextBox);
            this.Cursor = Cursors.Default;

            this.Invoke(() => this._mainForm.UpdateTotalMemory());
        }

        private void EndFillInvoker(IAsyncDataAdapter dataAdapter, Exception e)
        {
            this.Invoke(() => this.EndFill(dataAdapter, e));
        }

        private void WriteEndInvoker(IAsyncDataAdapter dataAdapter)
        {
            this.Invoke(() => this.WriteEnd(dataAdapter));
        }

        private void EndFillHandleException(Exception ex)
        {
            this.QueryTextBox.Focus();
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }

        private void SetGui(CommandState buttonState)
        {
            var ok = (buttonState & CommandState.Execute) != 0;
            var cancel = (buttonState & CommandState.Cancel) != 0;

            this._mnuCancel.Enabled = cancel;

            this.ButtonState = buttonState;

            this._executeQueryToolStripMenuItem.Enabled = ok;
            this._executeQueryMenuItem.Enabled = ok;
            this._mnuExecuteQuerySingleRow.Enabled = ok;
            this._mnuExecuteQuerySchemaOnly.Enabled = ok;
            this._mnuExecuteQueryKeyInfo.Enabled = ok;
            this._mnuExecuteQueryXml.Enabled = ok;

            Log.Trace("this.executeQuerySplitButton.Enabled = {0};", ok);
            this._executeQuerySplitButton.Enabled = ok;
            this._cancelQueryButton.Enabled = cancel;
        }

        private void mnuOpenTable_Click(object sender, EventArgs e)
        {
            this.OpenTable(this.Query);
        }

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
            var waitHandles = new WaitHandle[]
            {
                this._enqueueEvent,
                this._cancellationTokenSource.Token.WaitHandle
            };

            while (true)
            {
                var hasElements = false;
                while (this._infoMessages.Count > 0 && this.IsHandleCreated)
                {
                    hasElements = true;
                    var infoMessages = new InfoMessage[this._infoMessages.Count];
                    var count = this._infoMessages.Take(infoMessages);
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i < count; i++)
                    {
                        this.Invoke(() =>
                        {
                            var message = infoMessages[i];
                            var color = this._messagesTextBox.SelectionColor;

                            switch (message.Severity)
                            {
                                case InfoMessageSeverity.Error:
                                    this._messagesTextBox.SelectionColor = Color.Red;
                                    break;

                                case InfoMessageSeverity.Information:
                                    this._messagesTextBox.SelectionColor = Color.Blue;
                                    break;
                            }

                            this.AppendMessageText(message.CreationTime, message.Severity, message.Message);

                            switch (message.Severity)
                            {
                                case InfoMessageSeverity.Error:
                                case InfoMessageSeverity.Information:
                                    this._messagesTextBox.SelectionColor = color;
                                    break;
                            }
                        });
                    }
                }

                if (hasElements)
                {
                    this.Invoke(() =>
                    {
                        this._messagesTextBox.ScrollToCaret();
                        this._messagesTextBox.Update();
                    });
                }

                if (this._infoMessages.Count == 0)
                {
                    var w = WaitHandle.WaitAny(waitHandles, 1000);
                    if (w == 1)
                    {
                        break;
                    }
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
            this._sbPanelCaretPosition.Text = "Ln " + line + " Col " + col;
        }

        private void AddTable(
            OleDbConnection oleDbConnection,
            DataSet dataSet,
            Guid guid,
            string name)
        {
            try
            {
                var dataTable = oleDbConnection.GetOleDbSchemaTable(guid, null);
                dataTable.TableName = name;
                dataSet.Tables.Add(dataTable);
            }
            catch (Exception e)
            {
                this._messagesTextBox.Text += e.ToString();
            }
        }

        private void mnuDescribeParameters_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var oldDbConnection = this.Connection.Connection as OleDbConnection;

                if (oldDbConnection != null && string.IsNullOrEmpty(this.Query))
                {
                    var dataSet = new DataSet();
                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Provider_Types, "Provider Types");
                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.DbInfoLiterals, "DbInfoLiterals");

                    var c2 = new ConnectionClass();
                    c2.Open(this._connectionString, null, null, 0);
                    var rs = c2.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                    var dataTable = OleDbHelper.Convert(rs);
                    c2.Close();
                    dataSet.Tables.Add(dataTable);

                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Sql_Languages, "Sql Languages");
                    this.ShowDataSet(dataSet);
                }
                else
                {
                    this._sqlStatement = new SqlStatement(this.Query);
                    this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);

                    if (this._command != null)
                    {
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, this._command.ToLogString()));
                        var dataTable = this.Provider.GetParameterTable(this._command.Parameters);

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
                                        const long ticksPerMillisecond = 10000;
                                        const long ticksPerSecond = ticksPerMillisecond*1000;
                                        const long ticksPerMinute = ticksPerSecond*60;
                                        const long ticksPerHour = ticksPerMinute*60;
                                        const long ticksPerDay = ticksPerHour*24;

                                        var dateTime = (DateTime)value;
                                        var ticks = dateTime.Ticks;

                                        if (ticks%ticksPerDay == 0)
                                        {
                                            row["Value"] = dateTime.ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                            row["Value"] = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        }

                                        break;

                                    default:
                                        break;
                                }
                            }

                            var dataSet = new DataSet();
                            dataSet.Tables.Add(dataTable);
                            this.ShowDataSet(dataSet);
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                this.ShowMessage(ex);
            }
        }

        private void mnuCloseTabPage_Click(object sender, EventArgs e)
        {
            var tabPage = this._tabControl.SelectedTab;
            if (tabPage != null && tabPage != this._messagesTabPage && tabPage != this._resultSetsTabPage)
            {
                this.CloseResultSetTabPage(tabPage);
            }
        }

        private void CloseResultSetTabPages()
        {
            var tabPages = this._resultSetsTabControl.TabPages.Cast<TabPage>().ToArray();
            foreach (var tabPage in tabPages)
            {
                this.CloseResultSetTabPage(tabPage);
            }
            this.ResultSetCount = 0;
        }

        private void mnuCloseAllTabPages_Click(object sender, EventArgs e)
        {
            this.CloseResultSetTabPages();

            this._tabControl.SelectedTab = this._messagesTabPage;
            this._messagesTextBox.Clear();
            this._sbPanelText.Text = null;
            this._sbPanelText.ForeColor = SystemColors.ControlText;

            if (this._dataAdapter == null)
            {
                this._sbPanelRows.Text = null;
                this._sbPanelTimer.Text = null;
            }

            this.Invoke(() => this.FocusControl(this.QueryTextBox));
        }

        public void CancelCommandQuery()
        {
            Log.Trace(ThreadMonitor.ToStringTableString());
            const string message = "Cancelling command...";
            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, message));
            this._sbPanelText.Text = "Cancel Executing Command/Query...";
            this._sbPanelText.ForeColor = SystemColors.ControlText;
            this._cancel = true;
            this.SetGui(CommandState.None);
            this._dataAdapter.Cancel();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.CancelCommandQuery();
        }

        private void WriteRows(long rowCount, int scale)
        {
            var ticks = this._stopwatch.ElapsedTicks;
            this._sbPanelTimer.Text = StopwatchTimeSpan.ToString(ticks, scale);

            var text = rowCount.ToString() + " row(s).";

            if (rowCount > 0)
            {
                var seconds = (double)ticks/Stopwatch.Frequency;

                text += " (" + Math.Round(rowCount/seconds, 0) + " rows/sec)";
            }

            this._sbPanelRows.Text = text;
        }

        private void ShowTimer()
        {
            if (this._dataAdapter != null)
            {
                var rowCount = this._dataAdapter.RowCount;
                this.WriteRows(rowCount, 0);
            }
        }

        private void Timer_Tick(object o, EventArgs e)
        {
            this.Invoke(this.ShowTimer);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var text = this.QueryTextBox.RichTextBox.Text;
            if (this.Connection != null)
            {
                Log.Write(LogLevel.Trace, "Saving text before closing form(connectionName: {0}):\r\n{1}", this.Connection.ConnectionName, text);
            }

            if (this._dataAdapter == null)
            {
                bool hasTransactions;
                if (this._transaction != null)
                {
                    hasTransactions = true;
                }
                else if (this.Connection != null && this.Connection.State == ConnectionState.Open)
                {
                    try
                    {
                        hasTransactions = this.Connection.TransactionCount > 0;
                    }
                    catch (Exception ex)
                    {
                        var message = this.Provider.GetExceptionMessage(ex);
                        var color = this._messagesTextBox.SelectionColor;
                        this._messagesTextBox.SelectionColor = Color.Red;
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, message));
                        this._messagesTextBox.SelectionColor = color;
                        hasTransactions = false;
                    }
                }
                else
                {
                    hasTransactions = false;
                }

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
                    var length = this.QueryTextBox.Text.Length;

                    if (length > 0)
                    {
                        text = $"The text in {this.Text} has been changed.\r\nDo you want to save the changes?";
                        var caption = DataCommanderApplication.Instance.Name;
                        var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                        switch (result)
                        {
                            case DialogResult.Yes:
                                if (this._fileName != null)
                                {
                                    this.Save(this._fileName);
                                }
                                else
                                {
                                    this.ShowSaveFileDialog();
                                }

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
                    this.CancelCommandQuery();
                    this._timer.Enabled = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                this._cancellationTokenSource.Cancel();

                if (this.Connection != null)
                {
                    var dataSource = this.Connection.DataSource;
                    this._parentStatusBar.Items[0].Text = "Closing connection to database " + dataSource + "....";
                    this.Connection.Close();
                    this._parentStatusBar.Items[0].Text = "Connection to database " + dataSource + " closed.";
                    this.Connection.Connection.Dispose();
                    this.Connection = null;
                }

                if (this._toolStrip != null)
                {
                    this._toolStrip.Dispose();
                    this._toolStrip = null;
                }
            }
        }

        private void SetResultWriterType(ResultWriterType tableStyle)
        {
            this.TableStyle = tableStyle;
            this._sbPanelTableStyle.Text = tableStyle.ToString();
        }

        private void mnuText_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.Text);
        }

        private void mnuDataGrid_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.DataGrid);
        }

        private void mnuHtml_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.Html);
        }

        private void mnuRtf_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.Rtf);
        }

        private void mnuListView_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.ListView);
        }

        private void mnuExcel_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.Excel);
        }

        private void menuResultModeFile_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.File);
        }

        private void mnuCommandTypeText_Click(object sender, EventArgs e)
        {
            this._mnuCommandTypeText.Checked = true;
            this._mnuCommandTypeStoredProcedure.Checked = false;
            this._commandType = CommandType.Text;
        }

        private void mnuCommandTypeStoredProcedure_Click(object sender, EventArgs e)
        {
            this._mnuCommandTypeText.Checked = false;
            this._mnuCommandTypeStoredProcedure.Checked = true;
            this._commandType = CommandType.StoredProcedure;
        }

        private void menuObjectExplorer_Click(object sender, EventArgs e)
        {
            var visible = !this._tvObjectExplorer.Visible;
            this._tvObjectExplorer.Visible = visible;
            this._splitterObjectExplorer.Visible = visible;
        }

        private void tvObjectBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var treeNode = e.Node;

            if (treeNode.Nodes.Count > 0)
            {
                var treeNode2 = (ITreeNode)treeNode.Nodes[0].Tag;

                if (treeNode2 == null)
                {
                    this.Cursor = Cursors.WaitCursor;

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
                            this.ShowMessage(ex);
                        }

                        if (children != null)
                        {
                            this.AddNodes(treeNode.Nodes, children, treeNode2.Sortable);
                        }
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    var count = treeNode.GetNodeCount(false);
                    this._sbPanelText.Text = treeNode.Text + " node has " + count + " children.";
                    this._sbPanelText.ForeColor = SystemColors.ControlText;
                }
            }
        }

        private void tvObjectBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                    var treeNode = this._tvObjectExplorer.GetNodeAt(e.X, e.Y);
                    if (treeNode != null)
                    {
                        var treeNode2 = (ITreeNode)treeNode.Tag;

                        if (e.Button != MouseButtons.Left)
                        {
                            this._tvObjectExplorer.SelectedNode = treeNode;
                        }

                        var text = treeNode.Text;
                    }
                    break;

                default:
                    break;
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            var treeNodeV = this._tvObjectExplorer.SelectedNode;
            if (treeNodeV != null)
            {
                var treeNode = (ITreeNode)treeNodeV.Tag;
                treeNodeV.Nodes.Clear();
                this.AddNodes(treeNodeV.Nodes, treeNode.GetChildren(true), treeNode.Sortable);
            }
        }

        private void mnuRefreshObjectExplorer_Click(object sender, EventArgs e)
        {
            var objectExplorer = this.Provider.ObjectExplorer;
            if (objectExplorer != null)
            {
                using (new CursorManager(Cursors.WaitCursor))
                {
                    var rootNodes = this._tvObjectExplorer.Nodes;
                    rootNodes.Clear();
                    var treeNodes = objectExplorer.GetChildren(true);
                    this.AddNodes(rootNodes, treeNodes, objectExplorer.Sortable);
                }
            }
        }

        private void tvObjectExplorer_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var treeNodeV = this._tvObjectExplorer.SelectedNode;
                    if (treeNodeV != null)
                    {
                        var treeNode = (ITreeNode)treeNodeV.Tag;
                        var contextMenu = treeNode.ContextMenu;

                        if (!treeNode.IsLeaf)
                        {
                            if (contextMenu == null)
                                contextMenu = new ContextMenuStrip(this.components);

                            contextMenu.Items.Add(new ToolStripMenuItem("Refresh", null, this.mnuRefresh_Click));
                        }

                        if (contextMenu != null)
                        {
                            if (_colorTheme != null)
                            {
                                contextMenu.ForeColor = _colorTheme.ForeColor;
                                contextMenu.BackColor = _colorTheme.BackColor;
                            }

                            var contains = this.components.Components.Cast<IComponent>().Contains(contextMenu);
                            if (!contains)
                            {
                                this.components.Add(contextMenu);
                                GarbageMonitor.Add("contextMenu", contextMenu);
                            }
                            var pos = new Point(e.X, e.Y);
                            contextMenu.Show(this._tvObjectExplorer, pos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tvObjectBrowser_DoubleClick(object sender, EventArgs e)
        {
            var selectedNode = this._tvObjectExplorer.SelectedNode;
            if (selectedNode != null)
            {
                var treeNode = (ITreeNode)selectedNode.Tag;

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var query = treeNode.Query;
                    if (query != null)
                    {
                        var text0 = this.QueryTextBox.Text;
                        string append = null;
                        var selectionStart = this.QueryTextBox.RichTextBox.TextLength;

                        if (!string.IsNullOrEmpty(text0))
                        {
                            append = Environment.NewLine + Environment.NewLine;
                            selectionStart += 2;
                        }

                        append += query;

                        this.QueryTextBox.RichTextBox.AppendText(append);
                        this.QueryTextBox.RichTextBox.SelectionStart = selectionStart;
                        this.QueryTextBox.RichTextBox.SelectionLength = query.Length;

                        this.QueryTextBox.Focus();
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            this.QueryTextBox.Paste();
        }

        private void mnuGoTo_Click(object sender, EventArgs e)
        {
            var control = this.ActiveControl;
            var richTextBox = control as RichTextBox;
            if (richTextBox == null)
            {
                richTextBox = this.QueryTextBox.RichTextBox;
            }

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
            {
                found = parent;
            }
            else
            {
                foreach (TreeNode child in parent.Nodes)
                {
                    found = this.FindTreeNode(child, matcher);

                    if (found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }

        private void FindText(string text)
        {
            var found = false;
            var control = this.ActiveControl;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this._sbPanelText.Text = $"Finding {text}...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                StringComparison comparison;
                var options = this._findTextForm.RichTextBoxFinds;
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
                    {
                        treeNode2 = treeView.SelectedNode.NextNode;
                    }

                    var treeNode = treeNode2;

                    while (treeNode != null)
                    {
                        treeNode2 = this.FindTreeNode(treeNode, matcher);

                        if (treeNode2 != null)
                        {
                            treeView.SelectedNode = treeNode2;
                            found = true;
                            break;
                        }
                        else
                        {
                            treeNode = treeNode.NextNode;
                        }
                    }
                }
                else
                {
                    var dataTableViewer = control as DataTableEditor;

                    if (dataTableViewer == null)
                    {
                        dataTableViewer = control.Parent as DataTableEditor;
                    }

                    if (dataTableViewer != null)
                    {
                        var dataTable = dataTableViewer.DataTable;

                        if (dataTable != null)
                        {
                            if (text.StartsWith("RowFilter="))
                            {
                                var rowFilter = text.Substring(5);
                                var dataView = dataTable.DefaultView;
                                dataView.RowFilter = rowFilter;
                                var count = dataView.Count;
                                found = count > 0;
                                this._sbPanelText.Text = $"{count} rows found. RowFilter: {rowFilter}";
                                this._sbPanelText.ForeColor = SystemColors.ControlText;
                            }
                            else if (text.StartsWith("Sort="))
                            {
                                var sort = text.Substring(5);
                                var dataView = dataTable.DefaultView;
                                dataView.Sort = sort;
                                this._sbPanelText.Text = $"Rows sorted by {sort}.";
                                this._sbPanelText.ForeColor = SystemColors.ControlText;
                            }
                            else
                            {
                                var dataGrid = dataTableViewer.DataGrid;
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
                        {
                            found = false;
                        }
                    }
                    else
                    {
                        var richTextBox = control as RichTextBox;

                        if (richTextBox == null)
                        {
                            richTextBox = this.QueryTextBox.RichTextBox;
                        }

                        var start = richTextBox.SelectionStart + richTextBox.SelectionLength;
                        var location = richTextBox.Find(text, start, options);
                        found = location >= 0;
                    }
                }
            }
            finally
            {
                this._sbPanelText.Text = null;
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                this.Cursor = Cursors.Default;
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
                if (this._findTextForm == null)
                {
                    this._findTextForm = new FindTextForm();
                }

                var control = this.ActiveControl;
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
                    this._findTextForm.Text = $"Find (DataTable: {name})";
                }
                else
                {
                    this._findTextForm.Text = "Find";
                }

                if (this._findTextForm.ShowDialog() == DialogResult.OK)
                {
                    this.FindText(this._findTextForm.FindText);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex);
            }
        }

        private void mnuFindNext_Click(object sender, EventArgs e)
        {
            if (this._findTextForm != null)
            {
                var text = this._findTextForm.FindText;

                if (text != null)
                {
                    this.FindText(text);
                }
            }
        }

        private void Save(string fileName)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                this._sbPanelText.Text = $"Saving file {fileName}...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;

                const RichTextBoxStreamType type = RichTextBoxStreamType.UnicodePlainText;
                var encoding = Encoding.Unicode;

                using (var stream = File.Create(fileName))
                {
                    var preamble = encoding.GetPreamble();
                    stream.Write(preamble, 0, preamble.Length);
                    this.QueryTextBox.RichTextBox.SaveFile(stream, type);
                }

                this._fileName = fileName;
                this.SetText();
                this._sbPanelText.Text = $"File {fileName} saved successfully.";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
            }
            finally
            {
                this.Cursor = Cursors.Default;
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
                this.Save(fileName);
            }
        }

        public void Save()
        {
            if (this._fileName != null)
            {
                this.Save(this._fileName);
            }
            else
            {
                this.ShowSaveFileDialog();
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            this.ShowSaveFileDialog();
        }

        private void mnuGotoQueryEditor_Click(object sender, EventArgs e)
        {
            this.QueryTextBox.Select();
        }

        private void mnuGotoMessageTabPage_Click(object sender, EventArgs e)
        {
            this._tabControl.SelectedTab = this._messagesTabPage;
        }

        private GetCompletionResponse GetCompletion()
        {
            var textBox = this.QueryTextBox.RichTextBox;
            var text = textBox.Text;
            var position = textBox.SelectionStart;

            var ticks = Stopwatch.GetTimestamp();
            var response = this.Provider.GetCompletion(this.Connection, this._transaction, text, position);
            var from = response.FromCache ? "cache" : "data source";
            ticks = Stopwatch.GetTimestamp() - ticks;
            var length = response.Items != null ? response.Items.Count : 0;
            this._sbPanelText.Text = $"GetCompletion returned {length} items from {@from} in {StopwatchTimeSpan.ToString(ticks, 3)} seconds.";
            this._sbPanelText.ForeColor = SystemColors.ControlText;
            return response;
        }

        private void mnuListMembers_Click(object sender, EventArgs e)
        {
            if (this.QueryTextBox.KeyboardHandler == null)
            {
                using (new CursorManager(Cursors.WaitCursor))
                {
                    var response = this.GetCompletion();
                    if (response.Items != null)
                    {
                        var completionForm = new CompletionForm(this);
                        completionForm.Initialize(this.QueryTextBox, response, _colorTheme);
                        completionForm.ItemSelected += new EventHandler<ItemSelectedEventArgs>(this.completionForm_ItemSelected);
                        completionForm.Show(this);
                        this.QueryTextBox.RichTextBox.Focus();
                    }
                }
            }
        }

        private void completionForm_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            var textBox = this.QueryTextBox;

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
            this.Provider.ClearCompletionCache();
        }

        private async void ExecuteReader(CommandBehavior commandBehavior)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this._sqlStatement = new SqlStatement(this.Query);
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);

                if (this._command != null)
                {
                    IDataReader dataReader = null;

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                dataReader = this._command.ExecuteReader(commandBehavior);
                                break;
                            }
                            catch
                            {
                                if (this.Connection.State != ConnectionState.Open)
                                {
                                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Opening connection..."));
                                    await this.Connection.OpenAsync(CancellationToken.None);
                                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information,
                                        "Connection opened successfully."));
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        DataSet dataSet = null;
                        var i = 1;

                        do
                        {
                            var dataTable = this.Provider.GetSchemaTable(dataReader);

                            if (dataTable != null)
                            {
                                if (dataSet == null)
                                {
                                    dataSet = new DataSet();
                                }

                                dataTable.TableName = "SchemaTable" + i;
                                dataSet.Tables.Add(dataTable);
                                i++;
                            }
                        } while (dataReader.NextResult());

                        this.ShowDataSet(dataSet);
                        this._tabControl.SelectedTab = this._resultSetsTabPage;
                    }
                    finally
                    {
                        if (dataReader != null)
                        {
                            dataReader.Close();
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                this.ShowMessage(ex);
            }
        }

        private void mnuResultSchema_Click(object sender, EventArgs e)
        {
            this.ExecuteReader(CommandBehavior.SchemaOnly);
        }

        private void mnuKeyInfo_Click(object sender, EventArgs e)
        {
            this.ExecuteReader(CommandBehavior.KeyInfo);
        }

        private void ExecuteQuerySingleRow()
        {
            try
            {
                this._sqlStatement = new SqlStatement(this.Query);
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
                var dataSet = new DataSet();
                using (var dataReader = this._command.ExecuteReader())
                {
                    do
                    {
                        var schemaTable = this.Provider.GetSchemaTable(dataReader);
                        var dataReaderHelper = this.Provider.CreateDataReaderHelper(dataReader);
                        var rowCount = 0;

                        while (dataReader.Read())
                        {
                            rowCount++;
                            var values = new object[dataReader.FieldCount];
                            dataReaderHelper.GetValues(values);

                            var dataTable = new DataTable("SingleRow(" + rowCount + ")");
                            dataTable.Columns.Add(" ", typeof (int));
                            dataTable.Columns.Add("Name", typeof (string));
                            dataTable.Columns.Add("Value");
                            var count = schemaTable.Rows.Count;

                            for (var i = 0; i < count; i++)
                            {
                                var schemaRow = schemaTable.Rows[i];
                                var columnName = schemaRow["Name"].ToString();

                                var dataRow = dataTable.NewRow();
                                dataRow[0] = i + 1;
                                dataRow[1] = columnName;
                                dataRow[2] = values[i];
                                dataTable.Rows.Add(dataRow);
                            }

                            //ShowDataTable(dataTable, tableStyle);
                            dataSet.Tables.Add(dataTable);

                            if (rowCount == 20)
                            {
                                break;
                            }
                        }
                    } while (dataReader.NextResult());

                    dataReader.Close();
                }

                this.ShowDataSet(dataSet);
                this._tabControl.SelectedTab = this._resultSetsTabPage;
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex);
            }
        }

        private void mnuSingleRow_Click(object sender, EventArgs e)
        {
            this.ExecuteQuerySingleRow();
        }

        private void mnuShowShemaTable_Click(object sender, EventArgs e)
        {
            this._mnuShowShemaTable.Checked = !this._mnuShowShemaTable.Checked;
            this._showSchemaTable = !this._showSchemaTable;
        }

        private void mnuXml_Click(object sender, EventArgs e)
        {
            try
            {
                this._sqlStatement = new SqlStatement(this.Query);
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);

                using (var dataReader = this._command.ExecuteReader())
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
                            this._resultSetsTabControl.TabPages.Add(resultSetTabPage);

                            var htmlTextBox = new HtmlTextBox();
                            htmlTextBox.Dock = DockStyle.Fill;

                            resultSetTabPage.Controls.Add(htmlTextBox);

                            htmlTextBox.Navigate(path);
                            this._resultSetsTabControl.SelectedTab = resultSetTabPage;
                            this._tabControl.SelectedTab = this._resultSetsTabPage;
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
                this.ShowMessage(ex);
            }
        }

        private void mnuCreateInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var textWriter = this._standardOutput.TextWriter;

                this._sqlStatement = new SqlStatement(this.Query);
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, CommandType.Text, this._commandTimeout);
                var tableName = this._sqlStatement.FindTableName();
                var tableIndex = 0;

                using (var dataReader = this._command.ExecuteReader())
                {
                    while (true)
                    {
                        if (tableIndex > 0)
                        {
                            tableName = $"Table{tableIndex}";
                        }

                        var dataReaderHelper = this.Provider.CreateDataReaderHelper(dataReader);
                        var schemaTable = dataReader.GetSchemaTable();
                        var sb = new StringBuilder();

                        if (schemaTable != null)
                        {
                            if (tableName != null)
                            {
                                schemaTable.TableName = tableName;
                            }
                            else
                            {
                                tableName = schemaTable.TableName;
                            }

                            this._standardOutput.WriteLine(InsertScriptFileWriter.GetCreateTableStatement(schemaTable));
                            var schemaRows = schemaTable.Rows;
                            var columnCount = schemaRows.Count;
                            sb.AppendFormat("insert into {0}(", tableName);

                            for (var i = 0; i < columnCount; i++)
                            {
                                if (i > 0)
                                {
                                    sb.Append(',');
                                }

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
                                {
                                    sb.Append(',');
                                }

                                var s = InsertScriptFileWriter.ToString(values[i]);
                                sb.Append(s);
                            }

                            sb.AppendLine(");");
                            statementCount++;

                            if (statementCount%100 == 0)
                            {
                                this._standardOutput.Write(sb);
                                sb.Length = 0;
                            }
                        }

                        if (statementCount%100 != 0)
                        {
                            this._standardOutput.Write(sb);
                        }

                        if (!dataReader.NextResult())
                        {
                            break;
                        }

                        tableIndex++;
                    }
                }

                this._tabControl.SelectedTab = this._messagesTabPage;

            }
            catch (Exception ex)
            {
                this.ShowMessage(ex);
            }
        }

        private void mnuCreateInsertSelect_Click(object sender, EventArgs e)
        {
            try
            {
                this._sqlStatement = new SqlStatement(this.Query);
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, CommandType.Text, this._commandTimeout);
                var tableName = this._sqlStatement.FindTableName();

                if (tableName != null)
                {
                    using (var dataReader = this._command.ExecuteReader())
                    {
                        var dataReaderHelper = this.Provider.CreateDataReaderHelper(dataReader);
                        var schemaTable = dataReader.GetSchemaTable();
                        var schemaRows = schemaTable.Rows;
                        var columnCount = schemaRows.Count;
                        var sb = new StringBuilder();
                        sb.AppendFormat("insert into {0}(", tableName);

                        for (var i = 0; i < columnCount; i++)
                        {
                            if (i > 0)
                            {
                                sb.Append(',');
                            }

                            var schemaRow = schemaRows[i];
                            var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                            sb.Append(columnName);
                        }

                        sb.Append(")\r\nselect");
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
                                sb.Append('\t');

                                if (i == 0)
                                {
                                    sb.Append(' ');
                                }
                                {
                                    sb.Append(',');
                                }

                                var s = InsertScriptFileWriter.ToString(values[i]);
                                sb.AppendFormat("{0}\t\tas {1}", s, dataReader.GetName(i));
                            }

                            this._standardOutput.WriteLine(sb);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex);
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

            this.QueryTextBox.Text = text;
            this._fileName = path;
            this.SetText();
            this._sbPanelText.Text = $"File {this._fileName} loaded successfully.";
            this._sbPanelText.ForeColor = SystemColors.ControlText;
        }

        private void tvObjectBrowser_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var treeNode = (TreeNode)e.Item;
            var treeNode2 = (ITreeNode)treeNode.Tag;
            var text = treeNode.Text;
            this._tvObjectExplorer.DoDragDrop(text, DragDropEffects.All);
        }

        private async void mnuDuplicateConnection_Click(object sender, EventArgs e)
        {
            var mainForm = DataCommanderApplication.Instance.MainForm;
            var index = mainForm.MdiChildren.Length;

            var connection = this.Provider.CreateConnection(this._connectionString);
            connection.ConnectionName = this.Connection.ConnectionName;
            await connection.OpenAsync(CancellationToken.None);
            var database = this.Connection.Database;

            if (connection.Database != this.Connection.Database)
            {
                connection.Connection.ChangeDatabase(database);
            }

            var queryForm = new QueryForm(this._mainForm, index, this.Provider, this._connectionString, connection, mainForm.StatusBar, _colorTheme);
            queryForm.Font = mainForm.SelectedFont;
            queryForm.MdiParent = mainForm;
            queryForm.WindowState = this.WindowState;
            queryForm.Show();
        }

        public void OpenTable(string query)
        {
            try
            {
                Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
                this._sqlStatement = new SqlStatement(query);
                this._commandType = CommandType.Text;
                this._openTableMode = true;
                this._command = this._sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
                this._dataAdapter = new AsyncDataAdapter();
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Executing query..."));
                this._stopwatch.Start();
                this._timer.Start();
                const int maxRecords = int.MaxValue;
                this._dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this._showSchemaTable);
                IResultWriter resultWriter = this._dataSetResultWriter;
                this._dataAdapter.BeginFill(
                    this.Provider,
                    new[]
                    {
                        new AsyncDataAdapterCommand
                        {
                            LineIndex = 0,
                            Command = this._command
                        }
                    },
                    maxRecords, this._rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            catch (Exception ex)
            {
                this.EndFill(this._dataAdapter, ex);
            }
        }

        private void sQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.SqLite);
        }

        private void createSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sqlStatement = new SqlStatement(this.Query);
            this._command = sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
            IAsyncDataAdapter asyncDataAdatper = new AsyncDataAdapter();
            var maxRecords = int.MaxValue;
            var tableName = sqlStatement.FindTableName();
            var sqlCeResultWriter = new SqlCeResultWriter(this._textBoxWriter, tableName);
            asyncDataAdatper.BeginFill(
                this.Provider,
                new[]
                {
                    new AsyncDataAdapterCommand
                    {
                        LineIndex = 0,
                        Command = this._command
                    }
                },
                maxRecords, this._rowBlockSize, sqlCeResultWriter, this.EndFillInvoker, this.WriteEndInvoker);
        }

        private void SetTransaction(IDbTransaction transaction)
        {
            if (this._transaction == null && transaction != null)
            {
                this._transaction = transaction;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction created successfully."));
                this._tabControl.SelectedTab = this._messagesTabPage;
                this._beginTransactionToolStripMenuItem.Enabled = false;
                this._commitTransactionToolStripMenuItem.Enabled = true;
                this._rollbackTransactionToolStripMenuItem.Enabled = true;
            }
        }

        private void InvokeSetTransaction(IDbTransaction transaction)
        {
            this.Invoke(new Action<IDbTransaction>(this.SetTransaction), transaction);
        }

        private void beginTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._transaction == null)
            {
                var transaction = this.Connection.Connection.BeginTransaction();
                this.SetTransaction(transaction);
            }
        }

        private void commitTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._transaction != null)
            {
                this._transaction.Commit();
                this._transaction.Dispose();
                this._transaction = null;

                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction commited successfully."));

                this._tabControl.SelectedTab = this._messagesTabPage;
                this._beginTransactionToolStripMenuItem.Enabled = true;
                this._commitTransactionToolStripMenuItem.Enabled = false;
                this._rollbackTransactionToolStripMenuItem.Enabled = false;
            }
        }

        private void rollbackTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._transaction != null)
            {
                try
                {
                    this._transaction.Rollback();
                    this._transaction.Dispose();
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction rolled back successfully."));
                }
                catch (Exception ex)
                {
                    var message = $"Rollback failed. Exception:\r\n{ex.ToLogString()}";
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Error, message));
                }

                this._transaction = null;
                this._tabControl.SelectedTab = this._messagesTabPage;
                this._beginTransactionToolStripMenuItem.Enabled = true;
                this._commitTransactionToolStripMenuItem.Enabled = false;
                this._rollbackTransactionToolStripMenuItem.Enabled = false;
            }
        }

        internal void ScriptQueryAsCreateTable()
        {
            var sqlStatement = new SqlStatement(this.Query);
            var command = sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);

            var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
            var index = Array.IndexOf(forms, this);
            IProvider destinationProvider;

            if (index < forms.Length - 1)
            {
                var nextQueryForm = (QueryForm)forms[index + 1];
                destinationProvider = nextQueryForm.Provider;
            }
            else
            {
                destinationProvider = this.Provider;
            }

            DataTable schemaTable;
            string[] dataTypeNames;

            using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                schemaTable = dataReader.GetSchemaTable();
                dataTypeNames = new string[dataReader.FieldCount];

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    dataTypeNames[i] = dataReader.GetDataTypeName(i);
                }
            }
            string tableName;
            if (command.CommandType == CommandType.StoredProcedure)
            {
                tableName = command.CommandText;
            }
            else
            {
                tableName = sqlStatement.FindTableName();
            }
            var createTable = new StringBuilder();
            createTable.AppendFormat("create table [{0}]\r\n(\r\n", tableName);
            var stringTable = new StringTable(3);
            var last = schemaTable.Rows.Count - 1;

            for (var i = 0; i <= last; i++)
            {
                var dataRow = schemaTable.Rows[i];
                var schemaRow = new DbColumn(dataRow);
                var row = stringTable.NewRow();
                var typeName = destinationProvider.GetColumnTypeName(this.Provider, dataRow, dataTypeNames[i]);
                row[1] = schemaRow.ColumnName;
                row[2] = typeName;
                var allowDbNull = schemaRow.AllowDbNull;

                if (allowDbNull == false)
                {
                    row[2] += " not null";
                }

                if (i < last)
                {
                    row[2] += ',';
                }

                stringTable.Rows.Add(row);
            }



            createTable.Append(stringTable.ToString(4));

            createTable.Append(')');
            var commandText = createTable.ToString();

            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "\r\n" + commandText));
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
                var sqlStatement = new SqlStatement(this.Query);
                this._command = sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
                string tableName;
                if (this._command.CommandType == CommandType.StoredProcedure)
                {
                    tableName = this._command.CommandText;
                }
                else
                {
                    tableName = sqlStatement.FindTableName();
                }

                if (tableName[0] == '[' && destinationProvider.Name == "System.Data.OracleClient")
                {
                    tableName = tableName.Substring(1, tableName.Length - 2);
                }

                IResultWriter resultWriter = new CopyResultWriter(this.AddInfoMessage, destinationProvider, destinationConnection, tableName,
                    nextQueryForm.InvokeSetTransaction, CancellationToken.None);
                var maxRecords = int.MaxValue;
                var rowBlockSize = 10000;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, "Copying table..."));
                this._sbPanelText.Text = "Copying table...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                this.SetGui(CommandState.Cancel);
                this._errorCount = 0;
                this._stopwatch.Start();
                this._timer.Start();
                this._dataAdapter = new AsyncDataAdapter();
                this._dataAdapter.BeginFill(
                    this.Provider,
                    new[]
                    {
                        new AsyncDataAdapterCommand
                        {
                            LineIndex = 0,
                            Command = this._command
                        }
                    },
                    maxRecords, rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            else
            {
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Please open a destination connection."));
            }
        }

        internal void CopyTableWithSqlBulkCopy()
        {
            var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
            var index = Array.IndexOf(forms, this);
            if (index < forms.Length - 1)
            {
                var nextQueryForm = (QueryForm)forms[index + 1];
                var destinationProvider = nextQueryForm.Provider;
                var destinationConnection = (SqlConnection)nextQueryForm.Connection.Connection;
                var destionationTransaction = (SqlTransaction)nextQueryForm._transaction;
                var sqlStatement = new SqlStatement(this.Query);
                this._command = sqlStatement.CreateCommand(this.Provider, this.Connection, this._commandType, this._commandTimeout);
                string tableName;
                if (this._command.CommandType == CommandType.StoredProcedure)
                {
                    tableName = this._command.CommandText;
                }
                else
                {
                    tableName = sqlStatement.FindTableName();
                }

                //IResultWriter resultWriter = new SqlBulkCopyResultWriter( this.AddInfoMessage, destinationProvider, destinationConnection, tableName, nextQueryForm.InvokeSetTransaction );
                var maxRecords = int.MaxValue;
                var rowBlockSize = 10000;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, "Copying table..."));
                this._sbPanelText.Text = "Copying table...";
                this._sbPanelText.ForeColor = SystemColors.ControlText;
                this.SetGui(CommandState.Cancel);
                this._errorCount = 0;
                this._stopwatch.Start();
                this._timer.Start();
                this._dataAdapter = new SqlBulkCopyAsyncDataAdapter(destinationConnection, destionationTransaction, tableName, this.AddInfoMessage);
                this._dataAdapter.BeginFill(
                    this.Provider,
                    new[]
                    {
                        new AsyncDataAdapterCommand
                        {
                            LineIndex = 0,
                            Command = this._command
                        }
                    },
                    maxRecords, rowBlockSize, null, this.EndFillInvoker, this.WriteEndInvoker);
            }
            else
            {
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Please open a destination connection."));
            }
        }

        private void insertScriptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.InsertScriptFile);
        }

        private void TableStyleMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var tableStyle = (ResultWriterType)item.Tag;
            this.SetResultWriterType(tableStyle);
        }

        public static void ShowText(string text)
        {
            var mainForm = DataCommanderApplication.Instance.MainForm;
            mainForm.Cursor = Cursors.WaitCursor;
            var queryForm = (QueryForm)mainForm.ActiveMdiChild;

            try
            {
                var queryTextBox = queryForm.QueryTextBox;
                var selectionStart = queryTextBox.RichTextBox.TextLength;
                var append = text;
                queryTextBox.RichTextBox.AppendText(append);
                queryTextBox.RichTextBox.SelectionStart = selectionStart;
                queryTextBox.RichTextBox.SelectionLength = append.Length;

                queryTextBox.Focus();
            }
            catch (Exception e)
            {
                queryForm.ShowMessage(e);
            }
            finally
            {
                mainForm.Cursor = Cursors.Default;
            }
        }

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
                this.pt = new Point(x, y);
                this.flags = Tchittestflags.TchtOnitem;
            }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref Tchittestinfo lParam);

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            this.ExecuteQuery();
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteQuery();
        }

        private void cancelExecutingQueryButton_Click(object sender, EventArgs e)
        {
            this.CancelCommandQuery();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ExecuteQuery();
        }

        private void openTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTable(this.Query);
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var transactionScope = new DbTransactionScope(this.Connection.Connection, null);
            var on = false;
            try
            {
                transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = "SET PARSEONLY ON"});
                on = true;
                var query = this.Query;
                bool succeeded;

                try
                {
                    transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = query});
                    succeeded = this._infoMessages.Count == 0;
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    var infoMessages = this.Provider.ToInfoMessages(exception);
                    this.AddInfoMessages(infoMessages);
                }

                if (succeeded)
                {
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Command(s) completed successfully."));
                }
            }
            catch (Exception exception)
            {
                var infoMessages = this.Provider.ToInfoMessages(exception);
                this.AddInfoMessages(infoMessages);
            }

            if (on)
            {
                transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = "SET PARSEONLY OFF"});
            }
        }

        public void SetStatusbarPanelText(string text, Color color)
        {
            this._sbPanelText.Text = text;
            this._sbPanelText.ForeColor = color;
            this.Refresh();
        }

#endregion
    }
}