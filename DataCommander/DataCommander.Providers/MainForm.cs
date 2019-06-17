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
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataCommander.Providers.Connection;
using DataCommander.Providers.Query;
using Foundation.Assertions;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Threading;
using Foundation.Windows.Forms;

namespace DataCommander.Providers
{
    public class MainForm : Form
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly StringCollection _recentFileList = new StringCollection();

        private MenuStrip _mainMenu;
        private ToolStripMenuItem _menuItem1;
        private ToolStripMenuItem _mnuConnect;
        private ImageList _imageList;
        private StatusStrip _statusBar;
        private ToolStrip _toolStrip;
        private ToolStripMenuItem _mnuExit;
        private ToolStripMenuItem _mnuHelp;
        private ToolStripMenuItem _mnuAbout;
        private ToolStripMenuItem _mnuOpen;
        private ToolStripButton _btnConnect;
        private ToolStripMenuItem _mnuWindow;
        private ToolStripMenuItem _mnuRecentFileList;
        private ToolStripMenuItem optionsMenuItem;
        private ToolStripButton _openButton;
        private ToolStripButton _saveButton;
        private ToolStripSeparator _toolStripSeparator1;
        private ToolStripButton _helpButton;
        private ToolStripMenuItem _newToolStripMenuItem;
        private ToolStripMenuItem _contentsToolStripMenuItem;
        private ToolStripSeparator _toolStripSeparator2;
        private ToolStripPanel _toolStripPanel;
        private ToolStripMenuItem _closeAllDocumentsMenuItem;
        private IContainer components;
        private ToolStripStatusLabel _toolStripStatusLabel;
        private ToolStripMenuItem _saveAllToolStripMenuItem;
        private ToolStripMenuItem _recentConnectionsToolStripMenuItem;
        private ToolStripMenuItem _checkForToolStripMenuItem;
        private ToolStripStatusLabel _managedMemoryToolStripStatusLabel;
        private ToolStrip _queryFormToolStrip;
        private readonly System.Windows.Forms.Timer _timer;
        private ColorTheme _colorTheme;
        private bool _first = true;

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

            _helpButton.Click += helpButton_Click;
            _mnuAbout.Click += mnuAbout_Click;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            LoadLayout();

            var start = Process.GetCurrentProcess().StartTime;
            var end = DateTime.Now;
            var elapsed = end - start;

            var message = $"Current user: {WindowsIdentity.GetCurrent().Name}. Application loaded in {new StopwatchTimeSpan(elapsed).ToString(3)} seconds.";
            _toolStripStatusLabel.Text = message;
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

                _toolStripPanel.BackColor = _colorTheme.BackColor;

                _mainMenu.ForeColor = _colorTheme.ForeColor;
                _mainMenu.BackColor = _colorTheme.BackColor;

                foreach (var menuItem in _mainMenu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>())
                {
                    foreach (ToolStripItem x in menuItem.DropDownItems)
                    {
                        x.ForeColor = _colorTheme.ForeColor;
                        x.BackColor = _colorTheme.BackColor;
                    }
                }

                _toolStrip.BackColor = _colorTheme.BackColor;
                _toolStrip.ForeColor = _colorTheme.ForeColor;

                foreach (ToolStripItem item in _toolStrip.Items)
                {
                    item.ForeColor = _colorTheme.ForeColor;
                    item.BackColor = _colorTheme.BackColor;
                }

                foreach (ToolStripItem item in _statusBar.Items)
                {
                    item.ForeColor = _colorTheme.ForeColor;
                    item.BackColor = _colorTheme.BackColor;
                }
            }

            UpdateTotalMemory();

            _timer = new System.Windows.Forms.Timer(components)
            {
                Interval = 5000, // 10 seconds
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void UpdateTotalMemory()
        {
            var totalMemory = GC.GetTotalMemory(false);
            var totalMemoryMb = (double) totalMemory / 1024.0 / 1024.0;
            var text = $"{Math.Round(totalMemoryMb, 0)} MB";

            _managedMemoryToolStripStatusLabel.Text = text;

            _managedMemoryToolStripStatusLabel.ForeColor = totalMemoryMb <= 256
                ? _colorTheme != null
                    ? _colorTheme.ForeColor
                    : SystemColors.ControlText
                : Color.Red;
        }

        private void Timer_Tick(object sender, EventArgs e) => UpdateTotalMemory();

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._mainMenu = new System.Windows.Forms.MenuStrip();
            this._menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this._recentConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuRecentFileList = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this._closeAllDocumentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this._contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._checkForToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._btnConnect = new System.Windows.Forms.ToolStripButton();
            this._openButton = new System.Windows.Forms.ToolStripButton();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._helpButton = new System.Windows.Forms.ToolStripButton();
            this._toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._activeMdiChildToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this._statusBar = new System.Windows.Forms.StatusStrip();
            this._toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._managedMemoryToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._toolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this._mainMenu.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this._statusBar.SuspendLayout();
            this._toolStripPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainMenu
            // 
            this._mainMenu.Dock = System.Windows.Forms.DockStyle.None;
            this._mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItem1,
            this.optionsMenuItem,
            this._mnuWindow,
            this._mnuHelp});
            this._mainMenu.Location = new System.Drawing.Point(0, 0);
            this._mainMenu.MdiWindowListItem = this._mnuWindow;
            this._mainMenu.Name = "_mainMenu";
            this._mainMenu.Size = new System.Drawing.Size(982, 24);
            this._mainMenu.TabIndex = 1;
            // 
            // _menuItem1
            // 
            this._menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._newToolStripMenuItem,
            this._mnuConnect,
            this._mnuOpen,
            this._recentConnectionsToolStripMenuItem,
            this._saveAllToolStripMenuItem,
            this._mnuRecentFileList,
            this._mnuExit});
            this._menuItem1.MergeIndex = 1;
            this._menuItem1.Name = "_menuItem1";
            this._menuItem1.Size = new System.Drawing.Size(67, 20);
            this._menuItem1.Text = "&Database";
            // 
            // _newToolStripMenuItem
            // 
            this._newToolStripMenuItem.Name = "_newToolStripMenuItem";
            this._newToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this._newToolStripMenuItem.Text = "&Create";
            this._newToolStripMenuItem.Click += new System.EventHandler(this.CreateMenuItem_Click);
            // 
            // _mnuConnect
            // 
            this._mnuConnect.Image = ((System.Drawing.Image)(resources.GetObject("_mnuConnect.Image")));
            this._mnuConnect.MergeIndex = 0;
            this._mnuConnect.Name = "_mnuConnect";
            this._mnuConnect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this._mnuConnect.Size = new System.Drawing.Size(191, 26);
            this._mnuConnect.Text = "&Connect";
            this._mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // _mnuOpen
            // 
            this._mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("_mnuOpen.Image")));
            this._mnuOpen.MergeIndex = 1;
            this._mnuOpen.Name = "_mnuOpen";
            this._mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this._mnuOpen.Size = new System.Drawing.Size(191, 26);
            this._mnuOpen.Text = "&Open";
            this._mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // _recentConnectionsToolStripMenuItem
            // 
            this._recentConnectionsToolStripMenuItem.Name = "_recentConnectionsToolStripMenuItem";
            this._recentConnectionsToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this._recentConnectionsToolStripMenuItem.Text = "Recent connections";
            // 
            // _saveAllToolStripMenuItem
            // 
            this._saveAllToolStripMenuItem.Name = "_saveAllToolStripMenuItem";
            this._saveAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this._saveAllToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this._saveAllToolStripMenuItem.Text = "Save All";
            this._saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // _mnuRecentFileList
            // 
            this._mnuRecentFileList.MergeIndex = 2;
            this._mnuRecentFileList.Name = "_mnuRecentFileList";
            this._mnuRecentFileList.Size = new System.Drawing.Size(191, 26);
            this._mnuRecentFileList.Text = "Recent &File List";
            // 
            // _mnuExit
            // 
            this._mnuExit.Name = "_mnuExit";
            this._mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this._mnuExit.Size = new System.Drawing.Size(191, 26);
            this._mnuExit.Text = "Exit";
            this._mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.MergeIndex = 5;
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsMenuItem.Text = "Options";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // _mnuWindow
            // 
            this._mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._closeAllDocumentsMenuItem});
            this._mnuWindow.MergeIndex = 6;
            this._mnuWindow.Name = "_mnuWindow";
            this._mnuWindow.Size = new System.Drawing.Size(63, 20);
            this._mnuWindow.Text = "&Window";
            // 
            // _closeAllDocumentsMenuItem
            // 
            this._closeAllDocumentsMenuItem.Name = "_closeAllDocumentsMenuItem";
            this._closeAllDocumentsMenuItem.Size = new System.Drawing.Size(184, 22);
            this._closeAllDocumentsMenuItem.Text = "Close All Documents";
            this._closeAllDocumentsMenuItem.Click += new System.EventHandler(this.closeAllDocumentsMenuItem_Click);
            // 
            // _mnuHelp
            // 
            this._mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._contentsToolStripMenuItem,
            this._checkForToolStripMenuItem,
            this._mnuAbout});
            this._mnuHelp.MergeIndex = 7;
            this._mnuHelp.Name = "_mnuHelp";
            this._mnuHelp.Size = new System.Drawing.Size(44, 20);
            this._mnuHelp.Text = "&Help";
            // 
            // _contentsToolStripMenuItem
            // 
            this._contentsToolStripMenuItem.Name = "_contentsToolStripMenuItem";
            this._contentsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this._contentsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this._contentsToolStripMenuItem.Text = "Contents";
            this._contentsToolStripMenuItem.Click += new System.EventHandler(this.contentsToolStripMenuItem_Click);
            // 
            // _checkForToolStripMenuItem
            // 
            this._checkForToolStripMenuItem.Name = "_checkForToolStripMenuItem";
            this._checkForToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this._checkForToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this._checkForToolStripMenuItem.Text = "Check for updates ";
            this._checkForToolStripMenuItem.Click += new System.EventHandler(this.checkForToolStripMenuItem_Click);
            // 
            // _mnuAbout
            // 
            this._mnuAbout.MergeIndex = 0;
            this._mnuAbout.Name = "_mnuAbout";
            this._mnuAbout.Size = new System.Drawing.Size(198, 22);
            this._mnuAbout.Text = "About...";
            // 
            // _toolStrip
            // 
            this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._toolStrip.ImageList = this._imageList;
            this._toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnConnect,
            this._openButton,
            this._saveButton,
            this._toolStripSeparator1,
            this._helpButton,
            this._toolStripSeparator2,
            this._activeMdiChildToolStripTextBox});
            this._toolStrip.Location = new System.Drawing.Point(3, 24);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(522, 27);
            this._toolStrip.TabIndex = 2;
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "");
            this._imageList.Images.SetKeyName(1, "");
            this._imageList.Images.SetKeyName(2, "");
            this._imageList.Images.SetKeyName(3, "");
            // 
            // _btnConnect
            // 
            this._btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("_btnConnect.Image")));
            this._btnConnect.Name = "_btnConnect";
            this._btnConnect.Size = new System.Drawing.Size(24, 24);
            this._btnConnect.ToolTipText = "Connect to database";
            this._btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // _openButton
            // 
            this._openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._openButton.Image = ((System.Drawing.Image)(resources.GetObject("_openButton.Image")));
            this._openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openButton.Name = "_openButton";
            this._openButton.Size = new System.Drawing.Size(24, 24);
            this._openButton.Text = "toolStripButton1";
            this._openButton.ToolTipText = "Open database";
            this._openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._saveButton.Enabled = false;
            this._saveButton.Image = ((System.Drawing.Image)(resources.GetObject("_saveButton.Image")));
            this._saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(24, 24);
            this._saveButton.ToolTipText = "Save Query";
            this._saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // _toolStripSeparator1
            // 
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            this._toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // _helpButton
            // 
            this._helpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._helpButton.Image = ((System.Drawing.Image)(resources.GetObject("_helpButton.Image")));
            this._helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(24, 24);
            this._helpButton.Text = "Help";
            // 
            // _toolStripSeparator2
            // 
            this._toolStripSeparator2.Name = "_toolStripSeparator2";
            this._toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // _activeMdiChildToolStripTextBox
            // 
            this._activeMdiChildToolStripTextBox.Name = "_activeMdiChildToolStripTextBox";
            this._activeMdiChildToolStripTextBox.ReadOnly = true;
            this._activeMdiChildToolStripTextBox.Size = new System.Drawing.Size(400, 27);
            // 
            // _statusBar
            // 
            this._statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripStatusLabel,
            this._managedMemoryToolStripStatusLabel});
            this._statusBar.Location = new System.Drawing.Point(0, 731);
            this._statusBar.Name = "_statusBar";
            this._statusBar.ShowItemToolTips = true;
            this._statusBar.Size = new System.Drawing.Size(982, 22);
            this._statusBar.TabIndex = 3;
            // 
            // _toolStripStatusLabel
            // 
            this._toolStripStatusLabel.Name = "_toolStripStatusLabel";
            this._toolStripStatusLabel.Size = new System.Drawing.Size(867, 17);
            this._toolStripStatusLabel.Spring = true;
            this._toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _managedMemoryToolStripStatusLabel
            // 
            this._managedMemoryToolStripStatusLabel.AutoSize = false;
            this._managedMemoryToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._managedMemoryToolStripStatusLabel.Name = "_managedMemoryToolStripStatusLabel";
            this._managedMemoryToolStripStatusLabel.Size = new System.Drawing.Size(100, 17);
            this._managedMemoryToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._managedMemoryToolStripStatusLabel.ToolTipText = "Managed memory";
            this._managedMemoryToolStripStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.managedMemoryToolStripStatusLabel_MouseUp);
            // 
            // _toolStripPanel
            // 
            this._toolStripPanel.Controls.Add(this._mainMenu);
            this._toolStripPanel.Controls.Add(this._toolStrip);
            this._toolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._toolStripPanel.Location = new System.Drawing.Point(0, 0);
            this._toolStripPanel.Name = "_toolStripPanel";
            this._toolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._toolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this._toolStripPanel.Size = new System.Drawing.Size(982, 51);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(982, 753);
            this.Controls.Add(this._toolStripPanel);
            this.Controls.Add(this._statusBar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this._mainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Data Commander";
            this._mainMenu.ResumeLayout(false);
            this._mainMenu.PerformLayout();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this._statusBar.ResumeLayout(false);
            this._statusBar.PerformLayout();
            this._toolStripPanel.ResumeLayout(false);
            this._toolStripPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            var optionsForm = new OptionsForm(_colorTheme != null, SelectedFont);
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

        #endregion

        private void Connect()
        {
            var connectionForm = new ConnectionForm(_statusBar, _colorTheme);

            if (connectionForm.ShowDialog() == DialogResult.OK)
            {
                var connectionProperties = connectionForm.ConnectionProperties;

                var queryForm = new QueryForm(this, MdiChildren.Length, connectionProperties.Provider, connectionProperties.ConnectionString,
                    connectionProperties.Connection, _statusBar, _colorTheme);

                queryForm.MdiParent = this;

                if (SelectedFont != null)
                    queryForm.Font = SelectedFont;

                queryForm.FormClosing += queryForm_FormClosing;

                switch (WindowState)
                {
                    case FormWindowState.Normal:
                        var width = Math.Max(ClientSize.Width + 70, 100);
                        var height = Math.Max(ClientSize.Height - 120, 50);
                        queryForm.ClientSize = new Size(width, height);
                        break;

                    case FormWindowState.Maximized:
                        //queryForm.WindowState = FormWindowState.Maximized;
                        break;

                    default:
                        break;
                }

                var message = $@"Connection opened in {StopwatchTimeSpan.ToString(connectionForm.Duration, 3)} seconds.
ServerVersion: {connectionProperties.Connection.ServerVersion}";

                var infoMessage = InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message);
                queryForm.AddInfoMessage(infoMessage);

                queryForm.Show();

                if (WindowState == FormWindowState.Maximized)
                {
                    queryForm.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void queryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel)
            {
                var form = (Form) sender;
                // form.MdiParent = null;

                if (_queryFormToolStrip != null)
                {
                    _toolStripPanel.Controls.Remove(_queryFormToolStrip);
                    _queryFormToolStrip = null;
                }
            }
        }

        private void mnuConnect_Click(object sender, EventArgs e) => Connect();

        private void mnuExit_Click(object sender, EventArgs e) => Close();

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();

            //MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void mnuRecentFile_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem) sender;
            var index = _mnuRecentFileList.DropDownItems.IndexOf(menuItem);
            var count = _recentFileList.Count;
            var path = _recentFileList[count - index - 1];
            LoadFiles(new[] {path});
        }

        private void CreateRecentFileListMenu()
        {
            var menuItems = new List<ToolStripMenuItem>();
            var count = _recentFileList.Count;
            for (var i = 0; i < count; i++)
            {
                var path = _recentFileList[count - i - 1];
                var text = $"{i + 1} {path}";
                var menuItem = new ToolStripMenuItem(text, null, mnuRecentFile_Click);
                menuItems.Add(menuItem);
            }

            _mnuRecentFileList.DropDownItems.Clear();
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
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter =
                    "SQL script files(*.sql)|*.sql|Access Files(*.mdb)|*.mdb|Access 2007 Files(*.accdb)|*.accdb|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|MSI files (*.msi)|*.msi|SQLite files (*.*)|*.*|SQL Server Compact files (*.sdf)|*.sdf|SQL Server Compact 4.0 files (*.sdf)|*.sdf";
                fileDialog.RestoreDirectory = true;
                var currentDirectory = Environment.CurrentDirectory;

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (Environment.CurrentDirectory != currentDirectory)
                        Environment.CurrentDirectory = currentDirectory;

                    var fileName = fileDialog.FileName;
                    var extension = Path.GetExtension(fileName).ToLower();
                    string connectionString = null;
                    IProvider provider = null;

                    switch (fileDialog.FilterIndex)
                    {
                        case 1:
                            LoadFiles(fileDialog.FileNames);
                            break;

                        case 2:
                            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName;
                            provider = ProviderFactory.CreateProvider(ProviderName.OleDb);
                            break;

                        case 3:
                            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Persist Security Info=False";
                            provider = ProviderFactory.CreateProvider(ProviderName.OleDb);
                            break;

                        case 4:
                            if (extension == ".xls")
                                connectionString = Environment.Is64BitProcess
                                    ? $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 8.0"
                                    : $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Extended Properties=Excel 8.0";
                            else
                                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 12.0";

                            provider = ProviderFactory.CreateProvider(ProviderName.OleDb);
                            break;

                        case 5:
                            connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                            provider = ProviderFactory.CreateProvider("Msi");
                            break;

                        case 6:
                            connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                            provider = ProviderFactory.CreateProvider(ProviderName.SqLite);
                            break;

                        case 7:
                            connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                            provider = ProviderFactory.CreateProvider("SqlServerCe");
                            break;

                        case 8:
                            connectionString = $"{ConnectionStringKeyword.DataSource}={fileName}";
                            provider = ProviderFactory.CreateProvider(ProviderName.SqlServerCe40);
                            break;

                        default:
                            throw new NotSupportedException();
                    }

                    if (provider != null)
                    {
                        var connection = provider.CreateConnection(connectionString);
                        await connection.OpenAsync(CancellationToken.None);

                        var connectionProperties = new ConnectionProperties
                        {
                            ConnectionName = null,
                            ProviderName = provider.Name,
                            ConnectionString = connectionString
                        };

                        var node = DataCommanderApplication.Instance.ConnectionsConfigurationNode;
                        var subNode = new ConfigurationNode(null);
                        node.AddChildNode(subNode);
                        ConnectionPropertiesRepository.Save(connectionProperties,subNode);

                        var queryForm = new QueryForm(this, MdiChildren.Length, provider, connectionString, connection, _statusBar, _colorTheme);

                        queryForm.MdiParent = this;
                        queryForm.Font = SelectedFont;
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

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            Open();
        }

        public void LoadFiles(string[] fileNames)
        {
            var i = fileNames.Length - 1;
            var path = fileNames[i];
            var queryForm = (QueryForm) ActiveMdiChild;
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
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, font);
            var bytes = memoryStream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static Font DeserializeFont(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var memoryStream = new MemoryStream(bytes);
            var binaryFormatter = new BinaryFormatter();
            var obj = binaryFormatter.Deserialize(memoryStream);
            var font = (Font) obj;
            return font;
        }

        public Font SelectedFont { get; private set; }

        private void btnConnect_Click(object sender, EventArgs e) => Connect();

        private void openButton_Click(object sender, EventArgs e) => Open();

        private void saveButton_Click(object sender, EventArgs e)
        {
            var queryForm = (QueryForm) ActiveMdiChild;

            if (queryForm != null)
                queryForm.Save();
        }

        private void helpButton_Click(object sender, EventArgs e) => ShowContents();

        private async void CreateMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "SQL Server Compact 4.0 files (*.sdf)|*.sdf|SQLite files (*.sqlite)|*.sqlite";

            var result = dialog.ShowDialog();

            var sb = new DbConnectionStringBuilder();

            if (result == DialogResult.OK)
            {
                sb.Add(ConnectionStringKeyword.DataSource, dialog.FileName);

                string connectionString;
                string providerName;

                switch (dialog.FilterIndex)
                {
                    case 1:
                        providerName = ProviderName.SqlServerCe40;
                        connectionString = sb.ConnectionString;
                        var engine = new SqlCeEngine(connectionString);
                        engine.CreateDatabase();
                        break;

                    case 2:
                        providerName = ProviderName.SqLite;
                        connectionString = sb.ConnectionString;
                        break;

                    default:
                        throw new Exception();
                }

                var provider = ProviderFactory.CreateProvider(providerName);
                Assert.IsTrue(provider != null);

                var connection = provider.CreateConnection(connectionString);
                await connection.OpenAsync(CancellationToken.None);

                var queryForm = new QueryForm(this, MdiChildren.Length, provider, connectionString, connection, _statusBar, _colorTheme);
                queryForm.MdiParent = this;
                queryForm.Font = SelectedFont;
                queryForm.Show();
            }
        }

        private void ShowContents()
        {
            const string url = "https://github.com/csbernath/DataCommander/blob/master/README.md";
            Process.Start(url);
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e) => ShowContents();

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);

            _activeMdiChildToolStripTextBox.Text = ActiveMdiChild != null ? ActiveMdiChild.Text : null;
            _saveButton.Enabled = ActiveMdiChild != null;

            if (ActiveMdiChild != null)
            {
                _toolStripPanel.ResumeLayout(false);
                if (_queryFormToolStrip != null)
                    _toolStripPanel.Controls.Remove(_queryFormToolStrip);

                var queryForm = (QueryForm) ActiveMdiChild;
                var queryFormToolStrip = queryForm.ToolStrip;
                if (queryFormToolStrip != null)
                {
                    queryFormToolStrip.Visible = true;
                    var location = new Point(_toolStrip.Right, _toolStrip.Top);
                    _toolStripPanel.Join(queryFormToolStrip, location);
                    _toolStripPanel.PerformLayout();

                    _queryFormToolStrip = queryFormToolStrip;
                }

                CreateRecentFileListMenu();
            }
            else
                _mnuRecentFileList.DropDownItems.Clear();
        }

        private void closeAllDocumentsMenuItem_Click(object sender, EventArgs e)
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
            _toolStripStatusLabel.Text = "Saving all items...";
            Log.Write(LogLevel.Trace, "Saving all items...");

            var fileNamePrefix = Path.GetTempPath() + "DataCommander.SaveAll." + '[' + DateTime.Now.ToString("yyyyMMddHHmmss.fff") + ']';
            var index = 1;
            foreach (var mdiChild in MdiChildren)
            {
                var queryForm = mdiChild as QueryForm;
                if (queryForm != null)
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

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e) => SaveAll();

        private ToolStripTextBox _activeMdiChildToolStripTextBox;
        public ToolStripTextBox ActiveMdiChildToolStripTextBox => _activeMdiChildToolStripTextBox;

        private void checkForToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/csbernath/DataCommander/releases";
            Process.Start(url);
        }

        private void managedMemoryToolStripStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip(components);

                var menuItem = new ToolStripMenuItem("Collect garbage");
                menuItem.Click += CollectGarbage_Click;
                menu.Items.Add(menuItem);

                var bounds = _managedMemoryToolStripStatusLabel.Bounds;
                var location = e.Location;
                menu.Show(_statusBar, bounds.X + location.X, bounds.Y + location.Y);
            }
        }

        private void CollectGarbage_Click(object sender, EventArgs e)
        {
            GC.Collect();

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(GarbageMonitor.Default.State);
            sb.AppendLine();
            sb.Append(ThreadMonitor.ToStringTableString());
            sb.AppendLine();
            sb.Append(AppDomainMonitor.GetCurrentDomainState());
            Log.Trace(sb.ToString());

            ThreadMonitor.Join(0);
        }
    }
}