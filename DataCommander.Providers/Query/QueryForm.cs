namespace DataCommander
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using ADODB;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Text;
    using DataCommander.Foundation.Threading;
    using DataCommander.Foundation.Windows.Forms;
    using DataCommander.Providers;
    using DataCommander.Providers.Query;
    using DataCommander.Providers.ResultWriter;
    using Microsoft.Office.Interop.Word;
    using Application = DataCommander.Providers.Application;
    using DataTable = System.Data.DataTable;
    using Font = System.Drawing.Font;
    using Point = System.Drawing.Point;
    using Task = System.Threading.Tasks.Task;
    using Timer = System.Windows.Forms.Timer;
    using View = System.Windows.Forms.View;

    /// <summary>
    /// Summary description for QueryForm.
    /// </summary>
    public sealed class QueryForm : Form
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly NumberFormatInfo numberFormatInfo;

        private MenuStrip mainMenu;
        private ToolStripMenuItem menuItem1;
        private StatusStrip statusBar;
        private ToolStripMenuItem mnuDescribeParameters;
        private ToolStripMenuItem mnuCloseTabPage;
        private ToolStripMenuItem mnuCancel;
        private ToolStripStatusLabel sbPanelText;
        private ToolStripStatusLabel sbPanelTimer;
        private ToolStripStatusLabel sbPanelRows;
        private ToolStripMenuItem menuItem2;
        private ToolStripMenuItem mnuDataGrid;
        private ToolStripMenuItem mnuHtml;
        private ToolStripMenuItem mnuRtf;
        private ToolStripMenuItem menuItem7;
        private ToolStripMenuItem mnuCommandTypeText;
        private ToolStripMenuItem mnuCommandTypeStoredProcedure;
        private ToolStripMenuItem mnuText;
        private TreeView tvObjectExplorer;
        private Splitter splitterObjectExplorer;
        private ToolStripMenuItem menuItem3;
        private QueryTextBox queryTextBox;
        private Splitter splitterQuery;
        private TabControl tabControl;
        private ToolStripMenuItem menuItem8;
        private ToolStripMenuItem mnuPaste;
        private ToolStripStatusLabel sbPanelCaretPosition;
        private ToolStripMenuItem mnuFind;
        private ToolStripMenuItem mnuFindNext;
        private ToolStripMenuItem mnuObjectExplorer;
        private ToolStripMenuItem menuItem9;
        private ToolStripMenuItem mnuSaveAs;
        private ToolStripMenuItem mnuExcel;
        private ToolStripMenuItem mnuIntelliSense;
        private ToolStripMenuItem mnuListMembers;
        private ToolStripMenuItem mnuGotoQueryEditor;
        private ToolStripMenuItem mnuCloseAllTabPages;
        private ToolStripMenuItem mnuGotoMessageTabPage;
        private ToolStripMenuItem mnuClearCache;
        private ToolStripMenuItem mnuListView;
        private ToolStripMenuItem mnuExecuteQueryKeyInfo;
        private ToolStripMenuItem mnuExecuteQuerySchemaOnly;
        private ToolStripMenuItem mnuExecuteQuerySingleRow;
        private ToolStripMenuItem mnuShowShemaTable;
        private ToolStripMenuItem mnuExecuteQueryXml;
        private ToolStripMenuItem mnuSave;
        private ToolStripMenuItem mnuCreateInsert;
        private ToolStripMenuItem menuResultModeFile;
        private ToolStripStatusLabel sbPanelTableStyle;
        private ToolStripMenuItem mnuGoTo;
        private ToolStripMenuItem mnuDuplicateConnection;
        private ToolStripMenuItem mnuCreateInsertSelect;
        private ToolStripMenuItem mnuOpenTable;
        private readonly IContainer components = new Container();
        private readonly IProvider provider;
        private readonly string connectionString;
        private ConnectionBase connection;
        private IDbTransaction transaction;
        private SqlStatement sqlStatement;
        private IDbCommand command;
        private CommandType commandType = CommandType.Text;
        private IAsyncDataAdapter dataAdapter;
        private bool cancel;
        private readonly Timer timer = new Timer();
        private readonly Stopwatch stopwatch = new Stopwatch();
        private ResultWriterType resultWriterType;
        private readonly int htmlMaxRecords;
        private readonly int wordMaxRecords;
        private DataSetResultWriter dataSetResultWriter;
        private bool showSchemaTable;
        private readonly StatusStrip parentStatusBar;
        private readonly TextBoxWriter textBoxWriter;
        private FindTextForm findTextForm;
        private readonly int rowBlockSize;
        private readonly StandardOutput standardOutput;
        private string database;
        private CommandState buttonState;
        private string fileName;
        private int commandTimeout;
        private Font font;
        private ToolStripMenuItem mnuRefreshObjectExplorer;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem sQLiteDatabaseToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem commitTransactionToolStripMenuItem;
        private ToolStripMenuItem beginTransactionToolStripMenuItem;
        private ToolStripMenuItem rollbackTransactionToolStripMenuItem;
        private ToolStripMenuItem createSqlCeDatabaseToolStripMenuItem;
        private ToolStripMenuItem insertScriptFileToolStripMenuItem;
        private bool openTableMode;

        private readonly TabPage messagesTabPage;
        private readonly RichTextBox messagesTextBox;

        private readonly TabPage resultSetsTabPage;
        private readonly TabControl resultSetsTabControl;
        private ToolStrip toolStrip;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSplitButton execueQuerySplitButton;
        private ToolStripMenuItem executeQueryMenuItem;
        private ToolStripMenuItem executeQuerySingleRowToolStripMenuItem;
        private ToolStripMenuItem cToolStripMenuItem;
        private ToolStripButton cancelQueryButton;
        private ToolStripMenuItem executeQueryToolStripMenuItem;
        private ToolStripMenuItem openTableToolStripMenuItem;
        private ToolStripMenuItem parseToolStripMenuItem;
        private int resultSetCount;

        private readonly ConcurrentQueue<InfoMessage> infoMessages = new ConcurrentQueue<InfoMessage>();
        private int errorCount;
        private readonly LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly EventWaitHandle enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);

        #endregion

        #region Constructors

        static QueryForm()
        {
            numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
        }

        public QueryForm(
            int index,
            IProvider provider,
            string connectionString,
            ConnectionBase connection,
            StatusStrip parentStatusBar)
        {
            GarbageMonitor.Add("QueryForm", this);
            this.provider = provider;
            this.connectionString = connectionString;
            this.connection = connection;
            this.parentStatusBar = parentStatusBar;
            connection.InfoMessage += this.Connection_InfoMessage;
            connection.DatabaseChanged += this.Connection_DatabaseChanged;
            this.timer.Tick += this.Timer_Tick;

            var task = new Task(this.ConsumeInfoMessages);
            task.Start(this.scheduler);

            this.messagesTextBox = new RichTextBox();
            this.components.Add(this.messagesTextBox);
            GarbageMonitor.Add("QueryForm.messagesTextBox", this.messagesTextBox);
            this.messagesTextBox.Multiline = true;
            this.messagesTextBox.WordWrap = false;
            this.messagesTextBox.Dock = DockStyle.Fill;
            this.messagesTextBox.ScrollBars = RichTextBoxScrollBars.Both;

            this.messagesTabPage = new TabPage("Messages");
            this.messagesTabPage.Controls.Add(this.messagesTextBox);

            this.InitializeComponent();
            GarbageMonitor.Add("queryForm.toolStrip", this.toolStrip);
            this.mnuFind.Click += this.mnuFind_Click;
            this.mnuFindNext.Click += this.mnuFindNext_Click;
            this.mnuPaste.Click += this.mnuPaste_Click;
            this.mnuGoTo.Click += this.mnuGoTo_Click;
            this.mnuClearCache.Click += this.mnuClearCache_Click;

            string[] sqlKeyWords = Settings.CurrentType.Attributes["Sql92ReservedWords"].GetValue<string[]>();
            string[] providerKeyWords = provider.KeyWords;

            this.queryTextBox.AddKeyWords(new string[] {"exec"}, Color.Green);
            this.queryTextBox.AddKeyWords(sqlKeyWords, Color.Blue);
            this.queryTextBox.AddKeyWords(providerKeyWords, Color.Red);

            this.queryTextBox.CaretPositionPanel = this.sbPanelCaretPosition;

            this.SetText();

            this.resultSetsTabPage = new TabPage("Results");
            this.resultSetsTabControl = new TabControl();
            this.resultSetsTabControl.MouseUp += new MouseEventHandler(this.resultSetsTabControl_MouseUp);
            this.resultSetsTabControl.Alignment = TabAlignment.Top;
            this.resultSetsTabControl.Dock = DockStyle.Fill;
            this.resultSetsTabPage.Controls.Add(this.resultSetsTabControl);

            this.tabControl.TabPages.Add(this.messagesTabPage);
            this.tabControl.SelectedTab = this.messagesTabPage;
            this.tabControl.TabPages.Add(this.resultSetsTabPage);

            this.standardOutput = new StandardOutput(new TextBoxWriter(this.messagesTextBox), this);

            this.textBoxWriter = new TextBoxWriter(this.messagesTextBox);

            IObjectExplorer objectExplorer = provider.ObjectExplorer;

            if (objectExplorer != null)
            {
                objectExplorer.SetConnection(connectionString, connection.Connection);
                this.AddNodes(this.tvObjectExplorer.Nodes, objectExplorer.GetChildren(true), objectExplorer.Sortable);
            }
            else
            {
                this.tvObjectExplorer.Visible = false;
                this.splitterObjectExplorer.Visible = false;
                this.mnuObjectExplorer.Enabled = false;
            }

            string text = string.Format("&{0} - {1}", index + 1, this.Text);

            this.database = connection.Database;
            this.SetResultWriterType(ResultWriterType.DataGrid);

            ConfigurationNode node = Settings.CurrentType;
            ConfigurationAttributeCollection attributes = node.Attributes;
            this.rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
            this.htmlMaxRecords = attributes["HtmlMaxRecords"].GetValue<int>();
            this.wordMaxRecords = attributes["WordMaxRecords"].GetValue<int>();
            this.rowBlockSize = attributes["RowBlockSize"].GetValue<int>();
            this.timer.Interval = attributes["TimerInterval"].GetValue<int>();
        }

        private void CloseResultTabPage(TabPage tabPage)
        {
            //var control = tabPage.Controls[0];
            //var dataTableEditor = control as DataTableEditor;
            //if (dataTableEditor != null)
            //{
            //    var dataTable = dataTableEditor.DataTable;
            //    dataTableEditor.DataTable = null;
            //    dataTable.Clear();
            //    dataTable.AcceptChanges();
            //}
            tabPage.Controls.Clear();

            //GarbageMonitor.SetDisposeTime(control, LocalTime.Default.Now);
            //control.Dispose();
        }

        private void CloseResultSetTabPage(TabPage tabPage)
        {
            this.resultSetsTabControl.TabPages.Remove(tabPage);
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
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = new TCHITTESTINFO(e.X, e.Y);
                int index = SendMessage(this.resultSetsTabControl.Handle, TCM_HITTEST, IntPtr.Zero, ref hitTestInfo);
                if (index >= 0)
                {
                    TabPage hotTab = this.resultSetsTabControl.TabPages[index];
                    var contextMenu = new ContextMenuStrip(this.components);
                    contextMenu.Items.Add(new ToolStripMenuItem("Close", null, this.CloseResultSetTabPage_Click)
                    {
                        Tag = hotTab
                    });
                    contextMenu.Items.Add(new ToolStripMenuItem("Close all", null, this.mnuCloseAllTabPages_Click, Keys.Control | Keys.Shift | Keys.F4));
                    contextMenu.Show(this.resultSetsTabControl, e.Location);
                }
            }
        }

        #endregion

        #region Properties

        public CommandState ButtonState
        {
            get
            {
                return this.buttonState;
            }
        }

        public ConnectionBase Connection
        {
            get
            {
                return this.connection;
            }
        }

        public override Font Font
        {
            set
            {
                this.font = value;
                this.queryTextBox.Font = value;
                Size size1 = TextRenderer.MeasureText("1", value);
                Size size2 = TextRenderer.MeasureText("12", value);
                int width = this.queryTextBox.TabSize*(size2.Width - size1.Width);
                int[] tabs = new int[12];

                for (int i = 0; i < tabs.Length; i++)
                {
                    tabs[i] = (i + 1)*width;
                }

                this.queryTextBox.RichTextBox.Font = value;
                this.queryTextBox.RichTextBox.SelectionTabs = tabs;

                this.messagesTextBox.Font = value;
                this.messagesTextBox.SelectionTabs = tabs;
            }
        }

        public static NumberFormatInfo NumberFormat
        {
            get
            {
                return numberFormatInfo;
            }
        }

        public IProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        private string Query
        {
            get
            {
                string query = this.queryTextBox.SelectedText;

                if (query.Length == 0)
                {
                    query = this.queryTextBox.Text;
                }

                query = query.Replace("\n", "\r\n");
                return query;
            }
        }

        public QueryTextBox QueryTextBox
        {
            get
            {
                return this.queryTextBox;
            }
        }

        internal int ResultSetCount
        {
            get
            {
                return this.resultSetCount;
            }
        }

        public ResultWriterType TableStyle
        {
            get
            {
                return this.resultWriterType;
            }
        }

        internal ToolStrip ToolStrip
        {
            get
            {
                return this.toolStrip;
            }
        }

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
            this.queryTextBox.RichTextBox.AppendText(text);
        }

        public void ShowDataSet(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataSet tableSchema = null;
                string text;
                if (this.openTableMode)
                {
                    string tableName = this.sqlStatement.FindTableName();
                    text = tableName;
                    dataSet.Tables[0].TableName = tableName;
                    tableSchema = this.provider.GetTableSchema(this.connection.Connection, tableName);
                }
                else
                {
                    this.resultSetCount++;
                    text = string.Format("Set {0}", this.resultSetCount);
                }
                var resultSetTabPage = new TabPage(text);
                GarbageMonitor.Add("resultSetTabPage", resultSetTabPage);
                resultSetTabPage.ToolTipText = null; // TODO
                this.resultSetsTabControl.TabPages.Add(resultSetTabPage);
                this.resultSetsTabControl.SelectedTab = resultSetTabPage;
                if (dataSet.Tables.Count > 1)
                {
                    TabControl tabControl = new TabControl();
                    tabControl.Dock = DockStyle.Fill;
                    int index = 0;
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        var commandBuilder = this.provider.DbProviderFactory.CreateCommandBuilder();
                        var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataTable, tableSchema, this.resultWriterType, !this.openTableMode,
                            this.sbPanelText);
                        control.Dock = DockStyle.Fill;
                        //text = string.Format("Table {0}", index);
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
                    var commandBuilder = this.provider.DbProviderFactory.CreateCommandBuilder();
                    var control = QueryFormStaticMethods.CreateControlFromDataTable(commandBuilder, dataSet.Tables[0], tableSchema, this.resultWriterType, !this.openTableMode,
                        this.sbPanelText);
                    control.Dock = DockStyle.Fill;
                    resultSetTabPage.Controls.Add(control);
                }
            }
        }

        public void ShowXml(
            string tabPageName,
            string xml)
        {
            var htmlTextBox = new HtmlTextBox();
            htmlTextBox.Dock = DockStyle.Fill;

            var tabPage = new TabPage(tabPageName);
            this.tabControl.TabPages.Add(tabPage);
            tabPage.Controls.Add(htmlTextBox);

            htmlTextBox.Xml = xml;

            this.SettingsChanged(null, null);
            Settings.Changed += this.SettingsChanged;
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
            this.mainMenu = new global::System.Windows.Forms.MenuStrip();
            this.menuItem9 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAs = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuDuplicateConnection = new global::System.Windows.Forms.ToolStripMenuItem();
            this.menuItem8 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuFind = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuFindNext = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuIntelliSense = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuListMembers = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearCache = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuGoTo = new global::System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.menuItem7 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCommandTypeText = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCommandTypeStoredProcedure = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuDescribeParameters = new global::System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new global::System.Windows.Forms.ToolStripSeparator();
            this.mnuShowShemaTable = new global::System.Windows.Forms.ToolStripMenuItem();
            this.executeQueryToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteQuerySingleRow = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteQuerySchemaOnly = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteQueryKeyInfo = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteQueryXml = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenTable = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCancel = new global::System.Windows.Forms.ToolStripMenuItem();
            this.parseToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new global::System.Windows.Forms.ToolStripSeparator();
            this.menuItem2 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuText = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuDataGrid = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuHtml = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuRtf = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuListView = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuExcel = new global::System.Windows.Forms.ToolStripMenuItem();
            this.menuResultModeFile = new global::System.Windows.Forms.ToolStripMenuItem();
            this.sQLiteDatabaseToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.insertScriptFileToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new global::System.Windows.Forms.ToolStripSeparator();
            this.mnuGotoQueryEditor = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuGotoMessageTabPage = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseTabPage = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseAllTabPages = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreateInsert = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreateInsertSelect = new global::System.Windows.Forms.ToolStripMenuItem();
            this.createSqlCeDatabaseToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.beginTransactionToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.commitTransactionToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.rollbackTransactionToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.menuItem3 = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuObjectExplorer = new global::System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefreshObjectExplorer = new global::System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new global::System.Windows.Forms.StatusStrip();
            this.sbPanelText = new global::System.Windows.Forms.ToolStripStatusLabel();
            this.sbPanelTableStyle = new global::System.Windows.Forms.ToolStripStatusLabel();
            this.sbPanelTimer = new global::System.Windows.Forms.ToolStripStatusLabel();
            this.sbPanelRows = new global::System.Windows.Forms.ToolStripStatusLabel();
            this.sbPanelCaretPosition = new global::System.Windows.Forms.ToolStripStatusLabel();
            this.tvObjectExplorer = new global::System.Windows.Forms.TreeView();
            this.splitterObjectExplorer = new global::System.Windows.Forms.Splitter();
            this.splitterQuery = new global::System.Windows.Forms.Splitter();
            this.tabControl = new global::System.Windows.Forms.TabControl();
            this.toolStrip = new global::System.Windows.Forms.ToolStrip();
            this.toolStripSeparator4 = new global::System.Windows.Forms.ToolStripSeparator();
            this.execueQuerySplitButton = new global::System.Windows.Forms.ToolStripSplitButton();
            this.executeQueryMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.executeQuerySingleRowToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.cToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.openTableToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
            this.cancelQueryButton = new global::System.Windows.Forms.ToolStripButton();
            this.queryTextBox = new DataCommander.Providers.QueryTextBox();
            this.mainMenu.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.menuItem9,
            this.menuItem8,
            this.menuItem1,
            this.menuItem3});
            this.mainMenu.Location = new Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new global::System.Drawing.Size(1016, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Visible = false;
            // 
            // menuItem9
            // 
            this.menuItem9.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuSaveAs,
            this.mnuDuplicateConnection});
            this.menuItem9.MergeAction = global::System.Windows.Forms.MergeAction.MatchOnly;
            this.menuItem9.MergeIndex = 0;
            this.menuItem9.Name = "menuItem9";
            this.menuItem9.Size = new global::System.Drawing.Size(37, 20);
            this.menuItem9.Text = "&File";
            // 
            // mnuSave
            // 
            this.mnuSave.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.mnuSave.MergeIndex = 2;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new global::System.Drawing.Size(230, 22);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new global::System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.mnuSaveAs.MergeIndex = 3;
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.Size = new global::System.Drawing.Size(230, 22);
            this.mnuSaveAs.Text = "Save &As";
            this.mnuSaveAs.Click += new global::System.EventHandler(this.mnuSaveAs_Click);
            // 
            // mnuDuplicateConnection
            // 
            this.mnuDuplicateConnection.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.mnuDuplicateConnection.MergeIndex = 4;
            this.mnuDuplicateConnection.Name = "mnuDuplicateConnection";
            this.mnuDuplicateConnection.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Q)));
            this.mnuDuplicateConnection.Size = new global::System.Drawing.Size(230, 22);
            this.mnuDuplicateConnection.Text = "Duplicate connection";
            this.mnuDuplicateConnection.Click += new global::System.EventHandler(this.mnuDuplicateConnection_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuPaste,
            this.mnuFind,
            this.mnuFindNext,
            this.mnuIntelliSense,
            this.mnuGoTo});
            this.menuItem8.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.menuItem8.MergeIndex = 2;
            this.menuItem8.Name = "menuItem8";
            this.menuItem8.Size = new global::System.Drawing.Size(39, 20);
            this.menuItem8.Text = "&Edit";
            // 
            // mnuPaste
            // 
            this.mnuPaste.Image = ((global::System.Drawing.Image)(resources.GetObject("mnuPaste.Image")));
            this.mnuPaste.MergeIndex = 0;
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.V)));
            this.mnuPaste.Size = new global::System.Drawing.Size(157, 22);
            this.mnuPaste.Text = "&Paste";
            // 
            // mnuFind
            // 
            this.mnuFind.Image = ((global::System.Drawing.Image)(resources.GetObject("mnuFind.Image")));
            this.mnuFind.MergeIndex = 1;
            this.mnuFind.Name = "mnuFind";
            this.mnuFind.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.F)));
            this.mnuFind.Size = new global::System.Drawing.Size(157, 22);
            this.mnuFind.Text = "&Find";
            // 
            // mnuFindNext
            // 
            this.mnuFindNext.MergeIndex = 2;
            this.mnuFindNext.Name = "mnuFindNext";
            this.mnuFindNext.ShortcutKeys = global::System.Windows.Forms.Keys.F3;
            this.mnuFindNext.Size = new global::System.Drawing.Size(157, 22);
            this.mnuFindNext.Text = "Find &Next";
            // 
            // mnuIntelliSense
            // 
            this.mnuIntelliSense.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuListMembers,
            this.mnuClearCache});
            this.mnuIntelliSense.MergeIndex = 3;
            this.mnuIntelliSense.Name = "mnuIntelliSense";
            this.mnuIntelliSense.Size = new global::System.Drawing.Size(157, 22);
            this.mnuIntelliSense.Text = "&IntelliSense";
            // 
            // mnuListMembers
            // 
            this.mnuListMembers.MergeIndex = 0;
            this.mnuListMembers.Name = "mnuListMembers";
            this.mnuListMembers.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.J)));
            this.mnuListMembers.Size = new global::System.Drawing.Size(211, 22);
            this.mnuListMembers.Text = "&List Members";
            this.mnuListMembers.Click += new global::System.EventHandler(this.mnuListMembers_Click);
            // 
            // mnuClearCache
            // 
            this.mnuClearCache.MergeIndex = 1;
            this.mnuClearCache.Name = "mnuClearCache";
            this.mnuClearCache.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Shift) 
            | global::System.Windows.Forms.Keys.C)));
            this.mnuClearCache.Size = new global::System.Drawing.Size(211, 22);
            this.mnuClearCache.Text = "&Clear Cache";
            // 
            // mnuGoTo
            // 
            this.mnuGoTo.MergeIndex = 4;
            this.mnuGoTo.Name = "mnuGoTo";
            this.mnuGoTo.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.G)));
            this.mnuGoTo.Size = new global::System.Drawing.Size(157, 22);
            this.mnuGoTo.Text = "Go To...";
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.menuItem7,
            this.mnuDescribeParameters,
            this.toolStripSeparator2,
            this.mnuShowShemaTable,
            this.executeQueryToolStripMenuItem,
            this.mnuExecuteQuerySingleRow,
            this.mnuExecuteQuerySchemaOnly,
            this.mnuExecuteQueryKeyInfo,
            this.mnuExecuteQueryXml,
            this.mnuOpenTable,
            this.mnuCancel,
            this.parseToolStripMenuItem,
            this.toolStripSeparator1,
            this.menuItem2,
            this.toolStripSeparator3,
            this.mnuGotoQueryEditor,
            this.mnuGotoMessageTabPage,
            this.mnuCloseTabPage,
            this.mnuCloseAllTabPages,
            this.mnuCreateInsert,
            this.mnuCreateInsertSelect,
            this.createSqlCeDatabaseToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.beginTransactionToolStripMenuItem,
            this.commitTransactionToolStripMenuItem,
            this.rollbackTransactionToolStripMenuItem});
            this.menuItem1.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.menuItem1.MergeIndex = 3;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new global::System.Drawing.Size(51, 20);
            this.menuItem1.Text = "&Query";
            // 
            // menuItem7
            // 
            this.menuItem7.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuCommandTypeText,
            this.mnuCommandTypeStoredProcedure});
            this.menuItem7.MergeIndex = 0;
            this.menuItem7.Name = "menuItem7";
            this.menuItem7.Size = new global::System.Drawing.Size(269, 22);
            this.menuItem7.Text = "Command&Type";
            // 
            // mnuCommandTypeText
            // 
            this.mnuCommandTypeText.Checked = true;
            this.mnuCommandTypeText.CheckState = global::System.Windows.Forms.CheckState.Checked;
            this.mnuCommandTypeText.MergeIndex = 0;
            this.mnuCommandTypeText.Name = "mnuCommandTypeText";
            this.mnuCommandTypeText.Size = new global::System.Drawing.Size(165, 22);
            this.mnuCommandTypeText.Text = "Text";
            this.mnuCommandTypeText.Click += new global::System.EventHandler(this.mnuCommandTypeText_Click);
            // 
            // mnuCommandTypeStoredProcedure
            // 
            this.mnuCommandTypeStoredProcedure.MergeIndex = 1;
            this.mnuCommandTypeStoredProcedure.Name = "mnuCommandTypeStoredProcedure";
            this.mnuCommandTypeStoredProcedure.Size = new global::System.Drawing.Size(165, 22);
            this.mnuCommandTypeStoredProcedure.Text = "Stored Procedure";
            this.mnuCommandTypeStoredProcedure.Click += new global::System.EventHandler(this.mnuCommandTypeStoredProcedure_Click);
            // 
            // mnuDescribeParameters
            // 
            this.mnuDescribeParameters.MergeIndex = 1;
            this.mnuDescribeParameters.Name = "mnuDescribeParameters";
            this.mnuDescribeParameters.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.P)));
            this.mnuDescribeParameters.Size = new global::System.Drawing.Size(269, 22);
            this.mnuDescribeParameters.Text = "Describe &Parameters";
            this.mnuDescribeParameters.Click += new global::System.EventHandler(this.mnuDescribeParameters_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new global::System.Drawing.Size(266, 6);
            // 
            // mnuShowShemaTable
            // 
            this.mnuShowShemaTable.MergeIndex = 3;
            this.mnuShowShemaTable.Name = "mnuShowShemaTable";
            this.mnuShowShemaTable.Size = new global::System.Drawing.Size(269, 22);
            this.mnuShowShemaTable.Text = "Show SchemaTable";
            this.mnuShowShemaTable.Click += new global::System.EventHandler(this.mnuShowShemaTable_Click);
            // 
            // executeQueryToolStripMenuItem
            // 
            this.executeQueryToolStripMenuItem.Name = "executeQueryToolStripMenuItem";
            this.executeQueryToolStripMenuItem.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.E)));
            this.executeQueryToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.executeQueryToolStripMenuItem.Text = "Execute Query";
            this.executeQueryToolStripMenuItem.Click += new global::System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // mnuExecuteQuerySingleRow
            // 
            this.mnuExecuteQuerySingleRow.MergeIndex = 6;
            this.mnuExecuteQuerySingleRow.Name = "mnuExecuteQuerySingleRow";
            this.mnuExecuteQuerySingleRow.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.D1)));
            this.mnuExecuteQuerySingleRow.Size = new global::System.Drawing.Size(269, 22);
            this.mnuExecuteQuerySingleRow.Text = "Execute Query (SingleRow)";
            this.mnuExecuteQuerySingleRow.Click += new global::System.EventHandler(this.mnuSingleRow_Click);
            // 
            // mnuExecuteQuerySchemaOnly
            // 
            this.mnuExecuteQuerySchemaOnly.MergeIndex = 7;
            this.mnuExecuteQuerySchemaOnly.Name = "mnuExecuteQuerySchemaOnly";
            this.mnuExecuteQuerySchemaOnly.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.R)));
            this.mnuExecuteQuerySchemaOnly.Size = new global::System.Drawing.Size(269, 22);
            this.mnuExecuteQuerySchemaOnly.Text = "Execute Query (Schema only)";
            this.mnuExecuteQuerySchemaOnly.Click += new global::System.EventHandler(this.mnuResultSchema_Click);
            // 
            // mnuExecuteQueryKeyInfo
            // 
            this.mnuExecuteQueryKeyInfo.MergeIndex = 8;
            this.mnuExecuteQueryKeyInfo.Name = "mnuExecuteQueryKeyInfo";
            this.mnuExecuteQueryKeyInfo.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.K)));
            this.mnuExecuteQueryKeyInfo.Size = new global::System.Drawing.Size(269, 22);
            this.mnuExecuteQueryKeyInfo.Text = "Execute Query (&KeyInfo)";
            this.mnuExecuteQueryKeyInfo.Click += new global::System.EventHandler(this.mnuKeyInfo_Click);
            // 
            // mnuExecuteQueryXml
            // 
            this.mnuExecuteQueryXml.MergeIndex = 9;
            this.mnuExecuteQueryXml.Name = "mnuExecuteQueryXml";
            this.mnuExecuteQueryXml.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Shift) 
            | global::System.Windows.Forms.Keys.X)));
            this.mnuExecuteQueryXml.Size = new global::System.Drawing.Size(269, 22);
            this.mnuExecuteQueryXml.Text = "Execute Query (XML)";
            this.mnuExecuteQueryXml.Click += new global::System.EventHandler(this.mnuXml_Click);
            // 
            // mnuOpenTable
            // 
            this.mnuOpenTable.MergeIndex = 10;
            this.mnuOpenTable.Name = "mnuOpenTable";
            this.mnuOpenTable.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Shift) 
            | global::System.Windows.Forms.Keys.O)));
            this.mnuOpenTable.Size = new global::System.Drawing.Size(269, 22);
            this.mnuOpenTable.Text = "Open Table";
            this.mnuOpenTable.Click += new global::System.EventHandler(this.mnuOpenTable_Click);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Enabled = false;
            this.mnuCancel.MergeIndex = 11;
            this.mnuCancel.Name = "mnuCancel";
            this.mnuCancel.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.F12)));
            this.mnuCancel.Size = new global::System.Drawing.Size(269, 22);
            this.mnuCancel.Text = "&Cancel Executing Query";
            this.mnuCancel.Click += new global::System.EventHandler(this.mnuCancel_Click);
            // 
            // parseToolStripMenuItem
            // 
            this.parseToolStripMenuItem.Name = "parseToolStripMenuItem";
            this.parseToolStripMenuItem.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.F5)));
            this.parseToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.parseToolStripMenuItem.Text = "Parse";
            this.parseToolStripMenuItem.Click += new global::System.EventHandler(this.parseToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new global::System.Drawing.Size(266, 6);
            // 
            // menuItem2
            // 
            this.menuItem2.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuText,
            this.mnuDataGrid,
            this.mnuHtml,
            this.mnuRtf,
            this.mnuListView,
            this.mnuExcel,
            this.menuResultModeFile,
            this.sQLiteDatabaseToolStripMenuItem,
            this.insertScriptFileToolStripMenuItem});
            this.menuItem2.MergeIndex = 13;
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new global::System.Drawing.Size(269, 22);
            this.menuItem2.Text = "Result &Mode";
            // 
            // mnuText
            // 
            this.mnuText.MergeIndex = 0;
            this.mnuText.Name = "mnuText";
            this.mnuText.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.T)));
            this.mnuText.Size = new global::System.Drawing.Size(221, 22);
            this.mnuText.Text = "&Text";
            this.mnuText.Click += new global::System.EventHandler(this.mnuText_Click);
            // 
            // mnuDataGrid
            // 
            this.mnuDataGrid.MergeIndex = 1;
            this.mnuDataGrid.Name = "mnuDataGrid";
            this.mnuDataGrid.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.D)));
            this.mnuDataGrid.Size = new global::System.Drawing.Size(221, 22);
            this.mnuDataGrid.Text = "&DataGrid";
            this.mnuDataGrid.Click += new global::System.EventHandler(this.mnuDataGrid_Click);
            // 
            // mnuHtml
            // 
            this.mnuHtml.MergeIndex = 2;
            this.mnuHtml.Name = "mnuHtml";
            this.mnuHtml.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.H)));
            this.mnuHtml.Size = new global::System.Drawing.Size(221, 22);
            this.mnuHtml.Text = "&Html";
            this.mnuHtml.Click += new global::System.EventHandler(this.mnuHtml_Click);
            // 
            // mnuRtf
            // 
            this.mnuRtf.MergeIndex = 3;
            this.mnuRtf.Name = "mnuRtf";
            this.mnuRtf.Size = new global::System.Drawing.Size(221, 22);
            this.mnuRtf.Text = "&Rtf";
            // 
            // mnuListView
            // 
            this.mnuListView.MergeIndex = 4;
            this.mnuListView.Name = "mnuListView";
            this.mnuListView.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.L)));
            this.mnuListView.Size = new global::System.Drawing.Size(221, 22);
            this.mnuListView.Text = "&ListView";
            this.mnuListView.Click += new global::System.EventHandler(this.mnuListView_Click);
            // 
            // mnuExcel
            // 
            this.mnuExcel.MergeIndex = 5;
            this.mnuExcel.Name = "mnuExcel";
            this.mnuExcel.Size = new global::System.Drawing.Size(221, 22);
            this.mnuExcel.Text = "&Excel";
            this.mnuExcel.Visible = false;
            // 
            // menuResultModeFile
            // 
            this.menuResultModeFile.MergeIndex = 6;
            this.menuResultModeFile.Name = "menuResultModeFile";
            this.menuResultModeFile.Size = new global::System.Drawing.Size(221, 22);
            this.menuResultModeFile.Text = "&File";
            this.menuResultModeFile.Click += new global::System.EventHandler(this.menuResultModeFile_Click);
            // 
            // sQLiteDatabaseToolStripMenuItem
            // 
            this.sQLiteDatabaseToolStripMenuItem.Name = "sQLiteDatabaseToolStripMenuItem";
            this.sQLiteDatabaseToolStripMenuItem.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Alt) 
            | global::System.Windows.Forms.Keys.S)));
            this.sQLiteDatabaseToolStripMenuItem.Size = new global::System.Drawing.Size(221, 22);
            this.sQLiteDatabaseToolStripMenuItem.Text = "SQLite database";
            this.sQLiteDatabaseToolStripMenuItem.Click += new global::System.EventHandler(this.sQLiteDatabaseToolStripMenuItem_Click);
            // 
            // insertScriptFileToolStripMenuItem
            // 
            this.insertScriptFileToolStripMenuItem.Name = "insertScriptFileToolStripMenuItem";
            this.insertScriptFileToolStripMenuItem.Size = new global::System.Drawing.Size(221, 22);
            this.insertScriptFileToolStripMenuItem.Text = "Insert Script File";
            this.insertScriptFileToolStripMenuItem.Click += new global::System.EventHandler(this.insertScriptFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new global::System.Drawing.Size(266, 6);
            // 
            // mnuGotoQueryEditor
            // 
            this.mnuGotoQueryEditor.MergeIndex = 15;
            this.mnuGotoQueryEditor.Name = "mnuGotoQueryEditor";
            this.mnuGotoQueryEditor.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Shift) 
            | global::System.Windows.Forms.Keys.Q)));
            this.mnuGotoQueryEditor.Size = new global::System.Drawing.Size(269, 22);
            this.mnuGotoQueryEditor.Text = "Goto &Query Editor";
            this.mnuGotoQueryEditor.Click += new global::System.EventHandler(this.mnuGotoQueryEditor_Click);
            // 
            // mnuGotoMessageTabPage
            // 
            this.mnuGotoMessageTabPage.MergeIndex = 16;
            this.mnuGotoMessageTabPage.Name = "mnuGotoMessageTabPage";
            this.mnuGotoMessageTabPage.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.M)));
            this.mnuGotoMessageTabPage.Size = new global::System.Drawing.Size(269, 22);
            this.mnuGotoMessageTabPage.Text = "Goto &Message TabPage";
            this.mnuGotoMessageTabPage.Click += new global::System.EventHandler(this.mnuGotoMessageTabPage_Click);
            // 
            // mnuCloseTabPage
            // 
            this.mnuCloseTabPage.MergeIndex = 17;
            this.mnuCloseTabPage.Name = "mnuCloseTabPage";
            this.mnuCloseTabPage.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.F4)));
            this.mnuCloseTabPage.Size = new global::System.Drawing.Size(269, 22);
            this.mnuCloseTabPage.Text = "Close Current &TabPage";
            this.mnuCloseTabPage.Click += new global::System.EventHandler(this.mnuCloseTabPage_Click);
            // 
            // mnuCloseAllTabPages
            // 
            this.mnuCloseAllTabPages.MergeIndex = 18;
            this.mnuCloseAllTabPages.Name = "mnuCloseAllTabPages";
            this.mnuCloseAllTabPages.ShortcutKeys = ((global::System.Windows.Forms.Keys)(((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.Shift) 
            | global::System.Windows.Forms.Keys.F4)));
            this.mnuCloseAllTabPages.Size = new global::System.Drawing.Size(269, 22);
            this.mnuCloseAllTabPages.Text = "Close &All TabPages";
            this.mnuCloseAllTabPages.Click += new global::System.EventHandler(this.mnuCloseAllTabPages_Click);
            // 
            // mnuCreateInsert
            // 
            this.mnuCreateInsert.MergeIndex = 19;
            this.mnuCreateInsert.Name = "mnuCreateInsert";
            this.mnuCreateInsert.ShortcutKeys = ((global::System.Windows.Forms.Keys)((global::System.Windows.Forms.Keys.Control | global::System.Windows.Forms.Keys.I)));
            this.mnuCreateInsert.Size = new global::System.Drawing.Size(269, 22);
            this.mnuCreateInsert.Text = "Create insert statements";
            this.mnuCreateInsert.Click += new global::System.EventHandler(this.mnuCreateInsert_Click);
            // 
            // mnuCreateInsertSelect
            // 
            this.mnuCreateInsertSelect.MergeIndex = 20;
            this.mnuCreateInsertSelect.Name = "mnuCreateInsertSelect";
            this.mnuCreateInsertSelect.Size = new global::System.Drawing.Size(269, 22);
            this.mnuCreateInsertSelect.Text = "Create \'insert select\' statements";
            this.mnuCreateInsertSelect.Click += new global::System.EventHandler(this.mnuCreateInsertSelect_Click);
            // 
            // createSqlCeDatabaseToolStripMenuItem
            // 
            this.createSqlCeDatabaseToolStripMenuItem.Name = "createSqlCeDatabaseToolStripMenuItem";
            this.createSqlCeDatabaseToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.createSqlCeDatabaseToolStripMenuItem.Text = "Create SQL Server Compact database";
            this.createSqlCeDatabaseToolStripMenuItem.Click += new global::System.EventHandler(this.createSqlCeDatabaseToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            // 
            // beginTransactionToolStripMenuItem
            // 
            this.beginTransactionToolStripMenuItem.Name = "beginTransactionToolStripMenuItem";
            this.beginTransactionToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.beginTransactionToolStripMenuItem.Text = "Begin Transaction";
            this.beginTransactionToolStripMenuItem.Click += new global::System.EventHandler(this.beginTransactionToolStripMenuItem_Click);
            // 
            // commitTransactionToolStripMenuItem
            // 
            this.commitTransactionToolStripMenuItem.Enabled = false;
            this.commitTransactionToolStripMenuItem.Name = "commitTransactionToolStripMenuItem";
            this.commitTransactionToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.commitTransactionToolStripMenuItem.Text = "Commit Transaction";
            this.commitTransactionToolStripMenuItem.Click += new global::System.EventHandler(this.commitTransactionToolStripMenuItem_Click);
            // 
            // rollbackTransactionToolStripMenuItem
            // 
            this.rollbackTransactionToolStripMenuItem.Enabled = false;
            this.rollbackTransactionToolStripMenuItem.Name = "rollbackTransactionToolStripMenuItem";
            this.rollbackTransactionToolStripMenuItem.Size = new global::System.Drawing.Size(269, 22);
            this.rollbackTransactionToolStripMenuItem.Text = "Rollback Transaction";
            this.rollbackTransactionToolStripMenuItem.Click += new global::System.EventHandler(this.rollbackTransactionToolStripMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.mnuObjectExplorer,
            this.mnuRefreshObjectExplorer});
            this.menuItem3.MergeAction = global::System.Windows.Forms.MergeAction.Insert;
            this.menuItem3.MergeIndex = 4;
            this.menuItem3.Name = "menuItem3";
            this.menuItem3.Size = new global::System.Drawing.Size(48, 20);
            this.menuItem3.Text = "&Tools";
            // 
            // mnuObjectExplorer
            // 
            this.mnuObjectExplorer.MergeIndex = 0;
            this.mnuObjectExplorer.Name = "mnuObjectExplorer";
            this.mnuObjectExplorer.ShortcutKeys = global::System.Windows.Forms.Keys.F8;
            this.mnuObjectExplorer.Size = new global::System.Drawing.Size(229, 22);
            this.mnuObjectExplorer.Text = "Object Explorer";
            this.mnuObjectExplorer.Click += new global::System.EventHandler(this.menuObjectExplorer_Click);
            // 
            // mnuRefreshObjectExplorer
            // 
            this.mnuRefreshObjectExplorer.MergeIndex = 1;
            this.mnuRefreshObjectExplorer.Name = "mnuRefreshObjectExplorer";
            this.mnuRefreshObjectExplorer.Size = new global::System.Drawing.Size(229, 22);
            this.mnuRefreshObjectExplorer.Text = "Refresh Object Explorer\'s root";
            this.mnuRefreshObjectExplorer.Click += new global::System.EventHandler(this.mnuRefreshObjectExplorer_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.sbPanelText,
            this.sbPanelTableStyle,
            this.sbPanelTimer,
            this.sbPanelRows,
            this.sbPanelCaretPosition});
            this.statusBar.Location = new Point(300, 586);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new global::System.Drawing.Size(716, 22);
            this.statusBar.TabIndex = 2;
            // 
            // sbPanelText
            // 
            this.sbPanelText.AutoSize = false;
            this.sbPanelText.Name = "sbPanelText";
            this.sbPanelText.Size = new global::System.Drawing.Size(231, 17);
            this.sbPanelText.Spring = true;
            this.sbPanelText.TextAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sbPanelTableStyle
            // 
            this.sbPanelTableStyle.AutoSize = false;
            this.sbPanelTableStyle.BorderSides = global::System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sbPanelTableStyle.Name = "sbPanelTableStyle";
            this.sbPanelTableStyle.Size = new global::System.Drawing.Size(100, 17);
            this.sbPanelTableStyle.TextAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
            this.sbPanelTableStyle.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.sbPanelTableStyle_MouseUp);
            // 
            // sbPanelTimer
            // 
            this.sbPanelTimer.Alignment = global::System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sbPanelTimer.AutoSize = false;
            this.sbPanelTimer.BorderSides = global::System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sbPanelTimer.Name = "sbPanelTimer";
            this.sbPanelTimer.Size = new global::System.Drawing.Size(70, 17);
            this.sbPanelTimer.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sbPanelRows
            // 
            this.sbPanelRows.Alignment = global::System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sbPanelRows.AutoSize = false;
            this.sbPanelRows.BorderSides = global::System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sbPanelRows.Name = "sbPanelRows";
            this.sbPanelRows.Size = new global::System.Drawing.Size(200, 17);
            this.sbPanelRows.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sbPanelCaretPosition
            // 
            this.sbPanelCaretPosition.AutoSize = false;
            this.sbPanelCaretPosition.BorderSides = global::System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sbPanelCaretPosition.Name = "sbPanelCaretPosition";
            this.sbPanelCaretPosition.Size = new global::System.Drawing.Size(100, 17);
            this.sbPanelCaretPosition.TextAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tvObjectExplorer
            // 
            this.tvObjectExplorer.Dock = global::System.Windows.Forms.DockStyle.Left;
            this.tvObjectExplorer.Font = new Font("Tahoma", 8.25F, global::System.Drawing.FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
            this.tvObjectExplorer.Location = new Point(0, 0);
            this.tvObjectExplorer.Name = "tvObjectExplorer";
            this.tvObjectExplorer.Size = new global::System.Drawing.Size(300, 608);
            this.tvObjectExplorer.TabIndex = 4;
            this.tvObjectExplorer.BeforeExpand += new global::System.Windows.Forms.TreeViewCancelEventHandler(this.tvObjectBrowser_BeforeExpand);
            this.tvObjectExplorer.ItemDrag += new global::System.Windows.Forms.ItemDragEventHandler(this.tvObjectBrowser_ItemDrag);
            this.tvObjectExplorer.DoubleClick += new global::System.EventHandler(this.tvObjectBrowser_DoubleClick);
            this.tvObjectExplorer.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.tvObjectBrowser_MouseDown);
            this.tvObjectExplorer.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.tvObjectExplorer_MouseUp);
            // 
            // splitterObjectExplorer
            // 
            this.splitterObjectExplorer.Location = new Point(300, 0);
            this.splitterObjectExplorer.Name = "splitterObjectExplorer";
            this.splitterObjectExplorer.Size = new global::System.Drawing.Size(3, 586);
            this.splitterObjectExplorer.TabIndex = 5;
            this.splitterObjectExplorer.TabStop = false;
            // 
            // splitterQuery
            // 
            this.splitterQuery.Dock = global::System.Windows.Forms.DockStyle.Top;
            this.splitterQuery.Location = new Point(303, 279);
            this.splitterQuery.Name = "splitterQuery";
            this.splitterQuery.Size = new global::System.Drawing.Size(713, 2);
            this.splitterQuery.TabIndex = 7;
            this.splitterQuery.TabStop = false;
            // 
            // tabControl
            // 
            this.tabControl.Dock = global::System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new Point(303, 281);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new global::System.Drawing.Size(713, 305);
            this.tabControl.TabIndex = 8;
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = global::System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator4,
            this.execueQuerySplitButton,
            this.cancelQueryButton});
            this.toolStrip.Location = new Point(303, 281);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new global::System.Drawing.Size(73, 25);
            this.toolStrip.TabIndex = 9;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new global::System.Drawing.Size(6, 25);
            // 
            // execueQuerySplitButton
            // 
            this.execueQuerySplitButton.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.execueQuerySplitButton.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[] {
            this.executeQueryMenuItem,
            this.executeQuerySingleRowToolStripMenuItem,
            this.cToolStripMenuItem,
            this.openTableToolStripMenuItem});
            this.execueQuerySplitButton.Image = ((global::System.Drawing.Image)(resources.GetObject("execueQuerySplitButton.Image")));
            this.execueQuerySplitButton.ImageTransparentColor = global::System.Drawing.Color.Magenta;
            this.execueQuerySplitButton.Name = "execueQuerySplitButton";
            this.execueQuerySplitButton.Size = new global::System.Drawing.Size(32, 22);
            this.execueQuerySplitButton.Text = "Execute Query";
            this.execueQuerySplitButton.ButtonClick += new global::System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // executeQueryMenuItem
            // 
            this.executeQueryMenuItem.Name = "executeQueryMenuItem";
            this.executeQueryMenuItem.ShortcutKeys = global::System.Windows.Forms.Keys.F5;
            this.executeQueryMenuItem.Size = new global::System.Drawing.Size(168, 22);
            this.executeQueryMenuItem.Text = "Execute Query";
            this.executeQueryMenuItem.Click += new global::System.EventHandler(this.aToolStripMenuItem_Click);
            // 
            // executeQuerySingleRowToolStripMenuItem
            // 
            this.executeQuerySingleRowToolStripMenuItem.Name = "executeQuerySingleRowToolStripMenuItem";
            this.executeQuerySingleRowToolStripMenuItem.Size = new global::System.Drawing.Size(168, 22);
            this.executeQuerySingleRowToolStripMenuItem.Text = "Single Row";
            this.executeQuerySingleRowToolStripMenuItem.Click += new global::System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            this.cToolStripMenuItem.Size = new global::System.Drawing.Size(168, 22);
            this.cToolStripMenuItem.Text = "XML";
            // 
            // openTableToolStripMenuItem
            // 
            this.openTableToolStripMenuItem.Name = "openTableToolStripMenuItem";
            this.openTableToolStripMenuItem.Size = new global::System.Drawing.Size(168, 22);
            this.openTableToolStripMenuItem.Text = "Open Table";
            this.openTableToolStripMenuItem.Click += new global::System.EventHandler(this.openTableToolStripMenuItem_Click);
            // 
            // cancelQueryButton
            // 
            this.cancelQueryButton.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cancelQueryButton.Enabled = false;
            this.cancelQueryButton.Image = ((global::System.Drawing.Image)(resources.GetObject("cancelQueryButton.Image")));
            this.cancelQueryButton.ImageTransparentColor = global::System.Drawing.Color.Magenta;
            this.cancelQueryButton.Name = "cancelQueryButton";
            this.cancelQueryButton.Size = new global::System.Drawing.Size(23, 22);
            this.cancelQueryButton.Text = "Cancel Executing Query";
            this.cancelQueryButton.Click += new global::System.EventHandler(this.cancelExecutingQueryButton_Click);
            // 
            // queryTextBox
            // 
            this.queryTextBox.Dock = global::System.Windows.Forms.DockStyle.Top;
            this.queryTextBox.Font = new Font("Consolas", 9F, global::System.Drawing.FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
            this.queryTextBox.KeyboardHandler = null;
            this.queryTextBox.Location = new Point(303, 0);
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.Size = new global::System.Drawing.Size(713, 279);
            this.queryTextBox.TabIndex = 1;
            this.queryTextBox.TabSize = 4;
            // 
            // QueryForm
            // 
            this.AutoScaleBaseSize = new global::System.Drawing.Size(7, 15);
            this.ClientSize = new global::System.Drawing.Size(1016, 608);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.splitterQuery);
            this.Controls.Add(this.queryTextBox);
            this.Controls.Add(this.splitterObjectExplorer);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.tvObjectExplorer);
            this.Controls.Add(this.mainMenu);
            this.Font = new Font("Consolas", 9F, global::System.Drawing.FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((global::System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "QueryForm";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Private Methods

        private void AddNodes(
            TreeNodeCollection parent,
            IEnumerable<ITreeNode> children,
            bool sortable)
        {
            long ticks = Stopwatch.GetTimestamp();
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

            int count = 0;

            if (children != null)
            {
                foreach (ITreeNode child in children)
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
            this.sbPanelText.Text = string.Format("{0} item(s) added to Object Explorer in {1}.", count, StopwatchTimeSpan.ToString(ticks, 3));
            this.sbPanelText.ForeColor = SystemColors.ControlText;
        }

        public void AddInfoMessage(InfoMessage infoMessage)
        {
            WriteInfoMessageToLog(infoMessage);

            if (infoMessage.Severity == InfoMessageSeverity.Error)
            {
                this.errorCount++;
            }

            this.infoMessages.Enqueue(infoMessage);
            this.enqueueEvent.Set();
        }

        private void AddInfoMessages(IEnumerable<InfoMessage> infoMessages)
        {
            foreach (var infoMessage in infoMessages)
            {
                WriteInfoMessageToLog(infoMessage);
            }

            int errorCount =
                (from infoMessage in infoMessages
                    where infoMessage.Severity == InfoMessageSeverity.Error
                    select infoMessage).Count();
            this.errorCount += errorCount;

            this.infoMessages.TryAddRange(infoMessages);
            this.enqueueEvent.Set();
        }

        private void AppendMessageText(
            DateTime dateTime,
            InfoMessageSeverity severity,
            string text)
        {
            string s = "[" + dateTime.ToString("HH:mm:ss.fff");

            if (severity == InfoMessageSeverity.Error)
            {
                s += ",Error";
            }

            s += "] " + text + "\r\n";
            this.messagesTextBox.AppendText(s);
        }

        private void AddTabPage(
            TabControl tabControl,
            string tabPageName,
            string tooltipText,
            Control control)
        {
            TabPage tabPage = new TabPage(tabPageName);
            tabPage.ToolTipText = tooltipText;
            tabPage.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            tabControl.TabPages.Add(tabPage);
        }

        private void Connection_InfoMessage(IEnumerable<InfoMessage> messages)
        {
            this.AddInfoMessages(messages);
        }

        private static string DBValue(object value)
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

            if (this.command != null)
            {
                sb.Append(this.command.CommandText + "\n");
            }

            if (dataTable != null)
            {
                sb.Append(dataTable.Rows.Count + " row(s)");
            }

            return sb.ToString();
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            ConfigurationNode folder = Settings.CurrentType;
            this.commandTimeout = folder.Attributes["CommandTimeout"].GetValue<int>();
        }

        private void SetText()
        {
            var sb = new StringBuilder();
            sb.Append(this.connection.ConnectionName);
            sb.Append(" - ");
            sb.Append(this.connection.Caption);

            if (this.fileName != null)
            {
                sb.AppendFormat(" - {0}", this.fileName);
            }

            this.Text = sb.ToString();

            var mainForm = Application.Instance.MainForm;
            mainForm.ActiveMdiChildToolStripTextBox.Text = sb.ToString();
        }

        private void sbPanelTableStyle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip(this.components);
                Array values = Enum.GetValues(typeof (ResultWriterType));

                for (int i = 0; i < values.Length; i++)
                {
                    var tableStyle = (ResultWriterType)values.GetValue(i);
                    var item = new ToolStripMenuItem();
                    item.Text = tableStyle.ToString();
                    item.Tag = tableStyle;
                    item.Click += this.TableStyleMenuItem_Click;
                    contextMenu.Items.Add(item);
                }

                var bounds = this.sbPanelTableStyle.Bounds;
                var location = e.Location;
                contextMenu.Show(this.statusBar, bounds.X + location.X, bounds.Y + location.Y);
            }
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteQuerySingleRow();
        }

        private void ExecuteQuery()
        {
            log.Write(LogLevel.Trace, "ExecuteQuery...");

            this.Cursor = Cursors.AppStarting;
            this.SetGui(CommandState.Cancel);

            if (this.dataAdapter != null)
            {
                log.Write(LogLevel.Error, "this.dataAdapter == null failed");
            }

            Contract.Assert(this.dataAdapter == null);

            log.Trace("ThreadMonitor:\r\n{0}", ThreadMonitor.ToStringTable());
            ThreadMonitor.Join(0);
            log.Write(LogLevel.Trace, GarbageMonitor.State);
            this.openTableMode = false;
            this.dataAdapter = new AsyncDataAdapter();
            this.cancel = false;

            try
            {
                this.sbPanelText.Text = "Executing query...";
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                string query = this.Query;
                var statements = this.provider.GetStatements(query);
                log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
                IEnumerable<IDbCommand> commands;

                if (statements.Count == 1)
                {
                    this.sqlStatement = new SqlStatement(statements[0]);
                    var command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
                    command.Transaction = this.transaction;
                    commands = command.ItemAsEnumerable();
                }
                else
                {
                    commands =
                        from statement in statements
                        select this.connection.Connection.CreateCommand(this.transaction, statement, CommandType.Text, this.commandTimeout);
                }

                int maxRecords;
                IResultWriter resultWriter = null;

                switch (this.resultWriterType)
                {
                    case ResultWriterType.DataGrid:
                    case ResultWriterType.ListView:
                        maxRecords = int.MaxValue;
                        this.dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this.showSchemaTable);
                        resultWriter = this.dataSetResultWriter;
                        break;

                    case ResultWriterType.DataGridView:
                        maxRecords = int.MaxValue;
                        resultWriter = new DataGridViewResultWriter();
                        break;

                    case ResultWriterType.Html:
                        maxRecords = this.htmlMaxRecords;
                        this.dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this.showSchemaTable);
                        resultWriter = this.dataSetResultWriter;
                        break;

                    case ResultWriterType.Rtf:
                        maxRecords = this.wordMaxRecords;
                        this.dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this.showSchemaTable);
                        resultWriter = this.dataSetResultWriter;
                        break;

                    case ResultWriterType.File:
                        maxRecords = int.MaxValue;
                        resultWriter = new FileResultWriter(this.textBoxWriter);
                        this.tabControl.SelectedTab = this.messagesTabPage;
                        break;

                    case ResultWriterType.SQLite:
                        maxRecords = int.MaxValue;
                        string tableName = this.sqlStatement.FindTableName();
                        resultWriter = new SQLiteResultWriter(this.textBoxWriter, tableName);
                        this.tabControl.SelectedTab = this.messagesTabPage;
                        break;

                    case ResultWriterType.InsertScriptFile:
                        maxRecords = int.MaxValue;
                        tableName = this.sqlStatement.FindTableName();
                        resultWriter = new InsertScriptFileWriter(tableName, this.textBoxWriter);
                        this.tabControl.SelectedTab = this.messagesTabPage;
                        break;

                    case ResultWriterType.Excel:
                        maxRecords = int.MaxValue;
                        resultWriter = new ExcelResultWriter(this.provider, this.AddInfoMessage);
                        this.tabControl.SelectedTab = this.messagesTabPage;
                        break;

                    case ResultWriterType.Log:
                        maxRecords = int.MaxValue;
                        resultWriter = new LogResultWriter(this.AddInfoMessage);
                        this.tabControl.SelectedTab = this.messagesTabPage;
                        break;

                    default:
                        maxRecords = int.MaxValue;
                        var textBox = new RichTextBox();
                        GarbageMonitor.Add("ExecuteQuery.textBox", textBox);
                        textBox.MaxLength = int.MaxValue;
                        textBox.Multiline = true;
                        textBox.WordWrap = false;
                        textBox.Font = this.font;
                        textBox.ScrollBars = RichTextBoxScrollBars.Both;
                        textBox.SelectionChanged += this.textBox_SelectionChanged;

                        this.ShowTabPage("TextResult", this.GetToolTipText(null), textBox);
                        var textWriter = new TextBoxWriter(textBox);
                        resultWriter = new TextResultWriter(textWriter, this);
                        break;
                }

                this.stopwatch.Start();
                this.timer.Start();
                this.ShowTimer(0);

                this.errorCount = 0;
                this.dataAdapter.BeginFill(this.provider, commands, maxRecords, this.rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            catch (Exception ex)
            {
                this.EndFill(this.dataAdapter, ex);
            }
        }

        private void ShowDataTableText(DataTable dataTable)
        {
            var textBox = new RichTextBox();
            GarbageMonitor.Add("ShowDataTableText.textBox", textBox);
            textBox.MaxLength = int.MaxValue;
            textBox.Multiline = true;
            textBox.WordWrap = false;
            textBox.Font = this.font;
            textBox.ScrollBars = RichTextBoxScrollBars.Both;

            this.ShowTabPage("TextResult", this.GetToolTipText(null), textBox);

            TextWriter textWriter = new TextBoxWriter(textBox);
            var resultWriter = new TextResultWriter(textWriter, this);

            resultWriter.WriteBegin();

            var schemaTable = new DataTable();

            schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof (string));
            schemaTable.Columns.Add("ColumnSize", typeof (int));
            schemaTable.Columns.Add("DataType", typeof (Type));
            schemaTable.Columns.Add("NumericPrecision", typeof (short));
            schemaTable.Columns.Add("NumericScale", typeof (short));

            foreach (DataColumn column in dataTable.Columns)
            {
                Type type = column.DataType;
                TypeCode typeCode = Type.GetTypeCode(type);
                int columnSize;
                const short numericPrecision = 0;
                const short numericScale = 0;

                switch (typeCode)
                {
                    case TypeCode.String:
                        int maxLength = 0;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            int length = dataRow[column].ToString().Length;

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

            resultWriter.WriteTableBegin(schemaTable, null);

            int colCount = dataTable.Columns.Count;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow dataRow = dataTable.Rows[i];
                object[] values = new object[colCount];

                for (int j = 0; j < colCount; j++)
                {
                    values[j] = dataRow[j];
                }

                resultWriter.WriteRows(new object[][] {values}, 1);
            }

            resultWriter.WriteTableEnd();

            resultWriter.WriteEnd();
        }

        private void dataGrid_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var dataGrid = (DataGrid)sender;

                var pointInCell00 = new Point(dataGrid.GetCellBounds(0, 0).X + 4, dataGrid.GetCellBounds(0, 0).Y + 4);
                DataGrid.HitTestInfo hti = dataGrid.HitTest(pointInCell00);
                int row = hti.Row;

                if (row == -1)
                {
                    row = 0;
                }

                int y = dataGrid.GetCellBounds(row, 0).Top + 2;

                var currencyManager = (CurrencyManager)this.BindingContext[dataGrid.DataSource, dataGrid.DataMember];

                int yDelta = dataGrid.GetCellBounds(row, 0).Height + 1;

                while (y < dataGrid.Height - yDelta && row < currencyManager.Count)
                {
                    //get & draw the header text...  
                    string text = (row + 1).ToString();
                    e.Graphics.DrawString(text, dataGrid.Font, new SolidBrush(Color.Black), 12, y);
                    yDelta = dataGrid.GetCellBounds(row, 0).Height + 1;
                    y += yDelta;
                    row++;
                }
            }
            catch
            {
            }
        }

        private void ShowDataTableDataGrid(DataTable dataTable)
        {
            var commandBuilder = this.provider.DbProviderFactory.CreateCommandBuilder();
            var dataTableEditor = new DataTableEditor(commandBuilder);
            dataTableEditor.StatusBarPanel = this.sbPanelText;
            dataTableEditor.ReadOnly = !this.openTableMode;

            if (this.openTableMode)
            {
                string tableName = this.sqlStatement.FindTableName();
                dataTableEditor.TableName = tableName;
                DataSet dataSet = this.provider.GetTableSchema(this.connection.Connection, tableName);

                foreach (DataTable schemaTable in dataSet.Tables)
                {
                    log.Write(LogLevel.Trace, "tableSchema:\r\n{0}", schemaTable.ToStringTable());
                }

                dataTableEditor.TableSchema = dataSet;
            }

            GarbageMonitor.Add("dataTableEditor", dataTableEditor);
            dataTableEditor.StatusBarPanel = this.sbPanelText;
            dataTableEditor.DataTable = dataTable;
            this.ShowTabPage(dataTable.TableName, this.GetToolTipText(dataTable), dataTableEditor);

            // queryTextBox.Height -= 1;
            // queryTextBox.Height += 1;
        }

        private void ShowDataViewHtml(DataView dataView)
        {
            string fileName = Path.GetTempFileName();
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    int[] columnIndexes = new int[dataView.Table.Columns.Count];
                    for (int i = 0; i < columnIndexes.Length; i++)
                    {
                        columnIndexes[i] = i;
                    }
                    HtmlFormatter.Write(dataView, columnIndexes, streamWriter);
                }
            }

            DataTable dataTable = dataView.Table;
            var tabPage = new TabPage(dataTable.TableName);
            tabPage.ToolTipText = this.GetToolTipText(dataTable);
            this.tabControl.TabPages.Add(tabPage);

            var htmlTextBox = new HtmlTextBox();
            htmlTextBox.Dock = DockStyle.Fill;
            tabPage.Controls.Add(htmlTextBox);

            this.tabControl.SelectedTab = tabPage;

            htmlTextBox.Navigate(fileName);

            this.sbPanelRows.Text = dataTable.Rows.Count + " row(s).";
        }

        private void ShowDataTableRtf(DataTable dataTable)
        {
            try
            {
                this.sbPanelText.Text = "Creating Word document...";
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                var application = new ApplicationClass();
                object template = Type.Missing;
                object newTemplate = Type.Missing;
                object documentType = Type.Missing;
                object visible = Type.Missing;
                Document document = application.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
                application.Selection.Font.Name = "Tahoma";
                application.Selection.Font.Size = 8;

                Range range = application.Selection.Range;
                object defaultTableBehaviour = Type.Missing;
                object autoFitBehaviour = WdAutoFitBehavior.wdAutoFitContent;

                int numOfRows = dataTable.Rows.Count + 1;
                int numOfColumns = Math.Min(dataTable.Columns.Count, 63);

                string text = null;
                string separator = "\t";

                for (int i = 0; i < numOfColumns - 1; i++)
                {
                    text += dataTable.Columns[i].ColumnName + separator;
                }

                text += dataTable.Columns[numOfColumns - 1].ColumnName + Environment.NewLine;

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    for (int i = 0; i < numOfColumns - 1; i++)
                    {
                        text += DBValue(dataRow[i]) + separator;
                    }

                    text += DBValue(dataRow[numOfColumns - 1]) + Environment.NewLine;
                }

                application.Selection.InsertAfter(text);
                object missing = Type.Missing;
                object format = WdTableFormat.wdTableFormatList4;
                Table table = application.Selection.Range.ConvertToTable(ref missing, ref missing, ref missing, ref missing, ref format, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                table.Columns.AutoFit();

                foreach (Column column in table.Columns)
                {
                    column.Select();
                    column.AutoFit();
                }

                string fileName = Path.GetTempFileName();
                object fileNameObj = fileName;
                object fileFormat = WdSaveFormat.wdFormatRTF;

                document.SaveAs(ref fileNameObj, ref fileFormat, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing);
                document.Close(ref missing, ref missing, ref missing);
                application.Quit(ref missing, ref missing, ref missing);

                this.sbPanelText.Text = "Word document created.";
                this.sbPanelText.ForeColor = SystemColors.ControlText;


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

                Type type = (Type)dataColumn.ExtendedProperties[0];

                if (type == null)
                {
                    type = dataColumn.DataType;
                }

                columnHeader.TextAlign = QueryFormStaticMethods.GetHorizontalAlignment(type);

                listView.Columns.Add(columnHeader);
            }

            int count = dataTable.Columns.Count;
            string[] items = new string[count];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (int i = 0; i < count; i++)
                {
                    object value = dataRow[i];

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
            this.tabControl.TabPages.Add(tabPage);
            tabPage.Controls.Add(control);
            this.tabControl.SelectedTab = tabPage;
            // tabPage.Refresh();
        }

        public void ShowMessage(Exception e)
        {
            string message = this.provider.GetExceptionMessage(e);
            var infoMessage = new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Error, message);
            this.AddInfoMessage(infoMessage);

            this.tabControl.SelectedTab = this.messagesTabPage;

            this.sbPanelText.Text = "Query batch completed with errors.";
            this.sbPanelRows.Text = null;
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
                if (this.database != args.database)
                {
                    string message = string.Format(
                        "[DatabaseChanged] Database changed from {0} to {1}",
                        this.database, this.database);
                    var infoMessage = new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message);
                    this.AddInfoMessage(infoMessage);

                    this.database = args.database;
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
                    DateTime now = LocalTime.Default.Now;
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
            if (this.dataSetResultWriter != null)
            {
                DataSet dataSet = this.dataSetResultWriter.DataSet;
                this.ShowDataSet(dataSet);
            }
        }

        private void EndFill(IAsyncDataAdapter dataAdapter, Exception e)
        {
            try
            {
                if (e != null)
                {
                    this.ShowMessage(e);
                }

                if (this.connection.State == ConnectionState.Open && this.connection.Database != this.database)
                {
                    this.database = this.connection.Database;
                    this.SetText();
                }

                switch (this.resultWriterType)
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
                        this.resultSetsTabControl.TabPages.Add(resultSetTabPage);
                        this.resultSetsTabControl.SelectedTab = resultSetTabPage;
                        var tabControl = new TabControl();
                        tabControl.Dock = DockStyle.Fill;
                        int index = 0;
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
                    if (this.connection.State == ConnectionState.Closed)
                    {
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Connection is closed. Opening connection..."));

                        var csb = new SqlConnectionStringBuilder(this.connectionString);
                        csb.InitialCatalog = this.database;

                        var connectionProperties = new ConnectionProperties
                        {
                            Provider = this.provider,
                            ConnectionString = csb.ConnectionString
                        };
                        var openConnectionForm = new OpenConnectionForm(connectionProperties);
                        if (openConnectionForm.ShowDialog() == DialogResult.OK)
                        {
                            this.connection.Connection.Dispose();
                            this.connection = connectionProperties.Connection;
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
                    this.tabControl.SelectedTab = this.messagesTabPage;
                }
                else
                {
                    switch (this.resultWriterType)
                    {
                        case ResultWriterType.DataGrid:
                        case ResultWriterType.DataGridView:
                        case ResultWriterType.Html:
                        case ResultWriterType.ListView:
                        case ResultWriterType.Rtf:
                        case ResultWriterType.SQLite:
                        case ResultWriterType.Text:
                            this.tabControl.SelectedTab = this.resultSetsTabPage;
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
            this.timer.Stop();
            this.ShowTimer(3);
            this.stopwatch.Reset();

            if (this.cancel)
            {
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Query was cancelled by user."));
                this.sbPanelText.Text = "Query canceled.";
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                this.cancel = false;
            }
            else
            {
                if (this.errorCount == 0)
                {
                    this.sbPanelText.ForeColor = SystemColors.ControlText;
                    this.sbPanelText.Text = "Query completed.";
                }
                else
                {
                    this.sbPanelText.ForeColor = Color.Red;
                    this.sbPanelText.Text = "Query completed with errors.";
                }
            }

            this.dataAdapter = null;
            this.SetGui(CommandState.Execute);
            this.FocusControl(this.queryTextBox);
            this.Cursor = Cursors.Default;
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
            this.queryTextBox.Focus();
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }

        private void SetGui(CommandState buttonState)
        {
            bool ok = (buttonState & CommandState.Execute) != 0;
            bool cancel = (buttonState & CommandState.Cancel) != 0;

            this.mnuCancel.Enabled = cancel;

            this.buttonState = buttonState;

            this.executeQueryToolStripMenuItem.Enabled = ok;
            this.executeQueryMenuItem.Enabled = ok;
            this.mnuExecuteQuerySingleRow.Enabled = ok;
            this.mnuExecuteQuerySchemaOnly.Enabled = ok;
            this.mnuExecuteQueryKeyInfo.Enabled = ok;
            this.mnuExecuteQueryXml.Enabled = ok;

            log.Write(LogLevel.Trace, "this.execueQuerySplitButton.Enabled = {0};", ok);
            this.execueQuerySplitButton.Enabled = ok;
            this.cancelQueryButton.Enabled = cancel;
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

            log.Write(logLevel, infoMessage.Message);
        }

        private void ConsumeInfoMessages()
        {
            var waitHandles = new WaitHandle[]
            {
                this.enqueueEvent,
                this.cancellationTokenSource.Token.WaitHandle
            };

            while (true)
            {
                bool hasElements = false;
                while (this.infoMessages.Count > 0 && this.IsHandleCreated)
                {
                    hasElements = true;
                    var infoMessages = new InfoMessage[this.infoMessages.Count];
                    int count = this.infoMessages.Take(infoMessages);
                    var sb = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        this.Invoke(() =>
                        {
                            var message = infoMessages[i];
                            Color color = this.messagesTextBox.SelectionColor;

                            switch (message.Severity)
                            {
                                case InfoMessageSeverity.Error:
                                    this.messagesTextBox.SelectionColor = Color.Red;
                                    break;

                                case InfoMessageSeverity.Information:
                                    this.messagesTextBox.SelectionColor = Color.Blue;
                                    break;
                            }

                            this.AppendMessageText(message.CreationTime, message.Severity, message.Message);

                            switch (message.Severity)
                            {
                                case InfoMessageSeverity.Error:
                                case InfoMessageSeverity.Information:
                                    this.messagesTextBox.SelectionColor = color;
                                    break;
                            }
                        });
                    }
                }

                if (hasElements)
                {
                    this.Invoke(() =>
                    {
                        this.messagesTextBox.ScrollToCaret();
                        this.messagesTextBox.Update();
                    });
                }

                if (this.infoMessages.Count == 0)
                {
                    int w = WaitHandle.WaitAny(waitHandles, 1000);
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
            int charIndex = richTextBox.SelectionStart;
            int line = richTextBox.GetLineFromCharIndex(charIndex) + 1;
            int lineIndex = QueryTextBox.GetLineIndex(richTextBox, -1);
            int col = charIndex - lineIndex + 1;
            this.sbPanelCaretPosition.Text = "Ln " + line + " Col " + col;
        }

        private void mnuExecuteQuery_Click(object sender, EventArgs e)
        {
            if (this.Query.Length > 0)
            {
                this.ExecuteQuery();
            }
        }

        private void mnuExecuteQueryF5_Click(object sender, EventArgs e)
        {
            if (this.Query.Length > 0)
            {
                this.ExecuteQuery();
            }
        }

        private void AddTable(
            OleDbConnection oleDbConnection,
            DataSet dataSet,
            Guid guid,
            string name)
        {
            try
            {
                DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(guid, null);
                dataTable.TableName = name;
                dataSet.Tables.Add(dataTable);
            }
            catch (Exception e)
            {
                this.messagesTextBox.Text += e.ToString();
            }
        }

        private void mnuDescribeParameters_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var oldDbConnection = this.connection.Connection as OleDbConnection;

                if (oldDbConnection != null && string.IsNullOrEmpty(this.Query))
                {
                    var dataSet = new DataSet();
                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Provider_Types, "Provider Types");
                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.DbInfoLiterals, "DbInfoLiterals");

                    var c2 = new ConnectionClass();
                    c2.Open(this.connectionString, null, null, 0);
                    Recordset rs = c2.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                    DataTable dataTable = OleDBHelper.Convert(rs);
                    c2.Close();
                    dataSet.Tables.Add(dataTable);

                    this.AddTable(oldDbConnection, dataSet, OleDbSchemaGuid.Sql_Languages, "Sql Languages");
                    this.ShowDataSet(dataSet);
                }
                else
                {
                    this.sqlStatement = new SqlStatement(this.Query);
                    this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);

                    if (this.command != null)
                    {
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, this.command.ToLogString()));
                        DataTable dataTable = this.provider.GetParameterTable(this.command.Parameters);

                        if (dataTable != null)
                        {
                            dataTable.TableName = "Parameters";

                            foreach (DataRow row in dataTable.Rows)
                            {
                                object value = row["Value"];
                                Type type = value.GetType();
                                TypeCode typeCode = Type.GetTypeCode(type);

                                switch (typeCode)
                                {
                                    case TypeCode.DateTime:
                                        const long TicksPerMillisecond = 10000;
                                        const long TicksPerSecond = TicksPerMillisecond*1000;
                                        const long TicksPerMinute = TicksPerSecond*60;
                                        const long TicksPerHour = TicksPerMinute*60;
                                        const long TicksPerDay = TicksPerHour*24;

                                        DateTime dateTime = (DateTime)value;
                                        long ticks = dateTime.Ticks;

                                        if (ticks/TicksPerDay*TicksPerDay == ticks)
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
            var tabPage = this.tabControl.SelectedTab;
            if (tabPage != null && tabPage != this.messagesTabPage && tabPage != this.resultSetsTabPage)
            {
                this.CloseResultSetTabPage(tabPage);
            }
        }

        private void CloseResultSetTabPages()
        {
            var tabPages = this.resultSetsTabControl.TabPages.Cast<TabPage>().ToArray();
            foreach (var tabPage in tabPages)
            {
                this.CloseResultSetTabPage(tabPage);
            }
            this.resultSetCount = 0;
        }

        private void mnuCloseAllTabPages_Click(object sender, EventArgs e)
        {
            this.CloseResultSetTabPages();

            this.tabControl.SelectedTab = this.messagesTabPage;
            this.messagesTextBox.Clear();
            this.sbPanelText.Text = null;
            this.sbPanelText.ForeColor = SystemColors.ControlText;

            if (this.dataAdapter == null)
            {
                this.sbPanelRows.Text = null;
                this.sbPanelTimer.Text = null;
            }

            this.Invoke(() => this.FocusControl(this.queryTextBox));
        }

        public void CancelQuery()
        {
            log.Trace(ThreadMonitor.ToStringTable().ToString());
            const string message = "Cancelling command...";
            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, message));
            this.sbPanelText.Text = "Cancel Executing Query...";
            this.sbPanelText.ForeColor = SystemColors.ControlText;
            this.cancel = true;
            this.SetGui(CommandState.None);
            this.dataAdapter.Cancel();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.CancelQuery();
        }

        private void ShowTimer(int scale)
        {
            long ticks = this.stopwatch.ElapsedTicks;
            this.sbPanelTimer.Text = StopwatchTimeSpan.ToString(ticks, scale);

            if (this.dataAdapter != null)
            {
                long rowCount = this.dataAdapter.RowCount;
                string text = rowCount.ToString() + " row(s).";

                if (rowCount > 0)
                {
                    double seconds = (double)ticks/Stopwatch.Frequency;

                    text += " (" + Math.Round(rowCount/seconds, 0) + " rows/sec)";
                }

                this.sbPanelRows.Text = text;
            }
        }

        private void Timer_Tick(object o, EventArgs e)
        {
            this.Invoke(() => this.ShowTimer(0));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            string text = this.queryTextBox.RichTextBox.Text;
            if (this.connection != null)
            {
                log.Write(LogLevel.Trace, "Saving text before closing form(connectionName: {0}):\r\n{1}", this.connection.ConnectionName, text);
            }

            if (this.dataAdapter == null)
            {
                bool hasTransactions;
                if (this.transaction != null)
                {
                    hasTransactions = true;
                }
                else if (this.connection != null && this.connection.State == ConnectionState.Open)
                {
                    try
                    {
                        hasTransactions = this.connection.TransactionCount > 0;
                    }
                    catch (Exception ex)
                    {
                        string message = this.provider.GetExceptionMessage(ex);
                        Color color = this.messagesTextBox.SelectionColor;
                        this.messagesTextBox.SelectionColor = Color.Red;
                        this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, message));
                        this.messagesTextBox.SelectionColor = color;
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
                    string caption = Application.Instance.Name;
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
                    int length = this.queryTextBox.Text.Length;

                    if (length > 0)
                    {
                        text = string.Format("The text in {0} has been changed.\r\nDo you want to save the changes?", this.Text);
                        string caption = Application.Instance.Name;
                        DialogResult result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                        switch (result)
                        {
                            case DialogResult.Yes:
                                if (this.fileName != null)
                                {
                                    this.Save(this.fileName);
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
                string caption = Application.Instance.Name;
                DialogResult result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    this.CancelQuery();
                    this.timer.Enabled = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                // this.CloseResultSetTabPages();
                // this.tabControl.TabPages.Clear();

                this.cancellationTokenSource.Cancel();

                if (this.connection != null)
                {
                    string dataSource = this.connection.DataSource;
                    this.parentStatusBar.Items[0].Text = "Closing connection to DataSource " + dataSource + "....";
                    this.connection.Close();
                    this.parentStatusBar.Items[0].Text = "Connection to DataSource " + dataSource + " closed.";
                    this.connection.Connection.Dispose();
                    this.connection = null;
                }

                if (this.toolStrip != null)
                {
                    this.toolStrip.Dispose();
                    this.toolStrip = null;
                }
            }
        }

        private void SetResultWriterType(ResultWriterType tableStyle)
        {
            this.resultWriterType = tableStyle;
            this.sbPanelTableStyle.Text = tableStyle.ToString();
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
            this.mnuCommandTypeText.Checked = true;
            this.mnuCommandTypeStoredProcedure.Checked = false;
            this.commandType = CommandType.Text;
        }

        private void mnuCommandTypeStoredProcedure_Click(object sender, EventArgs e)
        {
            this.mnuCommandTypeText.Checked = false;
            this.mnuCommandTypeStoredProcedure.Checked = true;
            this.commandType = CommandType.StoredProcedure;
        }

        private void menuObjectExplorer_Click(object sender, EventArgs e)
        {
            bool visible = !this.tvObjectExplorer.Visible;
            this.tvObjectExplorer.Visible = visible;
            this.splitterObjectExplorer.Visible = visible;
        }

        private void tvObjectBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode treeNode = e.Node;

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
                    int count = treeNode.GetNodeCount(false);
                    this.sbPanelText.Text = treeNode.Text + " node has " + count + " children.";
                    this.sbPanelText.ForeColor = SystemColors.ControlText;
                }
            }
        }

        private void tvObjectBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                    TreeNode treeNode = this.tvObjectExplorer.GetNodeAt(e.X, e.Y);

                    if (treeNode != null)
                    {
                        var treeNode2 = (ITreeNode)treeNode.Tag;

                        if (e.Button != MouseButtons.Left)
                        {
                            this.tvObjectExplorer.SelectedNode = treeNode;
                        }

                        string text = treeNode.Text;
                    }

                    break;

                default:
                    break;
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            TreeNode treeNodeV = this.tvObjectExplorer.SelectedNode;

            if (treeNodeV != null)
            {
                var treeNode = (ITreeNode)treeNodeV.Tag;
                treeNodeV.Nodes.Clear();
                this.AddNodes(treeNodeV.Nodes, treeNode.GetChildren(true), treeNode.Sortable);
            }
        }

        private void mnuRefreshObjectExplorer_Click(object sender, EventArgs e)
        {
            IObjectExplorer objectExplorer = this.provider.ObjectExplorer;

            if (objectExplorer != null)
            {
                using (new CursorManager(Cursors.WaitCursor))
                {
                    TreeNodeCollection rootNodes = this.tvObjectExplorer.Nodes;
                    rootNodes.Clear();
                    IEnumerable<ITreeNode> treeNodes = objectExplorer.GetChildren(true);
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
                    TreeNode treeNodeV = this.tvObjectExplorer.SelectedNode;

                    if (treeNodeV != null)
                    {
                        var treeNode = (ITreeNode)treeNodeV.Tag;
                        var contextMenu = treeNode.ContextMenu;

                        if (!treeNode.IsLeaf)
                        {
                            if (contextMenu == null)
                            {
                                contextMenu = new ContextMenuStrip(this.components);
                            }

                            contextMenu.Items.Add(new ToolStripMenuItem("Refresh", null, this.mnuRefresh_Click));
                        }

                        if (contextMenu != null)
                        {
                            bool contains = this.components.Components.Cast<IComponent>().Contains(contextMenu);
                            if (!contains)
                            {
                                this.components.Add(contextMenu);
                                GarbageMonitor.Add("contextMenu", contextMenu);
                            }
                            var pos = new Point(e.X, e.Y);
                            contextMenu.Show(this.tvObjectExplorer, pos);
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
            TreeNode selectedNode = this.tvObjectExplorer.SelectedNode;

            if (selectedNode != null)
            {
                var treeNode = (ITreeNode)selectedNode.Tag;

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    string query = treeNode.Query;

                    if (query != null)
                    {
                        string text0 = this.queryTextBox.Text;
                        string append = null;
                        int selectionStart = this.queryTextBox.RichTextBox.TextLength;

                        if (!string.IsNullOrEmpty(text0))
                        {
                            append = Environment.NewLine + Environment.NewLine;
                            selectionStart += 2;
                        }

                        append += query;

                        this.queryTextBox.RichTextBox.AppendText(append);
                        this.queryTextBox.RichTextBox.SelectionStart = selectionStart;
                        this.queryTextBox.RichTextBox.SelectionLength = query.Length;

                        this.queryTextBox.Focus();
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
            this.queryTextBox.Paste();
        }

        private void mnuGoTo_Click(object sender, EventArgs e)
        {
            Control control = this.ActiveControl;
            var richTextBox = control as RichTextBox;

            if (richTextBox == null)
            {
                richTextBox = this.queryTextBox.RichTextBox;
            }

            int charIndex = richTextBox.SelectionStart;
            int currentLineNumber = richTextBox.GetLineFromCharIndex(charIndex) + 1;
            var form = new GotoLineForm();
            int maxLineNumber = richTextBox.Lines.Length;
            form.Init(currentLineNumber, maxLineNumber);

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                int lineNumber = form.LineNumber;
                charIndex = NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int)NativeMethods.Message.EditBox.LineIndex, lineNumber - 1, 0);
                richTextBox.SelectionStart = charIndex;
            }
        }

        private TreeNode FindTreeNode(
            TreeNode parent,
            IStringMatcher matcher)
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
            bool found = false;
            Control control = this.ActiveControl;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.sbPanelText.Text = string.Format("Finding {0}...", text);
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                StringComparison comparison;
                var options = this.findTextForm.RichTextBoxFinds;
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
                    TreeNode treeNode2 = treeView.SelectedNode.FirstNode;

                    if (treeNode2 == null || treeNode2.Tag == null)
                    {
                        treeNode2 = treeView.SelectedNode.NextNode;
                    }

                    TreeNode treeNode = treeNode2;

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
                        DataTable dataTable = dataTableViewer.DataTable;

                        if (dataTable != null)
                        {
                            if (text.StartsWith("RowFilter="))
                            {
                                string rowFilter = text.Substring(5);
                                DataView dataView = dataTable.DefaultView;
                                dataView.RowFilter = rowFilter;
                                int count = dataView.Count;
                                found = count > 0;
                                this.sbPanelText.Text = string.Format("{0} rows found. RowFilter: {1}", count, rowFilter);
                                this.sbPanelText.ForeColor = SystemColors.ControlText;
                            }
                            else if (text.StartsWith("Sort="))
                            {
                                string sort = text.Substring(5);
                                DataView dataView = dataTable.DefaultView;
                                dataView.Sort = sort;
                                this.sbPanelText.Text = string.Format("Rows sorted by {0}.", sort);
                                this.sbPanelText.ForeColor = SystemColors.ControlText;
                            }
                            else
                            {
                                DataGridView dataGrid = dataTableViewer.DataGrid;
                                DataGridViewCell cell = dataGrid.CurrentCell;
                                int rowIndex = cell.RowIndex;
                                int columnIndex = cell.ColumnIndex;
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
                            richTextBox = this.queryTextBox.RichTextBox;
                        }

                        int start = richTextBox.SelectionStart + richTextBox.SelectionLength;
                        int location = richTextBox.Find(text, start, options);
                        found = location >= 0;
                    }
                }
            }
            finally
            {
                this.sbPanelText.Text = null;
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                this.Cursor = Cursors.Default;
            }

            if (!found)
            {
                string message = string.Format("The specified text was not found.\r\n\r\nText: {0}\r\nControl: {1}", text, control.Name);
                MessageBox.Show(this, message, Application.Instance.Name);
            }
        }

        private void mnuFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.findTextForm == null)
                {
                    this.findTextForm = new FindTextForm();
                }

                Control control = this.ActiveControl;
                var dataTableViewer = control as DataTableEditor;

                if (dataTableViewer == null)
                {
                    control = control.Parent;
                    dataTableViewer = control as DataTableEditor;
                }

                if (dataTableViewer != null)
                {
                    var dataTable = dataTableViewer.DataTable;
                    string name = dataTable.TableName;
                    this.findTextForm.Text = string.Format("Find (DataTable: {0})", name);
                }
                else
                {
                    this.findTextForm.Text = "Find";
                }

                if (this.findTextForm.ShowDialog() == DialogResult.OK)
                {
                    this.FindText(this.findTextForm.FindText);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex);
            }
        }

        private void mnuFindNext_Click(object sender, EventArgs e)
        {
            if (this.findTextForm != null)
            {
                string text = this.findTextForm.FindText;

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
                this.sbPanelText.Text = string.Format("Saving file {0}...", fileName);
                this.sbPanelText.ForeColor = SystemColors.ControlText;

                const RichTextBoxStreamType type = RichTextBoxStreamType.UnicodePlainText;
                Encoding encoding;

                switch (type)
                {
                    case RichTextBoxStreamType.PlainText:
                    default:
                        encoding = Encoding.ASCII;
                        break;

                    case RichTextBoxStreamType.UnicodePlainText:
                        encoding = Encoding.Unicode;
                        break;
                }

                using (FileStream stream = File.Create(fileName))
                {
                    byte[] preamble = encoding.GetPreamble();
                    stream.Write(preamble, 0, preamble.Length);
                    this.queryTextBox.RichTextBox.SaveFile(stream, type);
                }

                this.fileName = fileName;
                this.SetText();
                this.sbPanelText.Text = string.Format("File {0} saved successfully.", fileName);
                this.sbPanelText.ForeColor = SystemColors.ControlText;
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
                string fileName = saveFileDialog.FileName;
                this.Save(fileName);
            }
        }

        public void Save()
        {
            if (this.fileName != null)
            {
                this.Save(this.fileName);
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
            log.Write(LogLevel.Trace, "GarbageMonitor.State:\r\n" + GarbageMonitor.State);
            this.queryTextBox.Select();
        }

        private void mnuGotoMessageTabPage_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectedTab = this.messagesTabPage;
        }

        private GetCompletionResponse GetCompletion()
        {
            RichTextBox textBox = this.queryTextBox.RichTextBox;
            string text = textBox.Text;
            int position = textBox.SelectionStart;

            long ticks = Stopwatch.GetTimestamp();
            var response = this.provider.GetCompletion(this.connection, this.transaction, text, position);
            string from = response.FromCache ? "cache" : "data source";
            ticks = Stopwatch.GetTimestamp() - ticks;
            int length = response.Items != null ? response.Items.Count : 0;
            this.sbPanelText.Text = string.Format("GetCompletion returned {0} items from {1} in {2} seconds.", length, from, StopwatchTimeSpan.ToString(ticks, 3));
            this.sbPanelText.ForeColor = SystemColors.ControlText;
            return response;
        }

        private CompletionForm completionForm;

        private void mnuListMembers_Click(object sender, EventArgs e)
        {
            if (this.queryTextBox.KeyboardHandler == null)
            {
                using (new CursorManager(Cursors.WaitCursor))
                {
                    var response = this.GetCompletion();
                    if (response.Items != null)
                    {
                        this.completionForm = new CompletionForm(this);
                        this.completionForm.Initialize(this.queryTextBox, response);
                        this.completionForm.ItemSelected += new EventHandler<ItemSelectedEventArgs>(this.completionForm_ItemSelected);
                        this.completionForm.Show();
                        this.queryTextBox.RichTextBox.Focus();
                    }
                }
            }
        }

        private void completionForm_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            var textBox = this.queryTextBox;

            IntPtr intPtr = textBox.RichTextBox.Handle;
            int hWnd = intPtr.ToInt32();
            NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

            string objectName = e.ObjectName.QuotedName;

            textBox.RichTextBox.SelectionStart = e.StartIndex;
            textBox.RichTextBox.SelectionLength = e.Length;
            textBox.RichTextBox.SelectedText = objectName;
            textBox.RichTextBox.SelectionStart = e.StartIndex + objectName.Length;

            NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
        }

        internal void OnCompletionFormClosed()
        {
            this.completionForm = null;
        }

        private void mnuClearCache_Click(object sender, EventArgs e)
        {
            this.provider.ClearCompletionCache();
        }

        private void ExecuteReader(CommandBehavior commandBehavior)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.sqlStatement = new SqlStatement(this.Query);
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);

                if (this.command != null)
                {
                    IDataReader dataReader = null;

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                dataReader = this.command.ExecuteReader(commandBehavior);
                                break;
                            }
                            catch
                            {
                                if (this.connection.State != ConnectionState.Open)
                                {
                                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Opening connection..."));
                                    this.connection.Open();
                                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Connection opened successfully."));
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        DataSet dataSet = null;
                        int i = 1;

                        do
                        {
                            DataTable dataTable = this.provider.GetSchemaTable(dataReader);

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
                this.sqlStatement = new SqlStatement(this.Query);
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
                var dataSet = new DataSet();
                using (var dataReader = this.command.ExecuteReader())
                {
                    do
                    {
                        DataTable schemaTable = this.provider.GetSchemaTable(dataReader);
                        IDataReaderHelper dataReaderHelper = this.provider.CreateDataReaderHelper(dataReader);
                        int rowCount = 0;

                        while (dataReader.Read())
                        {
                            rowCount++;
                            object[] values = new object[dataReader.FieldCount];
                            dataReaderHelper.GetValues(values);

                            var dataTable = new DataTable("SingleRow(" + rowCount + ")");
                            dataTable.Columns.Add(" ", typeof (int));
                            dataTable.Columns.Add("Name", typeof (string));
                            dataTable.Columns.Add("Value");
                            int count = schemaTable.Rows.Count;

                            for (int i = 0; i < count; i++)
                            {
                                DataRow schemaRow = schemaTable.Rows[i];
                                string columnName = schemaRow["Name"].ToString();

                                DataRow dataRow = dataTable.NewRow();
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
            this.mnuShowShemaTable.Checked = !this.mnuShowShemaTable.Checked;
            this.showSchemaTable = !this.showSchemaTable;
        }

        private void mnuXml_Click(object sender, EventArgs e)
        {
            try
            {
                this.sqlStatement = new SqlStatement(this.Query);
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);

                using (var dataReader = this.command.ExecuteReader())
                {
                    while (true)
                    {
                        var writer = new StringWriter();
                        bool read = false;

                        while (dataReader.Read())
                        {
                            if (!read)
                            {
                                read = true;
                            }

                            string fragment = (string)dataReader[0];
                            writer.Write(fragment);
                        }

                        if (read)
                        {
                            string xml = writer.ToString();
                            var xmlDocument = new XmlDocument();
                            string path = Path.GetTempFileName() + ".xml";

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
                            this.resultSetsTabControl.TabPages.Add(resultSetTabPage);

                            var htmlTextBox = new HtmlTextBox();
                            htmlTextBox.Dock = DockStyle.Fill;

                            resultSetTabPage.Controls.Add(htmlTextBox);

                            htmlTextBox.Navigate(path);
                            this.resultSetsTabControl.SelectedTab = resultSetTabPage;
                            this.tabControl.SelectedTab = this.resultSetsTabPage;
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
                var textWriter = this.standardOutput.TextWriter;
                switch (this.resultWriterType)
                {
                }

                this.sqlStatement = new SqlStatement(this.Query);
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, CommandType.Text, this.commandTimeout);
                string tableName = this.sqlStatement.FindTableName();

                using (var dataReader = this.command.ExecuteReader())
                {
                    var dataReaderHelper = this.provider.CreateDataReaderHelper(dataReader);
                    DataTable schemaTable = dataReader.GetSchemaTable();

                    if (tableName != null)
                    {
                        schemaTable.TableName = tableName;
                    }
                    else
                    {
                        tableName = schemaTable.TableName;
                    }

                    this.standardOutput.WriteLine(InsertScriptFileWriter.GetCreateTableStatement(schemaTable));
                    DataRowCollection schemaRows = schemaTable.Rows;
                    int columnCount = schemaRows.Count;
                    var sb = new StringBuilder();
                    sb.AppendFormat("insert into {0}(", tableName);

                    for (int i = 0; i < columnCount; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }

                        DataRow schemaRow = schemaRows[i];
                        string columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                        sb.Append(columnName);
                    }

                    sb.Append(") values(");
                    string insertInto = sb.ToString();
                    int fieldCount = dataReader.FieldCount;
                    sb.Length = 0;
                    int statementCount = 0;

                    while (dataReader.Read())
                    {
                        object[] values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb.Append(insertInto);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                            {
                                sb.Append(',');
                            }

                            string s = InsertScriptFileWriter.ToString(values[i]);
                            sb.Append(s);
                        }

                        sb.AppendLine(");");
                        statementCount++;

                        if (statementCount%100 == 0)
                        {
                            this.standardOutput.Write(sb);
                            sb.Length = 0;
                        }
                    }

                    if (statementCount%100 != 0)
                    {
                        this.standardOutput.Write(sb);
                    }
                }

                this.tabControl.SelectedTab = this.messagesTabPage;

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
                this.sqlStatement = new SqlStatement(this.Query);
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, CommandType.Text, this.commandTimeout);
                string tableName = this.sqlStatement.FindTableName();

                if (tableName != null)
                {
                    using (IDataReader dataReader = this.command.ExecuteReader())
                    {
                        IDataReaderHelper dataReaderHelper = this.provider.CreateDataReaderHelper(dataReader);
                        DataTable schemaTable = dataReader.GetSchemaTable();
                        DataRowCollection schemaRows = schemaTable.Rows;
                        int columnCount = schemaRows.Count;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("insert into {0}(", tableName);

                        for (int i = 0; i < columnCount; i++)
                        {
                            if (i > 0)
                            {
                                sb.Append(',');
                            }

                            DataRow schemaRow = schemaRows[i];
                            string columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                            sb.Append(columnName);
                        }

                        sb.Append(")\r\nselect");
                        string insertInto = sb.ToString();
                        int fieldCount = dataReader.FieldCount;

                        while (dataReader.Read())
                        {
                            var values = new object[fieldCount];
                            dataReaderHelper.GetValues(values);
                            sb = new StringBuilder();
                            sb.Append(insertInto);

                            for (int i = 0; i < fieldCount; i++)
                            {
                                sb.Append('\t');

                                if (i == 0)
                                {
                                    sb.Append(' ');
                                }
                                {
                                    sb.Append(',');
                                }

                                string s = InsertScriptFileWriter.ToString(values[i]);
                                sb.AppendFormat("{0}\t\tas {1}", s, dataReader.GetName(i));
                            }

                            this.standardOutput.WriteLine(sb);
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
                log.Write(LogLevel.Trace, "reader.CurrentEncoding.EncodingName: {0}", reader.CurrentEncoding.EncodingName);
                text = reader.ReadToEnd();
            }

            this.queryTextBox.Text = text;
            this.fileName = path;
            this.SetText();
            this.sbPanelText.Text = string.Format("File {0} loaded successfully.", this.fileName);
            this.sbPanelText.ForeColor = SystemColors.ControlText;
        }

        private void tvObjectBrowser_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var treeNode = (TreeNode)e.Item;
            var treeNode2 = (ITreeNode)treeNode.Tag;
            string text = treeNode.Text;
            this.tvObjectExplorer.DoDragDrop(text, DragDropEffects.All);
        }

        private void mnuDuplicateConnection_Click(object sender, EventArgs e)
        {
            var mainForm = Application.Instance.MainForm;
            int index = mainForm.MdiChildren.Length;

            var connection = this.provider.CreateConnection(this.connectionString);
            connection.ConnectionName = this.connection.ConnectionName;
            connection.Open();
            string database = this.Connection.Database;

            if (connection.Database != this.Connection.Database)
            {
                connection.Connection.ChangeDatabase(database);
            }

            var queryForm = new QueryForm(index, this.provider, this.connectionString, connection, mainForm.StatusBar);
            queryForm.Font = mainForm.SelectedFont;
            queryForm.MdiParent = mainForm;
            queryForm.WindowState = this.WindowState;
            queryForm.Show();
        }

        public void OpenTable(string query)
        {
            try
            {
                log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
                this.sqlStatement = new SqlStatement(query);
                this.commandType = CommandType.Text;
                this.openTableMode = true;
                this.command = this.sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
                this.dataAdapter = new AsyncDataAdapter();
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Executing query..."));
                this.stopwatch.Start();
                this.timer.Start();
                const int maxRecords = int.MaxValue;
                this.dataSetResultWriter = new DataSetResultWriter(this.AddInfoMessage, this, this.showSchemaTable);
                IResultWriter resultWriter = this.dataSetResultWriter;
                this.dataAdapter.BeginFill(this.provider, this.command.ItemAsEnumerable(), maxRecords, this.rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            catch (Exception ex)
            {
                this.EndFill(this.dataAdapter, ex);
            }
        }

        private void sQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetResultWriterType(ResultWriterType.SQLite);
        }

        private void createSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sqlStatement = new SqlStatement(this.Query);
            this.command = sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
            IAsyncDataAdapter asyncDataAdatper = new AsyncDataAdapter();
            int maxRecords = int.MaxValue;
            string tableName = sqlStatement.FindTableName();
            var sqlCeResultWriter = new SqlCeResultWriter(this.textBoxWriter, tableName);
            asyncDataAdatper.BeginFill(this.provider, this.command.ItemAsEnumerable(), maxRecords, this.rowBlockSize, sqlCeResultWriter, this.EndFillInvoker, this.WriteEndInvoker);
        }

        private void SetTransaction(IDbTransaction transaction)
        {
            if (this.transaction == null && transaction != null)
            {
                this.transaction = transaction;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction created successfully."));
                this.tabControl.SelectedTab = this.messagesTabPage;
                this.beginTransactionToolStripMenuItem.Enabled = false;
                this.commitTransactionToolStripMenuItem.Enabled = true;
                this.rollbackTransactionToolStripMenuItem.Enabled = true;
            }
        }

        private void InvokeSetTransaction(IDbTransaction transaction)
        {
            this.Invoke(new Action<IDbTransaction>(this.SetTransaction), transaction);
        }

        private void beginTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.transaction == null)
            {
                IDbTransaction transaction = this.connection.Connection.BeginTransaction();
                this.SetTransaction(transaction);
            }
        }

        private void commitTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.transaction != null)
            {
                this.transaction.Commit();
                this.transaction.Dispose();
                this.transaction = null;

                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction commited successfully."));

                this.tabControl.SelectedTab = this.messagesTabPage;
                this.beginTransactionToolStripMenuItem.Enabled = true;
                this.commitTransactionToolStripMenuItem.Enabled = false;
                this.rollbackTransactionToolStripMenuItem.Enabled = false;
            }
        }

        private void rollbackTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.transaction != null)
            {
                try
                {
                    this.transaction.Rollback();
                    this.transaction.Dispose();
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Transaction rolled back successfully."));
                }
                catch (Exception ex)
                {
                    string message = string.Format("Rollback failed. Exception:\r\n{0}", ex.ToLogString());
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Error, message));
                }

                this.transaction = null;
                this.tabControl.SelectedTab = this.messagesTabPage;
                this.beginTransactionToolStripMenuItem.Enabled = true;
                this.commitTransactionToolStripMenuItem.Enabled = false;
                this.rollbackTransactionToolStripMenuItem.Enabled = false;
            }
        }

        internal void ScriptQueryAsCreateTable()
        {
            SqlStatement sqlStatement = new SqlStatement(this.Query);
            IDbCommand command = sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);

            Form[] forms = Application.Instance.MainForm.MdiChildren;
            int index = Array.IndexOf(forms, this);
            IProvider destinationProvider;

            if (index < forms.Length - 1)
            {
                QueryForm nextQueryForm = (QueryForm)forms[index + 1];
                destinationProvider = nextQueryForm.provider;
            }
            else
            {
                destinationProvider = this.provider;
            }

            DataTable schemaTable;
            string[] dataTypeNames;

            using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                schemaTable = dataReader.GetSchemaTable();
                dataTypeNames = new string[dataReader.FieldCount];

                for (int i = 0; i < dataReader.FieldCount; i++)
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
            StringBuilder createTable = new StringBuilder();
            createTable.AppendFormat("create table [{0}]\r\n(\r\n", tableName);
            StringTable stringTable = new StringTable(3);
            int last = schemaTable.Rows.Count - 1;

            for (int i = 0; i <= last; i++)
            {
                DataRow dataRow = schemaTable.Rows[i];
                DataColumnSchema schemaRow = new DataColumnSchema(dataRow);
                StringTableRow row = stringTable.NewRow();
                string typeName = destinationProvider.GetColumnTypeName(this.provider, dataRow, dataTypeNames[i]);
                row[1] = schemaRow.ColumnName;
                row[2] = typeName;
                bool? allowDBNull = schemaRow.AllowDBNull;

                if (allowDBNull == false)
                {
                    row[2] += " not null";
                }

                if (i < last)
                {
                    row[2] += ',';
                }

                stringTable.Rows.Add(row);
            }

            StringWriter stringWriter = new StringWriter();
            stringTable.Write(stringWriter, 4);
            createTable.Append(stringWriter.ToString());
            createTable.Append(')');
            string commandText = createTable.ToString();

            this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "\r\n" + commandText));
        }

        internal void CopyTable()
        {
            var forms = Application.Instance.MainForm.MdiChildren;
            int index = Array.IndexOf(forms, this);
            if (index < forms.Length - 1)
            {
                var nextQueryForm = (QueryForm)forms[index + 1];
                var destinationProvider = nextQueryForm.provider;
                var destinationConnection = nextQueryForm.connection;
                var sqlStatement = new SqlStatement(this.Query);
                this.command = sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
                string tableName;
                if (this.command.CommandType == CommandType.StoredProcedure)
                {
                    tableName = this.command.CommandText;
                }
                else
                {
                    tableName = sqlStatement.FindTableName();
                }

                if (tableName[0] == '[' && destinationProvider.Name == "System.Data.OracleClient")
                {
                    tableName = tableName.Substring(1, tableName.Length - 2);
                }

                IResultWriter resultWriter = new CopyResultWriter(this.AddInfoMessage, destinationProvider, destinationConnection, tableName, nextQueryForm.InvokeSetTransaction);
                int maxRecords = int.MaxValue;
                int rowBlockSize = 10000;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, "Copying table..."));
                this.sbPanelText.Text = "Copying table...";
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                this.SetGui(CommandState.Cancel);
                this.errorCount = 0;
                this.stopwatch.Start();
                this.timer.Start();
                this.dataAdapter = new AsyncDataAdapter();
                this.dataAdapter.BeginFill(this.provider, this.command.ItemAsEnumerable(), maxRecords, rowBlockSize, resultWriter, this.EndFillInvoker, this.WriteEndInvoker);
            }
            else
            {
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Please open a destination connection."));
            }
        }

        internal void CopyTableWithSqlBulkCopy()
        {
            var forms = Application.Instance.MainForm.MdiChildren;
            int index = Array.IndexOf(forms, this);
            if (index < forms.Length - 1)
            {
                var nextQueryForm = (QueryForm)forms[index + 1];
                var destinationProvider = nextQueryForm.provider;
                var destinationConnection = (SqlConnection)nextQueryForm.connection.Connection;
                var destionationTransaction = (SqlTransaction)nextQueryForm.transaction;
                var sqlStatement = new SqlStatement(this.Query);
                this.command = sqlStatement.CreateCommand(this.provider, this.connection, this.commandType, this.commandTimeout);
                string tableName;
                if (this.command.CommandType == CommandType.StoredProcedure)
                {
                    tableName = this.command.CommandText;
                }
                else
                {
                    tableName = sqlStatement.FindTableName();
                }

                //IResultWriter resultWriter = new SqlBulkCopyResultWriter( this.AddInfoMessage, destinationProvider, destinationConnection, tableName, nextQueryForm.InvokeSetTransaction );
                int maxRecords = int.MaxValue;
                int rowBlockSize = 10000;
                this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, "Copying table..."));
                this.sbPanelText.Text = "Copying table...";
                this.sbPanelText.ForeColor = SystemColors.ControlText;
                this.SetGui(CommandState.Cancel);
                this.errorCount = 0;
                this.stopwatch.Start();
                this.timer.Start();
                this.dataAdapter = new SqlBulkCopyAsyncDataAdapter(destinationConnection, destionationTransaction, tableName, this.AddInfoMessage);
                this.dataAdapter.BeginFill(this.provider, this.command.ItemAsEnumerable(), maxRecords, rowBlockSize, null, this.EndFillInvoker, this.WriteEndInvoker);
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
            ResultWriterType tableStyle = (ResultWriterType)item.Tag;
            this.SetResultWriterType(tableStyle);
        }

        public static void ShowText(string text)
        {
            var mainForm = Application.Instance.MainForm;
            mainForm.Cursor = Cursors.WaitCursor;
            var queryForm = (QueryForm)mainForm.ActiveMdiChild;

            try
            {
                var queryTextBox = queryForm.QueryTextBox;
                int selectionStart = queryTextBox.RichTextBox.TextLength;
                string append = text;
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
        private enum TCHITTESTFLAGS
        {
            TCHT_NOWHERE = 1,
            TCHT_ONITEMICON = 2,
            TCHT_ONITEMLABEL = 4,
            TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
        }

        private const int TCM_HITTEST = 0x130D;

        [StructLayout(LayoutKind.Sequential)]
        private struct TCHITTESTINFO
        {
            public readonly Point pt;
            public readonly TCHITTESTFLAGS flags;

            public TCHITTESTINFO(int x, int y)
            {
                this.pt = new Point(x, y);
                this.flags = TCHITTESTFLAGS.TCHT_ONITEM;
            }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref TCHITTESTINFO lParam);

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
            this.CancelQuery();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ExecuteQuery();
        }

        internal void OnDeactivate()
        {
            //if (this.completionForm != null)
            //{
            //    this.completionForm.Close();
            //    this.completionForm = null;
            //}
        }

        private void openTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTable(this.Query);
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var connection = this.connection.Connection;
            bool on = false;
            try
            {
                connection.ExecuteNonQuery(null, "SET PARSEONLY ON", CommandType.Text, 0);
                on = true;
                string query = this.Query;
                bool succeeded;

                try
                {
                    connection.ExecuteNonQuery(null, query, CommandType.Text, 0);
                    succeeded = this.infoMessages.Count == 0;
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    var infoMessages = this.provider.ToInfoMessages(exception);
                    this.AddInfoMessages(infoMessages);
                }

                if (succeeded)
                {
                    this.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, "Command(s) completed successfully."));
                }
            }
            catch (Exception exception)
            {
                var infoMessages = this.provider.ToInfoMessages(exception);
                this.AddInfoMessages(infoMessages);
            }

            if (on)
            {
                connection.ExecuteNonQuery(null, "SET PARSEONLY OFF", CommandType.Text, 0);
            }

            // TODO why? this.infoMessages.Clear();
        }

        public void SetStatusbarPanelText(string text, Color color)
        {
            this.sbPanelText.Text = text;
            this.sbPanelText.ForeColor = color;
            this.Refresh();
        }

        #endregion
    }
}