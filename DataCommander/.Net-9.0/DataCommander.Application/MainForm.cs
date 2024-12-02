using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataCommander.Application.Connection;
using DataCommander.Application.Query;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Log;
using Foundation.Threading;
using Foundation.Windows.Forms;
using Newtonsoft.Json;

namespace DataCommander.Application;

public class MainForm : Form
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly StringCollection _recentFileList = [];

    private MenuStrip? _mainMenu;
    private ToolStripMenuItem? _menuItem1;
    private ToolStripMenuItem? _mnuConnect;
    private ImageList? _imageList;
    private StatusStrip? _statusBar;
    private ToolStrip? _toolStrip;
    private ToolStripMenuItem? _mnuExit;
    private ToolStripMenuItem? _mnuHelp;
    private ToolStripMenuItem? _mnuAbout;
    private ToolStripMenuItem? _mnuOpen;
    private ToolStripButton? _btnConnect;
    private ToolStripMenuItem? _mnuWindow;
    private ToolStripMenuItem? _mnuRecentFileList;
    private ToolStripMenuItem? optionsMenuItem;
    private ToolStripButton? _openButton;
    private ToolStripButton? _saveButton;
    private ToolStripSeparator? _toolStripSeparator1;
    private ToolStripButton? _helpButton;
    private ToolStripMenuItem? _newToolStripMenuItem;
    private ToolStripMenuItem? _contentsToolStripMenuItem;
    private ToolStripSeparator? _toolStripSeparator2;
    private ToolStripPanel? _toolStripPanel;
    private ToolStripMenuItem? _closeAllDocumentsMenuItem;
    private IContainer? components;
    private ToolStripStatusLabel? _toolStripStatusLabel;
    private ToolStripMenuItem? _saveAllToolStripMenuItem;
    private ToolStripMenuItem? _recentConnectionsToolStripMenuItem;
    private ToolStripMenuItem? _checkForToolStripMenuItem;
    private ToolStripStatusLabel? _managedMemoryToolStripStatusLabel;
    private ToolStrip? _queryFormToolStrip;
    private readonly System.Windows.Forms.Timer _timer;
    private ColorTheme? _colorTheme;

    private void SetColorTheme(bool darkColorTheme)
    {
        var colorTheme = darkColorTheme
            ? new ColorTheme(
                Color.FromArgb(220, 220, 220),
                Color.FromArgb(30, 30, 30),
                Color.DarkOliveGreen,
                Color.FromArgb(86, 156, 214),
                Color.FromArgb(203, 65, 65))
            : null;

        _colorTheme = colorTheme;
    }

    public MainForm()
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        Text = "Data Commander";

        _helpButton!.Click += HelpButton_Click;
        _mnuAbout!.Click += mnuAbout_Click;

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
        LoadLayout();

        var start = Process.GetCurrentProcess().StartTime;
        var end = DateTime.Now;
        var elapsed = end - start;

        var message = $"Application loaded in {new StopwatchTimeSpan(elapsed).ToString(3)} seconds.";
        _toolStripStatusLabel!.Text = message;
        Log.Trace(message);

        if (!DataCommanderApplication.Instance.ApplicationData.CurrentType.Attributes.TryGetAttributeValue<bool>("DarkColorTheme", out var darkColorTheme))
            darkColorTheme = false;

        SetColorTheme(darkColorTheme);

        if (_colorTheme != null)
        {
            ForeColor = _colorTheme.ForeColor;
            BackColor = _colorTheme.BackColor;

            foreach (Control control in Controls)
            {
                control.ForeColor = _colorTheme.ForeColor;
                control.BackColor = _colorTheme.BackColor;
            }

            _toolStripPanel!.BackColor = _colorTheme.BackColor;

            _mainMenu!.ForeColor = _colorTheme.ForeColor;
            _mainMenu.BackColor = _colorTheme.BackColor;

            foreach (var menuItem in _mainMenu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>())
            foreach (ToolStripItem x in menuItem.DropDownItems)
                _colorTheme.Apply(x);

            _toolStrip!.BackColor = _colorTheme.BackColor;
            _toolStrip.ForeColor = _colorTheme.ForeColor;

            foreach (ToolStripItem item in _toolStrip.Items)
                _colorTheme.Apply(item);

            foreach (ToolStripItem item in _statusBar!.Items)
                _colorTheme.Apply(item);
        }

        UpdateTotalMemory();

        _timer = new System.Windows.Forms.Timer(components!)
        {
            Interval = 5000, // 10 seconds
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private static string BytesToText(long bytes)
    {
        var megaBytes = bytes / 1024.0 / 1024.0;
        var text = $"{Math.Round(megaBytes, 0)}";
        return text;
    }

    public void UpdateTotalMemory()
    {
        var totalMemory = GC.GetTotalMemory(false);
        var workingSet = Environment.WorkingSet;

        _managedMemoryToolStripStatusLabel!.Text = $"{BytesToText(totalMemory)} / {BytesToText(workingSet)} MB";

        _managedMemoryToolStripStatusLabel.ForeColor = totalMemory <= 256 * 1024 * 1024
            ? _colorTheme != null
                ? _colorTheme.ForeColor
                : SystemColors.ControlText
            : Color.Red;
    }

    private void Timer_Tick(object? sender, EventArgs e) => UpdateTotalMemory();

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new Container();
        var resources = new ComponentResourceManager(typeof(MainForm));
        _mainMenu = new MenuStrip();
        _menuItem1 = new ToolStripMenuItem();
        _newToolStripMenuItem = new ToolStripMenuItem();
        _mnuConnect = new ToolStripMenuItem();
        _mnuOpen = new ToolStripMenuItem();
        _recentConnectionsToolStripMenuItem = new ToolStripMenuItem();
        _saveAllToolStripMenuItem = new ToolStripMenuItem();
        _mnuRecentFileList = new ToolStripMenuItem();
        _mnuExit = new ToolStripMenuItem();
        optionsMenuItem = new ToolStripMenuItem();
        _mnuWindow = new ToolStripMenuItem();
        _closeAllDocumentsMenuItem = new ToolStripMenuItem();
        _mnuHelp = new ToolStripMenuItem();
        _contentsToolStripMenuItem = new ToolStripMenuItem();
        _checkForToolStripMenuItem = new ToolStripMenuItem();
        _mnuAbout = new ToolStripMenuItem();
        _toolStrip = new ToolStrip();
        _imageList = new ImageList(components);
        _btnConnect = new ToolStripButton();
        _openButton = new ToolStripButton();
        _saveButton = new ToolStripButton();
        _toolStripSeparator1 = new ToolStripSeparator();
        _helpButton = new ToolStripButton();
        _toolStripSeparator2 = new ToolStripSeparator();
        _activeMdiChildToolStripTextBox = new ToolStripTextBox();
        _statusBar = new StatusStrip();
        _toolStripStatusLabel = new ToolStripStatusLabel();
        _managedMemoryToolStripStatusLabel = new ToolStripStatusLabel();
        _toolStripPanel = new ToolStripPanel();
        _mainMenu.SuspendLayout();
        _toolStrip.SuspendLayout();
        _statusBar.SuspendLayout();
        _toolStripPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _mainMenu
        // 
        _mainMenu.Dock = DockStyle.None;
        _mainMenu.ImageScalingSize = new Size(20, 20);
        _mainMenu.Items.AddRange([_menuItem1, optionsMenuItem, _mnuWindow, _mnuHelp]);
        _mainMenu.Location = new Point(0, 27);
        _mainMenu.MdiWindowListItem = _mnuWindow;
        _mainMenu.Name = "_mainMenu";
        _mainMenu.Size = new Size(982, 28);
        _mainMenu.TabIndex = 1;
        // 
        // _menuItem1
        // 
        _menuItem1.DropDownItems.AddRange([_newToolStripMenuItem, _mnuConnect, _mnuOpen, _recentConnectionsToolStripMenuItem, _saveAllToolStripMenuItem, _mnuRecentFileList, _mnuExit]);
        _menuItem1.MergeIndex = 1;
        _menuItem1.Name = "_menuItem1";
        _menuItem1.Size = new Size(86, 24);
        _menuItem1.Text = "&Database";
        // 
        // _newToolStripMenuItem
        // 
        _newToolStripMenuItem.Name = "_newToolStripMenuItem";
        _newToolStripMenuItem.Size = new Size(235, 26);
        _newToolStripMenuItem.Text = "&Create";
        _newToolStripMenuItem.Click += CreateMenuItem_Click;
        // 
        // _mnuConnect
        // 
        _mnuConnect.Image = (Image)resources.GetObject("_mnuConnect.Image");
        _mnuConnect.MergeIndex = 0;
        _mnuConnect.Name = "_mnuConnect";
        _mnuConnect.ShortcutKeys = Keys.Control | Keys.N;
        _mnuConnect.Size = new Size(235, 26);
        _mnuConnect.Text = "&Connect";
        _mnuConnect.Click += MnuConnect_Click;
        // 
        // _mnuOpen
        // 
        _mnuOpen.Image = (Image)resources.GetObject("_mnuOpen.Image");
        _mnuOpen.MergeIndex = 1;
        _mnuOpen.Name = "_mnuOpen";
        _mnuOpen.ShortcutKeys = Keys.Control | Keys.O;
        _mnuOpen.Size = new Size(235, 26);
        _mnuOpen.Text = "&Open";
        _mnuOpen.Click += mnuOpen_Click;
        // 
        // _recentConnectionsToolStripMenuItem
        // 
        _recentConnectionsToolStripMenuItem.Name = "_recentConnectionsToolStripMenuItem";
        _recentConnectionsToolStripMenuItem.Size = new Size(235, 26);
        _recentConnectionsToolStripMenuItem.Text = "Recent connections";
        // 
        // _saveAllToolStripMenuItem
        // 
        _saveAllToolStripMenuItem.Name = "_saveAllToolStripMenuItem";
        _saveAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        _saveAllToolStripMenuItem.Size = new Size(235, 26);
        _saveAllToolStripMenuItem.Text = "Save All";
        _saveAllToolStripMenuItem.Click += saveAllToolStripMenuItem_Click;
        // 
        // _mnuRecentFileList
        // 
        _mnuRecentFileList.MergeIndex = 2;
        _mnuRecentFileList.Name = "_mnuRecentFileList";
        _mnuRecentFileList.Size = new Size(235, 26);
        _mnuRecentFileList.Text = "Recent &File List";
        // 
        // _mnuExit
        // 
        _mnuExit.Name = "_mnuExit";
        _mnuExit.ShortcutKeys = Keys.Alt | Keys.F4;
        _mnuExit.Size = new Size(235, 26);
        _mnuExit.Text = "Exit";
        _mnuExit.Click += MnuExit_Click;
        // 
        // optionsMenuItem
        // 
        optionsMenuItem.MergeIndex = 5;
        optionsMenuItem.Name = "optionsMenuItem";
        optionsMenuItem.Size = new Size(75, 24);
        optionsMenuItem.Text = "Options";
        optionsMenuItem.Click += optionsMenuItem_Click;
        // 
        // _mnuWindow
        // 
        _mnuWindow.DropDownItems.AddRange([_closeAllDocumentsMenuItem]);
        _mnuWindow.MergeIndex = 6;
        _mnuWindow.Name = "_mnuWindow";
        _mnuWindow.Size = new Size(78, 24);
        _mnuWindow.Text = "&Window";
        // 
        // _closeAllDocumentsMenuItem
        // 
        _closeAllDocumentsMenuItem.Name = "_closeAllDocumentsMenuItem";
        _closeAllDocumentsMenuItem.Size = new Size(229, 26);
        _closeAllDocumentsMenuItem.Text = "Close All Documents";
        _closeAllDocumentsMenuItem.Click += CloseAllDocumentsMenuItem_Click;
        // 
        // _mnuHelp
        // 
        _mnuHelp.DropDownItems.AddRange([_contentsToolStripMenuItem, _checkForToolStripMenuItem, _mnuAbout]);
        _mnuHelp.MergeIndex = 7;
        _mnuHelp.Name = "_mnuHelp";
        _mnuHelp.Size = new Size(55, 24);
        _mnuHelp.Text = "&Help";
        // 
        // _contentsToolStripMenuItem
        // 
        _contentsToolStripMenuItem.Name = "_contentsToolStripMenuItem";
        _contentsToolStripMenuItem.ShortcutKeys = Keys.F1;
        _contentsToolStripMenuItem.Size = new Size(247, 26);
        _contentsToolStripMenuItem.Text = "Contents";
        _contentsToolStripMenuItem.Click += contentsToolStripMenuItem_Click;
        // 
        // _checkForToolStripMenuItem
        // 
        _checkForToolStripMenuItem.Name = "_checkForToolStripMenuItem";
        _checkForToolStripMenuItem.ShortcutKeys = Keys.F12;
        _checkForToolStripMenuItem.Size = new Size(247, 26);
        _checkForToolStripMenuItem.Text = "Check for updates ";
        _checkForToolStripMenuItem.Click += CheckForToolStripMenuItem_Click;
        // 
        // _mnuAbout
        // 
        _mnuAbout.MergeIndex = 0;
        _mnuAbout.Name = "_mnuAbout";
        _mnuAbout.Size = new Size(247, 26);
        _mnuAbout.Text = "About...";
        // 
        // _toolStrip
        // 
        _toolStrip.Dock = DockStyle.None;
        _toolStrip.ImageList = _imageList;
        _toolStrip.ImageScalingSize = new Size(20, 20);
        _toolStrip.Items.AddRange([_btnConnect, _openButton, _saveButton, _toolStripSeparator1, _helpButton, _toolStripSeparator2, _activeMdiChildToolStripTextBox]);
        _toolStrip.Location = new Point(4, 0);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Size = new Size(792, 27);
        _toolStrip.TabIndex = 2;
        // 
        // _imageList
        // 
        _imageList.ColorDepth = ColorDepth.Depth8Bit;
        _imageList.ImageStream = (ImageListStreamer)resources.GetObject("_imageList.ImageStream");
        _imageList.TransparentColor = Color.Transparent;
        _imageList.Images.SetKeyName(0, "");
        _imageList.Images.SetKeyName(1, "");
        _imageList.Images.SetKeyName(2, "");
        _imageList.Images.SetKeyName(3, "");
        // 
        // _btnConnect
        // 
        _btnConnect.Image = (Image)resources.GetObject("_btnConnect.Image");
        _btnConnect.Name = "_btnConnect";
        _btnConnect.Size = new Size(29, 24);
        _btnConnect.ToolTipText = "Connect to database";
        _btnConnect.Click += btnConnect_Click;
        // 
        // _openButton
        // 
        _openButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _openButton.Image = (Image)resources.GetObject("_openButton.Image");
        _openButton.ImageTransparentColor = Color.Magenta;
        _openButton.Name = "_openButton";
        _openButton.Size = new Size(29, 24);
        _openButton.Text = "toolStripButton1";
        _openButton.ToolTipText = "Open database";
        _openButton.Click += openButton_Click;
        // 
        // _saveButton
        // 
        _saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _saveButton.Enabled = false;
        _saveButton.Image = (Image)resources.GetObject("_saveButton.Image");
        _saveButton.ImageTransparentColor = Color.Magenta;
        _saveButton.Name = "_saveButton";
        _saveButton.Size = new Size(29, 24);
        _saveButton.ToolTipText = "Save Query";
        _saveButton.Click += saveButton_Click;
        // 
        // _toolStripSeparator1
        // 
        _toolStripSeparator1.Name = "_toolStripSeparator1";
        _toolStripSeparator1.Size = new Size(6, 27);
        // 
        // _helpButton
        // 
        _helpButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _helpButton.Image = (Image)resources.GetObject("_helpButton.Image");
        _helpButton.ImageTransparentColor = Color.Magenta;
        _helpButton.Name = "_helpButton";
        _helpButton.Size = new Size(29, 24);
        _helpButton.Text = "Help";
        // 
        // _toolStripSeparator2
        // 
        _toolStripSeparator2.Name = "_toolStripSeparator2";
        _toolStripSeparator2.Size = new Size(6, 27);
        // 
        // _activeMdiChildToolStripTextBox
        // 
        _activeMdiChildToolStripTextBox.Name = "_activeMdiChildToolStripTextBox";
        _activeMdiChildToolStripTextBox.ReadOnly = true;
        _activeMdiChildToolStripTextBox.Size = new Size(610, 27);
        // 
        // _statusBar
        // 
        _statusBar.ImageScalingSize = new Size(20, 20);
        _statusBar.Items.AddRange([_toolStripStatusLabel, _managedMemoryToolStripStatusLabel]);
        _statusBar.Location = new Point(0, 727);
        _statusBar.Name = "_statusBar";
        _statusBar.ShowItemToolTips = true;
        _statusBar.Size = new Size(982, 26);
        _statusBar.TabIndex = 3;
        // 
        // _toolStripStatusLabel
        // 
        _toolStripStatusLabel.Name = "_toolStripStatusLabel";
        _toolStripStatusLabel.Size = new Size(867, 20);
        _toolStripStatusLabel.Spring = true;
        _toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _managedMemoryToolStripStatusLabel
        // 
        _managedMemoryToolStripStatusLabel.AutoSize = false;
        _managedMemoryToolStripStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
        _managedMemoryToolStripStatusLabel.Name = "_managedMemoryToolStripStatusLabel";
        _managedMemoryToolStripStatusLabel.Size = new Size(100, 20);
        _managedMemoryToolStripStatusLabel.TextAlign = ContentAlignment.MiddleRight;
        _managedMemoryToolStripStatusLabel.ToolTipText = "Managed memory / Working set";
        _managedMemoryToolStripStatusLabel.MouseUp += managedMemoryToolStripStatusLabel_MouseUp;
        // 
        // _toolStripPanel
        // 
        _toolStripPanel.Controls.Add(_toolStrip);
        _toolStripPanel.Controls.Add(_mainMenu);
        _toolStripPanel.Dock = DockStyle.Top;
        _toolStripPanel.Location = new Point(0, 0);
        _toolStripPanel.Name = "_toolStripPanel";
        _toolStripPanel.Orientation = Orientation.Horizontal;
        _toolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
        _toolStripPanel.Size = new Size(982, 55);
        // 
        // MainForm
        // 
        AutoScaleBaseSize = new Size(7, 17);
        ClientSize = new Size(982, 753);
        Controls.Add(_toolStripPanel);
        Controls.Add(_statusBar);
        Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
        Icon = (Icon)resources.GetObject("$this.Icon");
        IsMdiContainer = true;
        MainMenuStrip = _mainMenu;
        Name = "MainForm";
        StartPosition = FormStartPosition.Manual;
        _mainMenu.ResumeLayout(false);
        _mainMenu.PerformLayout();
        _toolStrip.ResumeLayout(false);
        _toolStrip.PerformLayout();
        _statusBar.ResumeLayout(false);
        _statusBar.PerformLayout();
        _toolStripPanel.ResumeLayout(false);
        _toolStripPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private void optionsMenuItem_Click(object? sender, EventArgs e)
    {
        var optionsForm = new OptionsForm(_colorTheme != null, SelectedFont, _colorTheme);
        if (optionsForm.ShowDialog() == DialogResult.OK)
        {
            var darkColorTheme = optionsForm.DarkColorTheme;
            SetColorTheme(darkColorTheme);
            SelectedFont = optionsForm.SelectedFont;

            var attributes = DataCommanderApplication.Instance.ApplicationData.CurrentType.Attributes;
            attributes.SetAttributeValue("DarkColorTheme", darkColorTheme);
            attributes.SetAttributeValue("Font", Serialize(SelectedFont));
        }
    }

    private void Connect()
    {
        try
        {
            var connectionForm = new ConnectionListForm(_colorTheme);

            if (connectionForm.ShowDialog() == DialogResult.OK)
            {
                Log.Trace(CallerInformation.Create(), "connectionForm.ShowDialog() finished.");
                var connectionInfo = connectionForm.ConnectionInfo;
                var providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == connectionInfo.ProviderIdentifier);
                var provider = ProviderFactory.CreateProvider(connectionInfo.ProviderIdentifier);
                var queryForm = new QueryForm(this, provider, connectionInfo, connectionForm.Connection, _statusBar, _colorTheme)
                {
                    MdiParent = this
                };

                if (SelectedFont != null)
                    queryForm.Font = SelectedFont;

                queryForm.FormClosing += queryForm_FormClosing;

                switch (WindowState)
                {
                    case FormWindowState.Normal:
                        //var width = Math.Max(ClientSize.Width + 70, 100);
                        //var height = Math.Max(ClientSize.Height - 120, 50);
                        //queryForm.ClientSize = new Size(width, height);
                        queryForm.WindowState = FormWindowState.Maximized;
                        break;

                    case FormWindowState.Maximized:
                        //queryForm.WindowState = FormWindowState.Maximized;
                        break;
                }

                var connectionStringBuilder = provider.CreateConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString;
                var connection = connectionForm.Connection;
                QueryFormStaticMethods.AddInfoMessageToQueryForm(queryForm, connectionForm.ElapsedTicks, connectionInfo.ConnectionName, providerInfo.Name, connection);
                queryForm.Show();

                if (WindowState == FormWindowState.Maximized)
                {
                    queryForm.WindowState = FormWindowState.Maximized;
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message);
        }
    }

    private void queryForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!e.Cancel && _queryFormToolStrip != null)
        {
            _toolStripPanel!.Controls.Remove(_queryFormToolStrip);
            _queryFormToolStrip = null;
        }
    }

    private void MnuConnect_Click(object? sender, EventArgs e) => Connect();

    private void MnuExit_Click(object? sender, EventArgs e) => Close();

    private void mnuAbout_Click(object? sender, EventArgs e)
    {
        var aboutForm = new AboutForm(_colorTheme);
        aboutForm.ShowDialog();
    }

    private void SaveLayout()
    {
        var applicationData = DataCommanderApplication.Instance.ApplicationData;
        FormPosition.Save(this, applicationData);
        var folder = applicationData.CurrentType;
        var array = new string[_recentFileList.Count];
        _recentFileList.CopyTo(array, 0);
        folder.Attributes.SetAttributeValue("RecentFileList", array);
    }

    private void mnuRecentFile_Click(object? sender, EventArgs e)
    {
        var menuItem = (ToolStripMenuItem)sender!;
        var index = _mnuRecentFileList!.DropDownItems.IndexOf(menuItem);
        var count = _recentFileList.Count;
        var path = _recentFileList[count - index - 1];
        LoadFiles([path]);
    }

    private void CreateRecentFileListMenu()
    {
        List<ToolStripMenuItem> menuItems = [];
        var count = _recentFileList.Count;
        for (var i = 0; i < count; i++)
        {
            var path = _recentFileList[count - i - 1];
            var text = $"{i + 1} {path}";
            var menuItem = new ToolStripMenuItem(text, null, mnuRecentFile_Click);
            menuItems.Add(menuItem);
        }

        _mnuRecentFileList!.DropDownItems.Clear();
        _mnuRecentFileList.DropDownItems.AddRange(menuItems.ToArray());
    }

    private void LoadLayout()
    {
        var applicationData = DataCommanderApplication.Instance.ApplicationData;
        FormPosition.Load(applicationData, this);
        var folder = applicationData.CurrentType;
        var contains = folder.Attributes.TryGetAttributeValue("RecentFileList", out string[] array);

        if (contains && array != null)
        {
            int i;

            for (i = 0; i < array.Length; i++)
                _recentFileList.Add(array[i]);
        }

        contains = folder.Attributes.TryGetAttributeValue("Font", out string base64);

        if (contains)
            SelectedFont = DeserializeFont(base64);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        SaveLayout();
        base.OnClosing(e);
    }

    private async void Open()
    {
        try
        {
            var fileDialog = new OpenFileDialog
            {
                Filter =
                "SQL script files(*.sql)|*.sql|Access Files(*.mdb)|*.mdb|Access 2007 Files(*.accdb)|*.accdb|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|Microsoft.ACE.OLEDB.16.0|*.*|MSI files (*.msi)|*.msi|SQLite files (*.*)|*.*|SQL Server Compact files (*.sdf)|*.sdf|SQL Server Compact 4.0 files (*.sdf)|*.sdf",
                RestoreDirectory = true
            };
            var currentDirectory = Environment.CurrentDirectory;

            if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (Environment.CurrentDirectory != currentDirectory)
                    Environment.CurrentDirectory = currentDirectory;

                var fileName = fileDialog.FileName;
                var extension = Path.GetExtension(fileName).ToLower();
                string? connectionString = null;
                IProvider? provider = null;

                switch (fileDialog.FilterIndex)
                {
                    case 1:
                        LoadFiles(fileDialog.FileNames);
                        break;

                    case 2:
                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName;
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.OleDb);
                        break;

                    case 3:
                        connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Persist Security Info=False";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.OleDb);
                        break;

                    case 4:
                        if (extension == ".xls")
                            connectionString = Environment.Is64BitProcess
                                ? $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 8.0"
                                : $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Extended Properties=Excel 8.0";
                        else
                            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 12.0";

                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.OleDb);
                        break;
                    
                    case 5:
                        connectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={fileName}";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.OleDb);
                        break;

                    case 6:
                        connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.Msi);
                        break;

                    case 7:
                        connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.SqLite);
                        break;

                    case 8:
                        connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.SqlServerCe40);
                        break;

                    case 9:
                        connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                        provider = ProviderFactory.CreateProvider(ProviderIdentifier.SqlServerCe40);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                if (provider != null)
                {
                    var connectionStringAndCredential = new ConnectionStringAndCredential(connectionString, null);
                    var connection = provider.CreateConnection(connectionStringAndCredential);
                    await connection.OpenAsync(CancellationToken.None);
                    var connectionInfo = new ConnectionInfo(null, provider.Identifier, connectionStringAndCredential);

                    var connectionInfos = ConnectionInfoRepository.Get().ToList();
                    connectionInfos.Add(connectionInfo);
                    ConnectionInfoRepository.Save(connectionInfos);

                    var queryForm = new QueryForm(this, provider, connectionInfo, connection, _statusBar, _colorTheme)
                    {
                        MdiParent = this,
                        Font = SelectedFont
                    };
                    queryForm.Show();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Write(LogLevel.Error, ex.ToLogString());
            MessageBox.Show(this, ex.ToString());
        }
    }

    private void mnuOpen_Click(object? sender, EventArgs e) => Open();

    public void LoadFiles(string[] fileNames)
    {
        var i = fileNames.Length - 1;
        var path = fileNames[i];
        var queryForm = (QueryForm)ActiveMdiChild!;
        queryForm.LoadFile(path);

        var index = _recentFileList.IndexOf(path);

        if (index >= 0)
            _recentFileList.RemoveAt(index);

        _recentFileList.Add(path);
        CreateRecentFileListMenu();
    }

    public StatusStrip StatusBar => _statusBar;

    private static string Serialize(Font font)
    {
        var serializedFont = JsonConvert.SerializeObject(font);
        return serializedFont;
    }

    private static Font DeserializeFont(string serializedFont)
    {
        var font = serializedFont != null
            ? JsonConvert.DeserializeObject<Font>(serializedFont)
            : null;
        return font;
    }

    public Font SelectedFont { get; private set; }

    private void btnConnect_Click(object? sender, EventArgs e) => Connect();

    private void openButton_Click(object? sender, EventArgs e) => Open();

    private void saveButton_Click(object? sender, EventArgs e)
    {
        var queryForm = (QueryForm)ActiveMdiChild!;

        if (queryForm != null)
            queryForm.Save();
    }

    private void HelpButton_Click(object? sender, EventArgs e) => ShowContents();

    private async void CreateMenuItem_Click(object? sender, EventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "SQL Server Compact 4.0 files (*.sdf)|*.sdf|SQLite files (*.sqlite)|*.sqlite"
        };

        var result = dialog.ShowDialog();

        DbConnectionStringBuilder sb = [];

        if (result == DialogResult.OK)
        {
            sb.Add(ConnectionStringKeyword.DataSource, dialog.FileName);

            string connectionString;
            string providerIdentifier;

            switch (dialog.FilterIndex)
            {
                case 1:
                    providerIdentifier = ProviderIdentifier.SqlServerCe40;
                    connectionString = sb.ConnectionString;
                    var engine = new SqlCeEngine(connectionString);
                    engine.CreateDatabase();
                    break;

                case 2:
                    providerIdentifier = ProviderIdentifier.SqLite;
                    connectionString = sb.ConnectionString;
                    break;

                default:
                    throw new Exception();
            }

            var provider = ProviderFactory.CreateProvider(providerIdentifier);
            Assert.IsNotNull(provider);

            var connectionStringAndCredential = new ConnectionStringAndCredential(connectionString, null);
            var connectionInfo = new ConnectionInfo(null, providerIdentifier, connectionStringAndCredential);
            var connection = provider.CreateConnection(connectionStringAndCredential);
            await connection.OpenAsync(CancellationToken.None);

            var queryForm = new QueryForm(this, provider, connectionInfo, connection, _statusBar, _colorTheme)
            {
                MdiParent = this,
                Font = SelectedFont
            };
            queryForm.Show();
        }
    }

    private static void ShowContents()
    {
        const string url = "https://github.com/csbernath/DataCommander/blob/master/README.md";
        var processStartInfo = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(processStartInfo);
    }

    private void contentsToolStripMenuItem_Click(object? sender, EventArgs e) => ShowContents();

    protected override void OnMdiChildActivate(EventArgs e)
    {
        base.OnMdiChildActivate(e);

        _activeMdiChildToolStripTextBox!.Text = ActiveMdiChild?.Text;
        _saveButton!.Enabled = ActiveMdiChild != null;

        if (ActiveMdiChild != null)
        {
            _toolStripPanel!.ResumeLayout(false);
            if (_queryFormToolStrip != null)
                _toolStripPanel.Controls.Remove(_queryFormToolStrip);

            var queryForm = (QueryForm)ActiveMdiChild;
            var queryFormToolStrip = queryForm.ToolStrip;
            if (queryFormToolStrip != null)
            {
                queryFormToolStrip.Visible = true;
                var location = new Point(_toolStrip!.Right, _toolStrip.Top);
                _toolStripPanel.Join(queryFormToolStrip, location);
                _toolStripPanel.PerformLayout();

                _queryFormToolStrip = queryFormToolStrip;
            }

            CreateRecentFileListMenu();
        }
        else
            _mnuRecentFileList!.DropDownItems.Clear();
    }

    private void CloseAllDocumentsMenuItem_Click(object? sender, EventArgs e)
    {
        while (true)
        {
            var mdiChildren = MdiChildren;
            var length = mdiChildren.Length;
            if (length == 0)
                break;

            var mdiChild = mdiChildren[length - 1];
            mdiChild.Close();

            if (MdiChildren.Length == length)
                break;
        }
    }

    internal void SaveAll()
    {
        Cursor = Cursors.WaitCursor;
        _toolStripStatusLabel!.Text = "Saving all items...";
        Log.Write(LogLevel.Trace, "Saving all items...");

        var fileNamePrefix = Path.GetTempPath() + "DataCommander.SaveAll." + '[' + DateTime.Now.ToString("yyyyMMddHHmmss.fff") + ']';
        var index = 1;
        foreach (var mdiChild in MdiChildren)
        {
            if (mdiChild is QueryForm queryForm)
            {
                var text = queryForm.QueryTextBox.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    var fileName = fileNamePrefix + '[' + index.ToString().PadLeft(3, '0') + "].sql";
                    text = text.Replace("\n", "\r\n");
                    File.WriteAllText(fileName, text, Encoding.UTF8);
                    index++;
                }
            }
        }

        _toolStripStatusLabel.Text = $"All items saved to {fileNamePrefix}.";
        Cursor = Cursors.Default;
    }

    private void saveAllToolStripMenuItem_Click(object? sender, EventArgs e) => SaveAll();

    private ToolStripTextBox? _activeMdiChildToolStripTextBox;
    public ToolStripTextBox ActiveMdiChildToolStripTextBox => _activeMdiChildToolStripTextBox!;

    private void CheckForToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        const string url = "https://github.com/csbernath/DataCommander/releases";
        var processStartInfo = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(processStartInfo);
    }

    private void managedMemoryToolStripStatusLabel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var menu = new ContextMenuStrip(components);

            var menuItem = new ToolStripMenuItem("Collect garbage");
            menuItem.Click += CollectGarbage_Click;
            menu.Items.Add(menuItem);

            var bounds = _managedMemoryToolStripStatusLabel!.Bounds;
            var location = e.Location;
            menu.Show(_statusBar, bounds.X + location.X, bounds.Y + location.Y);
        }
    }

    private void CollectGarbage_Click(object? sender, EventArgs e)
    {
        GC.Collect();

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.Append(GarbageMonitor.Default.State);
        stringBuilder.AppendLine();
        stringBuilder.Append(ThreadMonitor.ToStringTableString());
        stringBuilder.AppendLine();
        stringBuilder.Append(AppDomainMonitor.GetCurrentDomainState());
        Log.Trace(stringBuilder.ToString());

        ThreadMonitor.Join(0);
    }
}