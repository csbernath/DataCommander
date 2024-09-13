using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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
using DataCommander.Application.Connection;
using DataCommander.Application.ResultWriter;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Data.SqlClient.DbQueryBuilding;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Threading;
using Newtonsoft.Json;

namespace DataCommander.Application.Query;

public sealed partial class QueryForm : Form, IQueryForm
{
    static QueryForm()
    {
        NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
    }

    public QueryForm(MainForm mainForm, IProvider provider, ConnectionInfo connectionInfo, ConnectionBase connection,
        StatusStrip parentStatusBar, ColorTheme? colorTheme)
    {
        Log.Trace(CallerInformation.Create(), "Queryform.ctor...");
        GarbageMonitor.Default.Add("QueryForm", this);

        _mainForm = mainForm;
        Provider = provider;
        _connectionInfo = connectionInfo;
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
        GarbageMonitor.Default.Add("QueryForm._messagesTextBox", _messagesTextBox);
        _messagesTextBox.Multiline = true;
        _messagesTextBox.WordWrap = false;
        _messagesTextBox.Dock = DockStyle.Fill;
        _messagesTextBox.ScrollBars = RichTextBoxScrollBars.Both;

        _messagesTabPage = new TabPage("Messages");
        _messagesTabPage.Controls.Add(_messagesTextBox);

        InitializeComponent();
        GarbageMonitor.Default.Add("queryForm._toolStrip", _toolStrip);
        _mnuFind.Click += mnuFind_Click;
        _mnuFindNext.Click += mnuFindNext_Click;
        _mnuPaste.Click += mnuPaste_Click;
        _mnuGoTo.Click += mnuGoTo_Click;
        _mnuClearCache.Click += mnuClearCache_Click;

        var sqlKeyWords = Settings.CurrentType.Attributes["SqlReservedWords"].GetValue<string[]>();
        var providerKeyWords = provider.KeyWords;

        _queryTextBox.SetColorTheme(colorTheme);
        _queryTextBox.AddKeyWords(["exec"], colorTheme != null
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
        _resultSetsTabControl.MouseUp += ResultSetsTabControl_MouseUp;
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
            var startTimestamp = Stopwatch.GetTimestamp();            
            objectExplorer.SetConnection(_connectionInfo.ConnectionStringAndCredential);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancelableOperationForm = new CancelableOperationForm(mainForm, cancellationTokenSource, TimeSpan.FromSeconds(2), "Getting children...",
                "Please wait...", colorTheme);
            var cancellationToken = cancellationTokenSource.Token;
            var children = cancelableOperationForm.Execute(new Task<IEnumerable<ITreeNode>>(() => objectExplorer.GetChildren(true, cancellationToken).Result));
            AddNodes(_tvObjectExplorer.Nodes, children, objectExplorer.Sortable, startTimestamp);
        }
        else
        {
            _tvObjectExplorer.Visible = false;
            _splitterObjectExplorer.Visible = false;
            _mnuObjectExplorer.Enabled = false;
        }

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

        colorTheme?.Apply(this);

        if (colorTheme != null)
        {
            PreOrderTreeTraversal.ForEach(
                (object)_mainMenu,
                @object =>
                {
                    if (@object == _mainMenu)
                        return _mainMenu.Items.Cast<object>();
                    if (@object is ToolStripDropDownItem toolStripDropDownItem)
                        return toolStripDropDownItem.DropDownItems.Cast<object>();
                    if (@object is ToolStripDropDown toolStripDropDown)
                        return toolStripDropDown.Items.Cast<object>();
                    else
                        return Array.Empty<object>();
                },
                @object =>
                {
                    if (@object is MenuStrip menuStrip)
                        _colorTheme.Apply(menuStrip);
                    else if (@object is ToolStripItem toolStripItem)
                        _colorTheme.Apply(toolStripItem);
                });
        }

        Log.Trace(CallerInformation.Create(), "Queryform.ctor finished.");
    }

    private void ResultSetsTabControl_MouseUp(object sender, MouseEventArgs e)
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

    public ColorTheme ColorTheme => _colorTheme;

    public CommandState ButtonState { get; private set; }

    public ConnectionBase? Connection { get; private set; }

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
        // _menuItem9
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
        this._menuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            this._mnuPaste,
            this._mnuFind,
            this._mnuFindNext,
            this._mnuCodeCompletion,
            this._mnuGoTo,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem
        });
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
            this._rollbackTransactionToolStripMenuItem,
            this.createCCommandQueryToolStripMenuItem
        });
        this._menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
        this._menuItem1.MergeIndex = 3;
        this._menuItem1.Name = "_menuItem1";
        this._menuItem1.Size = new System.Drawing.Size(51, 20);
        this._menuItem1.Text = "&Query";
        // 
        // _menuItem7
        // 
        this._menuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            this._mnuCommandTypeText,
            this._mnuCommandTypeStoredProcedure
        });
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
        this.createCCommandQueryToolStripMenuItem.ShortcutKeys =
            ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                                          | System.Windows.Forms.Keys.Q)));
        this.createCCommandQueryToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
        this.createCCommandQueryToolStripMenuItem.Text = "Create C# Command/Query";
        this.createCCommandQueryToolStripMenuItem.Click += new System.EventHandler(this.createCCommandQueryToolStripMenuItem_Click);
        // 
        // _menuItem3
        // 
        this._menuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            this._mnuObjectExplorer,
            this._mnuRefreshObjectExplorer
        });
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
        this._mnuRefreshObjectExplorer.Click += new System.EventHandler(this.MnuRefreshObjectExplorer_Click);
        // 
        // _statusBar
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
        this._tvObjectExplorer.Font =
            new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this._tvObjectExplorer.Location = new System.Drawing.Point(0, 24);
        this._tvObjectExplorer.Name = "_tvObjectExplorer";
        this._tvObjectExplorer.Size = new System.Drawing.Size(300, 541);
        this._tvObjectExplorer.TabIndex = 4;
        this._tvObjectExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvObjectBrowser_BeforeExpand);
        this._tvObjectExplorer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvObjectBrowser_ItemDrag);
        this._tvObjectExplorer.DoubleClick += new System.EventHandler(this.tvObjectBrowser_DoubleClick);
        this._tvObjectExplorer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvObjectBrowser_MouseDown);
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
        // _toolStripSeparator4
        // 
        this._toolStripSeparator4.Name = "_toolStripSeparator4";
        this._toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
        // 
        // _executeQuerySplitButton
        // 
        this._executeQuerySplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._executeQuerySplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            this._executeQueryMenuItem,
            this._executeQuerySingleRowToolStripMenuItem,
            this._cToolStripMenuItem,
            this._openTableToolStripMenuItem
        });
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

    private void AddNodes(TreeNodeCollection parent, IEnumerable<ITreeNode> children, bool sortable, long startTimestamp)
    {
        ArgumentNullException.ThrowIfNull(children);
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

        var ticks = Stopwatch.GetTimestamp() - startTimestamp;
        var items = DataCommander.Application.ResultWriter.StringExtensions.SingularOrPlural(count, "item", "items");
        SetStatusbarPanelText($"{items} added to Object Explorer in {StopwatchTimeSpan.ToString(ticks, 3)}.");
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
        var text =
            $"{_connectionName} - {Provider.GetConnectionName(() => Provider.CreateConnection(_connectionInfo.ConnectionStringAndCredential).Connection)}";
        Text = text;

        var mainForm = DataCommanderApplication.Instance.MainForm;
        mainForm.ActiveMdiChildToolStripTextBox.Text = text;
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
            SetStatusbarPanelText("Executing query...");
            var statements = Provider.GetStatements(query);
            Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
            IReadOnlyCollection<AsyncDataAdapterCommand> commands;

            if (statements.Count == 1)
            {
                DbCommand command;

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
            _dataAdapter = new AsyncDataAdapter(Provider, maxRecords, _rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
            _dataAdapter.Start(commands);
        }
        catch (Exception ex)
        {
            WriteEnd(_dataAdapter);
            EndFill(_dataAdapter, ex);
        }
    }

    private sealed class GetQueryConfigurationResult(
        bool succeeded,
        Api.QueryConfiguration.Query query,
        ReadOnlyCollection<DbRequestParameter> parameters,
        string commandText)
    {
        public readonly bool Succeeded = succeeded;
        public readonly Api.QueryConfiguration.Query Query = query;
        public readonly ReadOnlyCollection<DbRequestParameter> Parameters = parameters;
        public readonly string CommandText = commandText;
    }

    private static GetQueryConfigurationResult GetQueryConfiguration(string commandText)
    {
        ArgumentNullException.ThrowIfNull(commandText);

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
                configurationStart = commandText.IndexOf('{', configurationStart);
                configurationEnd = commandText.LastIndexOf('}', configurationEnd);
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

                    resultCommandText = commandText[(parametersEnd + 16)..];
                    succeeded = true;
                }
            }
        }

        return new GetQueryConfigurationResult(succeeded, query, parameters, resultCommandText);
    }

    private static DbRequestParameter ToDbRequestParameter(List<Token> declaration)
    {
        var name = declaration[1].Value;
        name = name[1..];
        var dataType = declaration[2].Value;
        var dataTypeLower = dataType.ToLower();
        SqlDbType sqlDbType;
        var size = 0;
        bool isNullable;
        string csharpValue = null;

        var sqlDataType = SqlDataTypeRepository.SqlDataTypes.FirstOrDefault(i => i.SqlDataTypeName == dataTypeLower);
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
            if (token.Type == TokenType.KeyWord && string.Equals(token.Value, "declare", StringComparison.InvariantCultureIgnoreCase))
            {
                if (declaration != null)
                    declarations.Add(declaration);

                declaration = [];
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

            schemaTable.Rows.Add([
                column.ColumnName,
                columnSize,
                column.DataType,
                numericPrecision,
                numericScale
            ]);
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

            resultWriter.WriteRows([values], 1);
        }

        resultWriter.WriteTableEnd();

        resultWriter.End();
    }

    private void ShowDataTableDataGrid(DataTable dataTable)
    {
        var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
        var dataTableEditor = new DataTableEditor(this, commandBuilder, _colorTheme)
        {
            ReadOnly = !_openTableMode
        };

        if (_openTableMode)
        {
            var tableName = _sqlStatement.FindTableName();
            dataTableEditor.TableName = tableName;
            var getTableSchemaResult = Provider.GetTableSchema(Connection.Connection, tableName);
            dataTableEditor.TableSchema = getTableSchemaResult;
        }

        GarbageMonitor.Default.Add("dataTableEditor", dataTableEditor);
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
            SetStatusbarPanelText("Creating Word document...");

            var fileName = WordDocumentCreator.CreateWordDocument(dataTable);

            var richTextBox = new RichTextBox();
            GarbageMonitor.Default.Add("ShowDataTableRtf.richTextBox", richTextBox);
            richTextBox.WordWrap = false;
            richTextBox.LoadFile(fileName);
            File.Delete(fileName);

            SetStatusbarPanelText("Word document created.");

            ShowTabPage(dataTable.TableName, GetToolTipText(dataTable), richTextBox);
        }
        catch (Exception e)
        {
            ShowMessage(e);
        }
    }

    private static void ShowDataTableExcel(DataTable dataTable)
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

    private static void FocusControl(Control control) => control.Focus();

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
                    var resultSetTabPage = new TabPage(text)
                    {
                        ToolTipText = null // TODO
                    };
                    _resultSetsTabControl.TabPages.Add(resultSetTabPage);
                    _resultSetsTabControl.SelectedTab = resultSetTabPage;
                    var tabControl = new TabControl();
                    tabControl.Dock = DockStyle.Fill;
                    var index = 0;
                    foreach (var dataGridView in dataGridViewResultWriter.DataGridViews)
                    {
                        dataGridView.Dock = DockStyle.Fill;
                        //text = dataTable.TableName;
                        var tabPage = new TabPage(text)
                        {
                            ToolTipText = null // TODO
                        };
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
                    try
                    {
                        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Connection is closed. Opening connection..."));
                        var connection = Provider.CreateConnection(_connectionInfo.ConnectionStringAndCredential);
                        var cancellationTokenSource = new CancellationTokenSource();
                        var cancellationToken = cancellationTokenSource.Token;
                        var cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(2),
                            "Opening connection...", string.Empty, _colorTheme);
                        cancelableOperationForm.Execute(new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken)));
                        Connection.Connection.Dispose();
                        Connection = connection;
                        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Opening connection succeeded."));
                    }
                    catch (Exception exception)
                    {
                        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Error, null, $"Opening connection failed.\r\n{exception.Message}"));
                    }
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
            SetStatusbarPanelText("Query was cancelled by user.");
            _cancel = false;
        }
        else
        {
            if (_errorCount == 0)
                SetStatusbarPanelText("Query executed successfully.");
            else
                SetStatusbarPanelText("Query completed with errors.", _colorTheme != null ? _colorTheme.ProviderKeyWordColor : Color.Red);
        }

        _dataAdapter = null;
        _dataSetResultWriter = null;

        SetGui(CommandState.Execute);
        FocusControl(QueryTextBox);
        Cursor = Cursors.Default;

        Invoke(() => _mainForm.UpdateTotalMemory());
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

    private static void WriteInfoMessageToLog(InfoMessage infoMessage)
    {
        var logLevel = infoMessage.Severity switch
        {
            InfoMessageSeverity.Error => LogLevel.Error,
            InfoMessageSeverity.Information => LogLevel.Information,
            InfoMessageSeverity.Verbose => LogLevel.Trace,
            _ => throw new Exception(),
        };
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
            while (!_infoMessages.IsEmpty && IsHandleCreated)
            {
                hasElements = true;
                var infoMessages = new InfoMessage[_infoMessages.Count];
                var count = _infoMessages.Take(infoMessages);
                try
                {
                    for (var i = 0; i < count; i++)
                    {
                        Invoke(() =>
                        {
                            var message = infoMessages[i];
                            var color = _messagesTextBox.SelectionColor;

                            switch (message.Severity)
                            {
                                case InfoMessageSeverity.Error:
                                    _messagesTextBox.SelectionColor = _colorTheme?.ProviderKeyWordColor ?? Color.Red;
                                    break;

                                case InfoMessageSeverity.Information:
                                    _messagesTextBox.SelectionColor = _colorTheme?.SqlKeyWordColor ?? Color.Blue;
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
                catch
                {
                }
            }

            if (hasElements)
            {
                try
                {
                    Invoke(() =>
                    {
                        _messagesTextBox.ScrollToCaret();
                        _messagesTextBox.Update();
                    });
                }
                catch
                {
                }
            }

            if (_infoMessages.IsEmpty)
            {
                var w = WaitHandle.WaitAny(waitHandles, 1000);
                if (w == 1)
                    break;
            }
        }
    }

    internal static void CopyTableWithSqlBulkCopy()
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
    private struct Tchittestinfo(int x, int y)
    {
        public readonly Point pt = new(x, y);
        public readonly Tchittestflags flags = Tchittestflags.TchtOnitem;
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref Tchittestinfo lParam);

    private void SetStatusbarPanelText(string text, Color color)
    {
        _sbPanelText.Text = text;
        _sbPanelText.ForeColor = color;
    }

    public ICancelableOperationForm CreateCancelableOperationForm(
        CancellationTokenSource cancellationTokenSource,
        TimeSpan showDialogDelay,
        string formText,
        string textBoxText) =>
        new CancelableOperationForm(this, cancellationTokenSource, showDialogDelay, formText, textBoxText, _colorTheme);
}