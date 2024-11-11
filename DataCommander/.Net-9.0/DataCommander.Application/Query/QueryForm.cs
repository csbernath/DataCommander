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
        _mnuFind!.Click += mnuFind_Click;
        _mnuFindNext!.Click += mnuFindNext_Click;
        _mnuPaste!.Click += mnuPaste_Click;
        _mnuGoTo!.Click += mnuGoTo_Click;
        _mnuClearCache!.Click += mnuClearCache_Click;

        var sqlKeyWords = Settings.CurrentType.Attributes["SqlReservedWords"].GetValue<string[]>();
        var providerKeyWords = provider.KeyWords;

        _queryTextBox!.SetColorTheme(colorTheme);
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
            var cancelableOperationForm = new CancelableOperationForm(mainForm, cancellationTokenSource, TimeSpan.FromSeconds(1), "Getting children...",
                "Please wait...", colorTheme);
            var cancellationToken = cancellationTokenSource.Token;
            var children = cancelableOperationForm.Execute(new Task<IEnumerable<ITreeNode>>(() => objectExplorer.GetChildren(true, cancellationToken).Result));
            AddNodes(_tvObjectExplorer!.Nodes, children, objectExplorer.Sortable, startTimestamp);
        }
        else
        {
            _tvObjectExplorer!.Visible = false;
            _splitterObjectExplorer!.Visible = false;
            _mnuObjectExplorer!.Enabled = false;
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
                        return _mainMenu!.Items.Cast<object>();
                    if (@object is ToolStripDropDownItem toolStripDropDownItem)
                        return toolStripDropDownItem.DropDownItems.Cast<object>();
                    if (@object is ToolStripDropDown toolStripDropDown)
                        return toolStripDropDown.Items.Cast<object>();
                    else
                        return [];
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

    private void ResultSetsTabControl_MouseUp(object? sender, MouseEventArgs e)
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
        _mainMenu = new MenuStrip();
        _menuItem9 = new ToolStripMenuItem();
        _mnuSave = new ToolStripMenuItem();
        _mnuSaveAs = new ToolStripMenuItem();
        _mnuDuplicateConnection = new ToolStripMenuItem();
        _menuItem8 = new ToolStripMenuItem();
        _mnuPaste = new ToolStripMenuItem();
        _mnuFind = new ToolStripMenuItem();
        _mnuFindNext = new ToolStripMenuItem();
        _mnuCodeCompletion = new ToolStripMenuItem();
        _mnuListMembers = new ToolStripMenuItem();
        _mnuClearCache = new ToolStripMenuItem();
        _mnuGoTo = new ToolStripMenuItem();
        undoToolStripMenuItem = new ToolStripMenuItem();
        redoToolStripMenuItem = new ToolStripMenuItem();
        _menuItem1 = new ToolStripMenuItem();
        _menuItem7 = new ToolStripMenuItem();
        _mnuCommandTypeText = new ToolStripMenuItem();
        _mnuCommandTypeStoredProcedure = new ToolStripMenuItem();
        _mnuDescribeParameters = new ToolStripMenuItem();
        _toolStripSeparator2 = new ToolStripSeparator();
        _mnuShowShemaTable = new ToolStripMenuItem();
        _executeQueryToolStripMenuItem = new ToolStripMenuItem();
        _mnuExecuteQuerySingleRow = new ToolStripMenuItem();
        _mnuExecuteQuerySchemaOnly = new ToolStripMenuItem();
        _mnuExecuteQueryKeyInfo = new ToolStripMenuItem();
        _mnuExecuteQueryXml = new ToolStripMenuItem();
        _mnuOpenTable = new ToolStripMenuItem();
        _mnuCancel = new ToolStripMenuItem();
        _parseToolStripMenuItem = new ToolStripMenuItem();
        _toolStripSeparator1 = new ToolStripSeparator();
        _menuItem2 = new ToolStripMenuItem();
        _mnuText = new ToolStripMenuItem();
        _mnuDataGrid = new ToolStripMenuItem();
        _mnuHtml = new ToolStripMenuItem();
        _mnuRtf = new ToolStripMenuItem();
        _mnuListView = new ToolStripMenuItem();
        _mnuExcel = new ToolStripMenuItem();
        _menuResultModeFile = new ToolStripMenuItem();
        _sQLiteDatabaseToolStripMenuItem = new ToolStripMenuItem();
        _insertScriptFileToolStripMenuItem = new ToolStripMenuItem();
        _toolStripSeparator3 = new ToolStripSeparator();
        _mnuGotoQueryEditor = new ToolStripMenuItem();
        _mnuGotoMessageTabPage = new ToolStripMenuItem();
        _mnuCloseTabPage = new ToolStripMenuItem();
        _mnuCloseAllTabPages = new ToolStripMenuItem();
        _mnuCreateInsert = new ToolStripMenuItem();
        _mnuCreateInsertSelect = new ToolStripMenuItem();
        _createSqlCeDatabaseToolStripMenuItem = new ToolStripMenuItem();
        _exportToolStripMenuItem = new ToolStripMenuItem();
        _beginTransactionToolStripMenuItem = new ToolStripMenuItem();
        _commitTransactionToolStripMenuItem = new ToolStripMenuItem();
        _rollbackTransactionToolStripMenuItem = new ToolStripMenuItem();
        createCCommandQueryToolStripMenuItem = new ToolStripMenuItem();
        _menuItem3 = new ToolStripMenuItem();
        _mnuObjectExplorer = new ToolStripMenuItem();
        _mnuRefreshObjectExplorer = new ToolStripMenuItem();
        _statusBar = new StatusStrip();
        _sbPanelText = new ToolStripStatusLabel();
        _sbPanelTableStyle = new ToolStripStatusLabel();
        _sbPanelTimer = new ToolStripStatusLabel();
        _sbPanelRows = new ToolStripStatusLabel();
        _sbPanelCaretPosition = new ToolStripStatusLabel();
        _tvObjectExplorer = new TreeView();
        _splitterObjectExplorer = new Splitter();
        _splitterQuery = new Splitter();
        _tabControl = new TabControl();
        _toolStrip = new ToolStrip();
        _toolStripSeparator4 = new ToolStripSeparator();
        _executeQuerySplitButton = new ToolStripSplitButton();
        _executeQueryMenuItem = new ToolStripMenuItem();
        _executeQuerySingleRowToolStripMenuItem = new ToolStripMenuItem();
        _cToolStripMenuItem = new ToolStripMenuItem();
        _openTableToolStripMenuItem = new ToolStripMenuItem();
        _cancelQueryButton = new ToolStripButton();
        _queryTextBox = new QueryTextBox();
        _mainMenu.SuspendLayout();
        _statusBar.SuspendLayout();
        _toolStrip.SuspendLayout();
        SuspendLayout();
        // 
        // _mainMenu
        // 
        _mainMenu.Items.AddRange(
        [
            _menuItem9,
            _menuItem8,
            _menuItem1,
            _menuItem3
        ]);
        _mainMenu.Location = new Point(0, 0);
        _mainMenu.Name = "_mainMenu";
        _mainMenu.Size = new Size(1016, 24);
        _mainMenu.TabIndex = 0;
        _mainMenu.Visible = false;
        // 
        // _menuItem9
        // 
        _menuItem9.DropDownItems.AddRange(
        [
            _mnuSave,
            _mnuSaveAs,
            _mnuDuplicateConnection
        ]);
        _menuItem9.MergeAction = MergeAction.MatchOnly;
        _menuItem9.MergeIndex = 0;
        _menuItem9.Name = "_menuItem9";
        _menuItem9.Size = new Size(37, 20);
        _menuItem9.Text = "&File";
        // 
        // _mnuSave
        // 
        _mnuSave.MergeAction = MergeAction.Insert;
        _mnuSave.MergeIndex = 2;
        _mnuSave.Name = "_mnuSave";
        _mnuSave.ShortcutKeys = ((Keys)((Keys.Control | Keys.S)));
        _mnuSave.Size = new Size(230, 22);
        _mnuSave.Text = "&Save";
        _mnuSave.Click += new EventHandler(mnuSave_Click);
        // 
        // _mnuSaveAs
        // 
        _mnuSaveAs.MergeAction = MergeAction.Insert;
        _mnuSaveAs.MergeIndex = 3;
        _mnuSaveAs.Name = "_mnuSaveAs";
        _mnuSaveAs.Size = new Size(230, 22);
        _mnuSaveAs.Text = "Save &As";
        _mnuSaveAs.Click += new EventHandler(mnuSaveAs_Click);
        // 
        // _mnuDuplicateConnection
        // 
        _mnuDuplicateConnection.MergeAction = MergeAction.Insert;
        _mnuDuplicateConnection.MergeIndex = 4;
        _mnuDuplicateConnection.Name = "_mnuDuplicateConnection";
        _mnuDuplicateConnection.ShortcutKeys = ((Keys)((Keys.Control | Keys.Q)));
        _mnuDuplicateConnection.Size = new Size(230, 22);
        _mnuDuplicateConnection.Text = "Duplicate connection";
        _mnuDuplicateConnection.Click += new EventHandler(mnuDuplicateConnection_Click);
        // 
        // _menuItem8
        // 
        _menuItem8.DropDownItems.AddRange(
        [
            _mnuPaste,
            _mnuFind,
            _mnuFindNext,
            _mnuCodeCompletion,
            _mnuGoTo,
            undoToolStripMenuItem,
            redoToolStripMenuItem
        ]);
        _menuItem8.MergeAction = MergeAction.Insert;
        _menuItem8.MergeIndex = 2;
        _menuItem8.Name = "_menuItem8";
        _menuItem8.Size = new Size(39, 20);
        _menuItem8.Text = "&Edit";
        // 
        // _mnuPaste
        // 
        _mnuPaste.Image = ((Image)(resources.GetObject("_mnuPaste.Image")));
        _mnuPaste.MergeIndex = 0;
        _mnuPaste.Name = "_mnuPaste";
        _mnuPaste.ShortcutKeys = ((Keys)((Keys.Control | Keys.V)));
        _mnuPaste.Size = new Size(166, 22);
        _mnuPaste.Text = "&Paste";
        // 
        // _mnuFind
        // 
        _mnuFind.Image = ((Image)(resources.GetObject("_mnuFind.Image")));
        _mnuFind.MergeIndex = 1;
        _mnuFind.Name = "_mnuFind";
        _mnuFind.ShortcutKeys = ((Keys)((Keys.Control | Keys.F)));
        _mnuFind.Size = new Size(166, 22);
        _mnuFind.Text = "&Find";
        // 
        // _mnuFindNext
        // 
        _mnuFindNext.MergeIndex = 2;
        _mnuFindNext.Name = "_mnuFindNext";
        _mnuFindNext.ShortcutKeys = Keys.F3;
        _mnuFindNext.Size = new Size(166, 22);
        _mnuFindNext.Text = "Find &Next";
        // 
        // _mnuCodeCompletion
        // 
        _mnuCodeCompletion.DropDownItems.AddRange(
        [
            _mnuListMembers,
            _mnuClearCache
        ]);
        _mnuCodeCompletion.MergeIndex = 3;
        _mnuCodeCompletion.Name = "_mnuCodeCompletion";
        _mnuCodeCompletion.Size = new Size(166, 22);
        _mnuCodeCompletion.Text = "&Code completion";
        // 
        // _mnuListMembers
        // 
        _mnuListMembers.MergeIndex = 0;
        _mnuListMembers.Name = "_mnuListMembers";
        _mnuListMembers.ShortcutKeys = ((Keys)((Keys.Control | Keys.J)));
        _mnuListMembers.Size = new Size(211, 22);
        _mnuListMembers.Text = "&List Members";
        _mnuListMembers.Click += new EventHandler(mnuListMembers_Click);
        // 
        // _mnuClearCache
        // 
        _mnuClearCache.MergeIndex = 1;
        _mnuClearCache.Name = "_mnuClearCache";
        _mnuClearCache.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                                                                         | Keys.C)));
        _mnuClearCache.Size = new Size(211, 22);
        _mnuClearCache.Text = "&Clear Cache";
        // 
        // _mnuGoTo
        // 
        _mnuGoTo.MergeIndex = 4;
        _mnuGoTo.Name = "_mnuGoTo";
        _mnuGoTo.ShortcutKeys = ((Keys)((Keys.Control | Keys.G)));
        _mnuGoTo.Size = new Size(166, 22);
        _mnuGoTo.Text = "Go To...";
        // 
        // undoToolStripMenuItem
        // 
        undoToolStripMenuItem.Name = "undoToolStripMenuItem";
        undoToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.Z)));
        undoToolStripMenuItem.Size = new Size(166, 22);
        undoToolStripMenuItem.Text = "Undo";
        undoToolStripMenuItem.Click += new EventHandler(UndoToolStripMenuItem_Click);
        // 
        // redoToolStripMenuItem
        // 
        redoToolStripMenuItem.Name = "redoToolStripMenuItem";
        redoToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.Y)));
        redoToolStripMenuItem.Size = new Size(166, 22);
        redoToolStripMenuItem.Text = "Redo";
        redoToolStripMenuItem.Click += new EventHandler(redoToolStripMenuItem_Click);
        // 
        // _menuItem1
        // 
        _menuItem1.DropDownItems.AddRange(
        [
            _menuItem7,
            _mnuDescribeParameters,
            _toolStripSeparator2,
            _mnuShowShemaTable,
            _executeQueryToolStripMenuItem,
            _mnuExecuteQuerySingleRow,
            _mnuExecuteQuerySchemaOnly,
            _mnuExecuteQueryKeyInfo,
            _mnuExecuteQueryXml,
            _mnuOpenTable,
            _mnuCancel,
            _parseToolStripMenuItem,
            _toolStripSeparator1,
            _menuItem2,
            _toolStripSeparator3,
            _mnuGotoQueryEditor,
            _mnuGotoMessageTabPage,
            _mnuCloseTabPage,
            _mnuCloseAllTabPages,
            _mnuCreateInsert,
            _mnuCreateInsertSelect,
            _createSqlCeDatabaseToolStripMenuItem,
            _exportToolStripMenuItem,
            _beginTransactionToolStripMenuItem,
            _commitTransactionToolStripMenuItem,
            _rollbackTransactionToolStripMenuItem,
            createCCommandQueryToolStripMenuItem
        ]);
        _menuItem1.MergeAction = MergeAction.Insert;
        _menuItem1.MergeIndex = 3;
        _menuItem1.Name = "_menuItem1";
        _menuItem1.Size = new Size(51, 20);
        _menuItem1.Text = "&Query";
        // 
        // _menuItem7
        // 
        _menuItem7.DropDownItems.AddRange(
        [
            _mnuCommandTypeText,
            _mnuCommandTypeStoredProcedure
        ]);
        _menuItem7.MergeIndex = 0;
        _menuItem7.Name = "_menuItem7";
        _menuItem7.Size = new Size(298, 22);
        _menuItem7.Text = "Command&Type";
        // 
        // _mnuCommandTypeText
        // 
        _mnuCommandTypeText.Checked = true;
        _mnuCommandTypeText.CheckState = CheckState.Checked;
        _mnuCommandTypeText.MergeIndex = 0;
        _mnuCommandTypeText.Name = "_mnuCommandTypeText";
        _mnuCommandTypeText.Size = new Size(165, 22);
        _mnuCommandTypeText.Text = "Text";
        _mnuCommandTypeText.Click += new EventHandler(mnuCommandTypeText_Click);
        // 
        // _mnuCommandTypeStoredProcedure
        // 
        _mnuCommandTypeStoredProcedure.MergeIndex = 1;
        _mnuCommandTypeStoredProcedure.Name = "_mnuCommandTypeStoredProcedure";
        _mnuCommandTypeStoredProcedure.Size = new Size(165, 22);
        _mnuCommandTypeStoredProcedure.Text = "Stored Procedure";
        _mnuCommandTypeStoredProcedure.Click += new EventHandler(mnuCommandTypeStoredProcedure_Click);
        // 
        // _mnuDescribeParameters
        // 
        _mnuDescribeParameters.MergeIndex = 1;
        _mnuDescribeParameters.Name = "_mnuDescribeParameters";
        _mnuDescribeParameters.ShortcutKeys = ((Keys)((Keys.Control | Keys.P)));
        _mnuDescribeParameters.Size = new Size(298, 22);
        _mnuDescribeParameters.Text = "Describe &Parameters";
        _mnuDescribeParameters.Click += new EventHandler(MnuDescribeParameters_Click);
        // 
        // _toolStripSeparator2
        // 
        _toolStripSeparator2.Name = "_toolStripSeparator2";
        _toolStripSeparator2.Size = new Size(295, 6);
        // 
        // _mnuShowShemaTable
        // 
        _mnuShowShemaTable.MergeIndex = 3;
        _mnuShowShemaTable.Name = "_mnuShowShemaTable";
        _mnuShowShemaTable.Size = new Size(298, 22);
        _mnuShowShemaTable.Text = "Show SchemaTable";
        _mnuShowShemaTable.Click += new EventHandler(mnuShowShemaTable_Click);
        // 
        // _executeQueryToolStripMenuItem
        // 
        _executeQueryToolStripMenuItem.Name = "_executeQueryToolStripMenuItem";
        _executeQueryToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.E)));
        _executeQueryToolStripMenuItem.Size = new Size(298, 22);
        _executeQueryToolStripMenuItem.Text = "Execute Query";
        _executeQueryToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem1_Click);
        // 
        // _mnuExecuteQuerySingleRow
        // 
        _mnuExecuteQuerySingleRow.MergeIndex = 6;
        _mnuExecuteQuerySingleRow.Name = "_mnuExecuteQuerySingleRow";
        _mnuExecuteQuerySingleRow.ShortcutKeys = ((Keys)((Keys.Control | Keys.D1)));
        _mnuExecuteQuerySingleRow.Size = new Size(298, 22);
        _mnuExecuteQuerySingleRow.Text = "Execute Query (SingleRow)";
        _mnuExecuteQuerySingleRow.Click += new EventHandler(mnuSingleRow_Click);
        // 
        // _mnuExecuteQuerySchemaOnly
        // 
        _mnuExecuteQuerySchemaOnly.MergeIndex = 7;
        _mnuExecuteQuerySchemaOnly.Name = "_mnuExecuteQuerySchemaOnly";
        _mnuExecuteQuerySchemaOnly.ShortcutKeys = ((Keys)((Keys.Control | Keys.R)));
        _mnuExecuteQuerySchemaOnly.Size = new Size(298, 22);
        _mnuExecuteQuerySchemaOnly.Text = "Execute Query (Schema only)";
        _mnuExecuteQuerySchemaOnly.Click += new EventHandler(mnuResultSchema_Click);
        // 
        // _mnuExecuteQueryKeyInfo
        // 
        _mnuExecuteQueryKeyInfo.MergeIndex = 8;
        _mnuExecuteQueryKeyInfo.Name = "_mnuExecuteQueryKeyInfo";
        _mnuExecuteQueryKeyInfo.ShortcutKeys = ((Keys)((Keys.Control | Keys.K)));
        _mnuExecuteQueryKeyInfo.Size = new Size(298, 22);
        _mnuExecuteQueryKeyInfo.Text = "Execute Query (&KeyInfo)";
        _mnuExecuteQueryKeyInfo.Click += new EventHandler(mnuKeyInfo_Click);
        // 
        // _mnuExecuteQueryXml
        // 
        _mnuExecuteQueryXml.MergeIndex = 9;
        _mnuExecuteQueryXml.Name = "_mnuExecuteQueryXml";
        _mnuExecuteQueryXml.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                                                                              | Keys.X)));
        _mnuExecuteQueryXml.Size = new Size(298, 22);
        _mnuExecuteQueryXml.Text = "Execute Query (XML)";
        _mnuExecuteQueryXml.Click += new EventHandler(mnuXml_Click);
        // 
        // _mnuOpenTable
        // 
        _mnuOpenTable.MergeIndex = 10;
        _mnuOpenTable.Name = "_mnuOpenTable";
        _mnuOpenTable.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                                                                        | Keys.O)));
        _mnuOpenTable.Size = new Size(298, 22);
        _mnuOpenTable.Text = "Edit Rows";
        _mnuOpenTable.Click += new EventHandler(EditRows_Click);
        // 
        // _mnuCancel
        // 
        _mnuCancel.Enabled = false;
        _mnuCancel.MergeIndex = 11;
        _mnuCancel.Name = "_mnuCancel";
        _mnuCancel.ShortcutKeys = ((Keys)((Keys.Alt | Keys.Pause)));
        _mnuCancel.Size = new Size(298, 22);
        _mnuCancel.Text = "&Cancel Executing Query";
        _mnuCancel.Click += new EventHandler(MnuCancel_Click);
        // 
        // _parseToolStripMenuItem
        // 
        _parseToolStripMenuItem.Name = "_parseToolStripMenuItem";
        _parseToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.F5)));
        _parseToolStripMenuItem.Size = new Size(298, 22);
        _parseToolStripMenuItem.Text = "Parse";
        _parseToolStripMenuItem.Click += new EventHandler(ParseToolStripMenuItem_Click);
        // 
        // _toolStripSeparator1
        // 
        _toolStripSeparator1.Name = "_toolStripSeparator1";
        _toolStripSeparator1.Size = new Size(295, 6);
        // 
        // _menuItem2
        // 
        _menuItem2.DropDownItems.AddRange(
        [
            _mnuText,
            _mnuDataGrid,
            _mnuHtml,
            _mnuRtf,
            _mnuListView,
            _mnuExcel,
            _menuResultModeFile,
            _sQLiteDatabaseToolStripMenuItem,
            _insertScriptFileToolStripMenuItem
        ]);
        _menuItem2.MergeIndex = 13;
        _menuItem2.Name = "_menuItem2";
        _menuItem2.Size = new Size(298, 22);
        _menuItem2.Text = "Result &Mode";
        // 
        // _mnuText
        // 
        _mnuText.MergeIndex = 0;
        _mnuText.Name = "_mnuText";
        _mnuText.ShortcutKeys = ((Keys)((Keys.Control | Keys.T)));
        _mnuText.Size = new Size(162, 22);
        _mnuText.Text = "&Text";
        _mnuText.Click += new EventHandler(mnuText_Click);
        // 
        // _mnuDataGrid
        // 
        _mnuDataGrid.MergeIndex = 1;
        _mnuDataGrid.Name = "_mnuDataGrid";
        _mnuDataGrid.ShortcutKeys = ((Keys)((Keys.Control | Keys.D)));
        _mnuDataGrid.Size = new Size(162, 22);
        _mnuDataGrid.Text = "&DataGrid";
        _mnuDataGrid.Click += new EventHandler(mnuDataGrid_Click);
        // 
        // _mnuHtml
        // 
        _mnuHtml.MergeIndex = 2;
        _mnuHtml.Name = "_mnuHtml";
        _mnuHtml.Size = new Size(162, 22);
        _mnuHtml.Text = "&Html";
        _mnuHtml.Click += new EventHandler(mnuHtml_Click);
        // 
        // _mnuRtf
        // 
        _mnuRtf.MergeIndex = 3;
        _mnuRtf.Name = "_mnuRtf";
        _mnuRtf.Size = new Size(162, 22);
        _mnuRtf.Text = "&Rtf";
        // 
        // _mnuListView
        // 
        _mnuListView.MergeIndex = 4;
        _mnuListView.Name = "_mnuListView";
        _mnuListView.ShortcutKeys = ((Keys)((Keys.Control | Keys.L)));
        _mnuListView.Size = new Size(162, 22);
        _mnuListView.Text = "&ListView";
        _mnuListView.Click += new EventHandler(mnuListView_Click);
        // 
        // _mnuExcel
        // 
        _mnuExcel.MergeIndex = 5;
        _mnuExcel.Name = "_mnuExcel";
        _mnuExcel.Size = new Size(162, 22);
        _mnuExcel.Text = "&Excel";
        _mnuExcel.Visible = false;
        // 
        // _menuResultModeFile
        // 
        _menuResultModeFile.MergeIndex = 6;
        _menuResultModeFile.Name = "_menuResultModeFile";
        _menuResultModeFile.Size = new Size(162, 22);
        _menuResultModeFile.Text = "&File";
        _menuResultModeFile.Click += new EventHandler(menuResultModeFile_Click);
        // 
        // _sQLiteDatabaseToolStripMenuItem
        // 
        _sQLiteDatabaseToolStripMenuItem.Name = "_sQLiteDatabaseToolStripMenuItem";
        _sQLiteDatabaseToolStripMenuItem.Size = new Size(162, 22);
        _sQLiteDatabaseToolStripMenuItem.Text = "SQLite database";
        _sQLiteDatabaseToolStripMenuItem.Click += new EventHandler(sQLiteDatabaseToolStripMenuItem_Click);
        // 
        // _insertScriptFileToolStripMenuItem
        // 
        _insertScriptFileToolStripMenuItem.Name = "_insertScriptFileToolStripMenuItem";
        _insertScriptFileToolStripMenuItem.Size = new Size(162, 22);
        _insertScriptFileToolStripMenuItem.Text = "Insert Script File";
        _insertScriptFileToolStripMenuItem.Click += new EventHandler(InsertScriptFileToolStripMenuItem_Click);
        // 
        // _toolStripSeparator3
        // 
        _toolStripSeparator3.Name = "_toolStripSeparator3";
        _toolStripSeparator3.Size = new Size(295, 6);
        // 
        // _mnuGotoQueryEditor
        // 
        _mnuGotoQueryEditor.MergeIndex = 15;
        _mnuGotoQueryEditor.Name = "_mnuGotoQueryEditor";
        _mnuGotoQueryEditor.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                                                                              | Keys.Q)));
        _mnuGotoQueryEditor.Size = new Size(298, 22);
        _mnuGotoQueryEditor.Text = "Goto &Query Editor";
        _mnuGotoQueryEditor.Click += new EventHandler(mnuGotoQueryEditor_Click);
        // 
        // _mnuGotoMessageTabPage
        // 
        _mnuGotoMessageTabPage.MergeIndex = 16;
        _mnuGotoMessageTabPage.Name = "_mnuGotoMessageTabPage";
        _mnuGotoMessageTabPage.ShortcutKeys = ((Keys)((Keys.Control | Keys.M)));
        _mnuGotoMessageTabPage.Size = new Size(298, 22);
        _mnuGotoMessageTabPage.Text = "Goto &Message TabPage";
        _mnuGotoMessageTabPage.Click += new EventHandler(mnuGotoMessageTabPage_Click);
        // 
        // _mnuCloseTabPage
        // 
        _mnuCloseTabPage.MergeIndex = 17;
        _mnuCloseTabPage.Name = "_mnuCloseTabPage";
        _mnuCloseTabPage.Size = new Size(298, 22);
        _mnuCloseTabPage.Text = "Close Current &TabPage";
        _mnuCloseTabPage.Click += new EventHandler(mnuCloseTabPage_Click);
        // 
        // _mnuCloseAllTabPages
        // 
        _mnuCloseAllTabPages.MergeIndex = 18;
        _mnuCloseAllTabPages.Name = "_mnuCloseAllTabPages";
        _mnuCloseAllTabPages.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                                                                               | Keys.F4)));
        _mnuCloseAllTabPages.Size = new Size(298, 22);
        _mnuCloseAllTabPages.Text = "Close &All TabPages";
        _mnuCloseAllTabPages.Click += new EventHandler(mnuCloseAllTabPages_Click);
        // 
        // _mnuCreateInsert
        // 
        _mnuCreateInsert.MergeIndex = 19;
        _mnuCreateInsert.Name = "_mnuCreateInsert";
        _mnuCreateInsert.ShortcutKeys = ((Keys)((Keys.Control | Keys.I)));
        _mnuCreateInsert.Size = new Size(298, 22);
        _mnuCreateInsert.Text = "Create insert statements";
        _mnuCreateInsert.Click += new EventHandler(mnuCreateInsert_Click);
        // 
        // _mnuCreateInsertSelect
        // 
        _mnuCreateInsertSelect.MergeIndex = 20;
        _mnuCreateInsertSelect.Name = "_mnuCreateInsertSelect";
        _mnuCreateInsertSelect.Size = new Size(298, 22);
        _mnuCreateInsertSelect.Text = "Create \'insert select\' statements";
        _mnuCreateInsertSelect.Click += new EventHandler(mnuCreateInsertSelect_Click);
        // 
        // _createSqlCeDatabaseToolStripMenuItem
        // 
        _createSqlCeDatabaseToolStripMenuItem.Name = "_createSqlCeDatabaseToolStripMenuItem";
        _createSqlCeDatabaseToolStripMenuItem.Size = new Size(298, 22);
        _createSqlCeDatabaseToolStripMenuItem.Text = "Create SQL Server Compact database";
        _createSqlCeDatabaseToolStripMenuItem.Click += new EventHandler(createSqlCeDatabaseToolStripMenuItem_Click);
        // 
        // _exportToolStripMenuItem
        // 
        _exportToolStripMenuItem.Name = "_exportToolStripMenuItem";
        _exportToolStripMenuItem.Size = new Size(298, 22);
        // 
        // _beginTransactionToolStripMenuItem
        // 
        _beginTransactionToolStripMenuItem.Name = "_beginTransactionToolStripMenuItem";
        _beginTransactionToolStripMenuItem.Size = new Size(298, 22);
        _beginTransactionToolStripMenuItem.Text = "Begin Transaction";
        _beginTransactionToolStripMenuItem.Click += new EventHandler(beginTransactionToolStripMenuItem_Click);
        // 
        // _commitTransactionToolStripMenuItem
        // 
        _commitTransactionToolStripMenuItem.Enabled = false;
        _commitTransactionToolStripMenuItem.Name = "_commitTransactionToolStripMenuItem";
        _commitTransactionToolStripMenuItem.Size = new Size(298, 22);
        _commitTransactionToolStripMenuItem.Text = "Commit Transaction";
        _commitTransactionToolStripMenuItem.Click += new EventHandler(commitTransactionToolStripMenuItem_Click);
        // 
        // _rollbackTransactionToolStripMenuItem
        // 
        _rollbackTransactionToolStripMenuItem.Enabled = false;
        _rollbackTransactionToolStripMenuItem.Name = "_rollbackTransactionToolStripMenuItem";
        _rollbackTransactionToolStripMenuItem.Size = new Size(298, 22);
        _rollbackTransactionToolStripMenuItem.Text = "Rollback Transaction";
        _rollbackTransactionToolStripMenuItem.Click += new EventHandler(RollbackTransactionToolStripMenuItem_Click);
        // 
        // createCCommandQueryToolStripMenuItem
        // 
        createCCommandQueryToolStripMenuItem.Name = "createCCommandQueryToolStripMenuItem";
        createCCommandQueryToolStripMenuItem.ShortcutKeys =
            ((Keys)(((Keys.Control | Keys.Shift)
                                          | Keys.Q)));
        createCCommandQueryToolStripMenuItem.Size = new Size(298, 22);
        createCCommandQueryToolStripMenuItem.Text = "Create C# Command/Query";
        createCCommandQueryToolStripMenuItem.Click += new EventHandler(createCCommandQueryToolStripMenuItem_Click);
        // 
        // _menuItem3
        // 
        _menuItem3.DropDownItems.AddRange(
        [
            _mnuObjectExplorer,
            _mnuRefreshObjectExplorer
        ]);
        _menuItem3.MergeAction = MergeAction.Insert;
        _menuItem3.MergeIndex = 4;
        _menuItem3.Name = "_menuItem3";
        _menuItem3.Size = new Size(47, 20);
        _menuItem3.Text = "&Tools";
        // 
        // _mnuObjectExplorer
        // 
        _mnuObjectExplorer.MergeIndex = 0;
        _mnuObjectExplorer.Name = "_mnuObjectExplorer";
        _mnuObjectExplorer.ShortcutKeys = Keys.F8;
        _mnuObjectExplorer.Size = new Size(229, 22);
        _mnuObjectExplorer.Text = "Object Explorer";
        _mnuObjectExplorer.Click += new EventHandler(menuObjectExplorer_Click);
        // 
        // _mnuRefreshObjectExplorer
        // 
        _mnuRefreshObjectExplorer.MergeIndex = 1;
        _mnuRefreshObjectExplorer.Name = "_mnuRefreshObjectExplorer";
        _mnuRefreshObjectExplorer.Size = new Size(229, 22);
        _mnuRefreshObjectExplorer.Text = "Refresh Object Explorer\'s root";
        _mnuRefreshObjectExplorer.Click += new EventHandler(MnuRefreshObjectExplorer_Click);
        // 
        // _statusBar
        // 
        _statusBar.Items.AddRange(
        [
            _sbPanelText,
            _sbPanelTableStyle,
            _sbPanelTimer,
            _sbPanelRows,
            _sbPanelCaretPosition
        ]);
        _statusBar.Location = new Point(300, 543);
        _statusBar.Name = "_statusBar";
        _statusBar.Size = new Size(716, 22);
        _statusBar.TabIndex = 2;
        // 
        // _sbPanelText
        // 
        _sbPanelText.AutoSize = false;
        _sbPanelText.Name = "_sbPanelText";
        _sbPanelText.Size = new Size(231, 17);
        _sbPanelText.Spring = true;
        _sbPanelText.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _sbPanelTableStyle
        // 
        _sbPanelTableStyle.AutoSize = false;
        _sbPanelTableStyle.BorderSides = ToolStripStatusLabelBorderSides.Left;
        _sbPanelTableStyle.Name = "_sbPanelTableStyle";
        _sbPanelTableStyle.Size = new Size(100, 17);
        _sbPanelTableStyle.TextAlign = ContentAlignment.MiddleLeft;
        _sbPanelTableStyle.MouseUp += new MouseEventHandler(sbPanelTableStyle_MouseUp);
        // 
        // _sbPanelTimer
        // 
        _sbPanelTimer.Alignment = ToolStripItemAlignment.Right;
        _sbPanelTimer.AutoSize = false;
        _sbPanelTimer.BorderSides = ToolStripStatusLabelBorderSides.Left;
        _sbPanelTimer.Name = "_sbPanelTimer";
        _sbPanelTimer.Size = new Size(70, 17);
        _sbPanelTimer.TextAlign = ContentAlignment.MiddleRight;
        // 
        // _sbPanelRows
        // 
        _sbPanelRows.Alignment = ToolStripItemAlignment.Right;
        _sbPanelRows.AutoSize = false;
        _sbPanelRows.BorderSides = ToolStripStatusLabelBorderSides.Left;
        _sbPanelRows.Name = "_sbPanelRows";
        _sbPanelRows.Size = new Size(200, 17);
        _sbPanelRows.TextAlign = ContentAlignment.MiddleRight;
        // 
        // _sbPanelCaretPosition
        // 
        _sbPanelCaretPosition.AutoSize = false;
        _sbPanelCaretPosition.BorderSides = ToolStripStatusLabelBorderSides.Left;
        _sbPanelCaretPosition.Name = "_sbPanelCaretPosition";
        _sbPanelCaretPosition.Size = new Size(100, 17);
        _sbPanelCaretPosition.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _tvObjectExplorer
        // 
        _tvObjectExplorer.Dock = DockStyle.Left;
        _tvObjectExplorer.Font =
            new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
        _tvObjectExplorer.Location = new Point(0, 24);
        _tvObjectExplorer.Name = "_tvObjectExplorer";
        _tvObjectExplorer.Size = new Size(300, 541);
        _tvObjectExplorer.TabIndex = 4;
        _tvObjectExplorer.BeforeExpand += new TreeViewCancelEventHandler(tvObjectBrowser_BeforeExpand);
        _tvObjectExplorer.ItemDrag += new ItemDragEventHandler(tvObjectBrowser_ItemDrag);
        _tvObjectExplorer.DoubleClick += new EventHandler(tvObjectBrowser_DoubleClick);
        _tvObjectExplorer.MouseDown += new MouseEventHandler(TvObjectBrowser_MouseDown);
        _tvObjectExplorer.MouseUp += new MouseEventHandler(tvObjectExplorer_MouseUp);
        // 
        // _splitterObjectExplorer
        // 
        _splitterObjectExplorer.Location = new Point(300, 24);
        _splitterObjectExplorer.Name = "_splitterObjectExplorer";
        _splitterObjectExplorer.Size = new Size(3, 519);
        _splitterObjectExplorer.TabIndex = 5;
        _splitterObjectExplorer.TabStop = false;
        // 
        // _splitterQuery
        // 
        _splitterQuery.Dock = DockStyle.Top;
        _splitterQuery.Location = new Point(303, 303);
        _splitterQuery.Name = "_splitterQuery";
        _splitterQuery.Size = new Size(713, 2);
        _splitterQuery.TabIndex = 7;
        _splitterQuery.TabStop = false;
        // 
        // _tabControl
        // 
        _tabControl.Dock = DockStyle.Fill;
        _tabControl.Location = new Point(303, 305);
        _tabControl.Name = "_tabControl";
        _tabControl.SelectedIndex = 0;
        _tabControl.ShowToolTips = true;
        _tabControl.Size = new Size(713, 238);
        _tabControl.TabIndex = 8;
        // 
        // _toolStrip
        // 
        _toolStrip.Dock = DockStyle.None;
        _toolStrip.Items.AddRange(
        [
            _toolStripSeparator4,
            _executeQuerySplitButton,
            _cancelQueryButton
        ]);
        _toolStrip.Location = new Point(303, 281);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Size = new Size(73, 25);
        _toolStrip.TabIndex = 9;
        _toolStrip.Text = "toolStrip1";
        // 
        // _toolStripSeparator4
        // 
        _toolStripSeparator4.Name = "_toolStripSeparator4";
        _toolStripSeparator4.Size = new Size(6, 25);
        // 
        // _executeQuerySplitButton
        // 
        _executeQuerySplitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _executeQuerySplitButton.DropDownItems.AddRange(
        [
            _executeQueryMenuItem,
            _executeQuerySingleRowToolStripMenuItem,
            _cToolStripMenuItem,
            _openTableToolStripMenuItem
        ]);
        _executeQuerySplitButton.Image = ((Image)(resources.GetObject("_executeQuerySplitButton.Image")));
        _executeQuerySplitButton.ImageTransparentColor = Color.Magenta;
        _executeQuerySplitButton.Name = "_executeQuerySplitButton";
        _executeQuerySplitButton.Size = new Size(32, 22);
        _executeQuerySplitButton.Text = "Execute Query";
        _executeQuerySplitButton.ButtonClick += new EventHandler(ToolStripSplitButton1_ButtonClick);
        // 
        // _executeQueryMenuItem
        // 
        _executeQueryMenuItem.Name = "_executeQueryMenuItem";
        _executeQueryMenuItem.ShortcutKeys = Keys.F5;
        _executeQueryMenuItem.Size = new Size(168, 22);
        _executeQueryMenuItem.Text = "Execute Query";
        _executeQueryMenuItem.Click += new EventHandler(AToolStripMenuItem_Click);
        // 
        // _executeQuerySingleRowToolStripMenuItem
        // 
        _executeQuerySingleRowToolStripMenuItem.Name = "_executeQuerySingleRowToolStripMenuItem";
        _executeQuerySingleRowToolStripMenuItem.Size = new Size(168, 22);
        _executeQuerySingleRowToolStripMenuItem.Text = "Single Row";
        _executeQuerySingleRowToolStripMenuItem.Click += new EventHandler(bToolStripMenuItem_Click);
        // 
        // _cToolStripMenuItem
        // 
        _cToolStripMenuItem.Name = "_cToolStripMenuItem";
        _cToolStripMenuItem.Size = new Size(168, 22);
        _cToolStripMenuItem.Text = "XML";
        // 
        // _openTableToolStripMenuItem
        // 
        _openTableToolStripMenuItem.Name = "_openTableToolStripMenuItem";
        _openTableToolStripMenuItem.Size = new Size(168, 22);
        _openTableToolStripMenuItem.Text = "Edit Rows2";
        _openTableToolStripMenuItem.Click += new EventHandler(EditRowsToolStripMenuItem_Click);
        // 
        // _cancelQueryButton
        // 
        _cancelQueryButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _cancelQueryButton.Enabled = false;
        _cancelQueryButton.Image = ((Image)(resources.GetObject("_cancelQueryButton.Image")));
        _cancelQueryButton.ImageTransparentColor = Color.Magenta;
        _cancelQueryButton.Name = "_cancelQueryButton";
        _cancelQueryButton.Size = new Size(23, 22);
        _cancelQueryButton.Text = "Cancel Executing Query";
        _cancelQueryButton.Click += new EventHandler(CancelExecutingQueryButton_Click);
        // 
        // _queryTextBox
        // 
        _queryTextBox.Dock = DockStyle.Top;
        _queryTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
        _queryTextBox.Location = new Point(303, 24);
        _queryTextBox.Name = "_queryTextBox";
        _queryTextBox.Size = new Size(713, 279);
        _queryTextBox.TabIndex = 1;
        _queryTextBox.TabSize = 4;
        // 
        // QueryForm
        // 
        AutoScaleBaseSize = new Size(7, 15);
        ClientSize = new Size(1016, 565);
        Controls.Add(_toolStrip);
        Controls.Add(_tabControl);
        Controls.Add(_splitterQuery);
        Controls.Add(_queryTextBox);
        Controls.Add(_splitterObjectExplorer);
        Controls.Add(_statusBar);
        Controls.Add(_tvObjectExplorer);
        Controls.Add(_mainMenu);
        Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
        Icon = ((Icon)(resources.GetObject("$this.Icon")));
        MainMenuStrip = _mainMenu;
        Name = "QueryForm";
        _mainMenu.ResumeLayout(false);
        _mainMenu.PerformLayout();
        _statusBar.ResumeLayout(false);
        _statusBar.PerformLayout();
        _toolStrip.ResumeLayout(false);
        _toolStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();

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
                var treeNode = new TreeNode(child.Name)
                {
                    Tag = child
                };
                //treeNode.ImageIndex

                if (!child.IsLeaf)
                    treeNode.Nodes.Add(new TreeNode());

                parent.Add(treeNode);
                count++;
            }
        }

        var ticks = Stopwatch.GetTimestamp() - startTimestamp;
        var items = ResultWriter.StringExtensions.SingularOrPlural(count, "item", "items");
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

    private void AppendMessageText(DateTime dateTime, InfoMessageSeverity severity, string? header, string text)
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

    private void SettingsChanged(object? sender, EventArgs e)
    {
        var folder = Settings.CurrentType;
        _commandTimeout = folder.Attributes["CommandTimeout"].GetValue<int>();
    }

    private void SetText()
    {
        var text = Provider.GetConnectionName(Connection!.Connection);
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
                    resultWriter = new LogResultWriter(AddInfoMessage, _showSchemaTable);
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

                    if (_colorTheme != null)
                        _colorTheme.Apply(resultSetTabPage);

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
        string? resultCommandText = null;

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
                if (string.Equals(sizeString, "max", StringComparison.InvariantCultureIgnoreCase))
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
        List<List<Token>> declarations = [];
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
        var tabPage = new TabPage(tabPageName)
        {
            ToolTipText = toolTipText
        };
        _tabControl.TabPages.Add(tabPage);
        tabPage.Controls.Add(control);
        _tabControl.SelectedTab = tabPage;
        // tabPage.Refresh();
    }

    private void Connection_DatabaseChanged(object? sender, DatabaseChangedEventArgs args)
    {
        if (InvokeRequired)
        {
            Invoke(() => Connection_DatabaseChanged(sender, args));
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
                    var tabControl = new TabControl
                    {
                        Dock = DockStyle.Fill
                    };
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
                        var cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1),
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
            Invoke(() => EndFillHandleException(ex));
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

    private void EndFillInvoker(IAsyncDataAdapter dataAdapter, Exception e) => Invoke(() => EndFill(dataAdapter, e));

    private void WriteEndInvoker(IAsyncDataAdapter dataAdapter) => Invoke(() => WriteEnd(dataAdapter));

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
    private readonly struct Tchittestinfo(int x, int y)
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