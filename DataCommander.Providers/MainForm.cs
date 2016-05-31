namespace DataCommander
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Threading;
    using DataCommander.Foundation.Windows.Forms;
    using DataCommander.Providers;

    /// <summary>
    /// Summary description for MainForm.
    /// </summary>
    public class MainForm : Form
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly StringCollection recentFileList = new StringCollection();

        private MenuStrip mainMenu;
        private ToolStripMenuItem menuItem1;
        private ToolStripMenuItem mnuConnect;
        private ImageList imageList;
        private StatusStrip statusBar;
        private ToolStrip toolStrip;
        private ToolStripMenuItem mnuExit;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuAbout;
        private ToolStripMenuItem mnuOpen;
        private ToolStripButton btnConnect;
        private ToolStripMenuItem mnuWindow;
        private ToolStripMenuItem mnuRecentFileList;
        private ToolStripMenuItem mnuFont;
        private ToolStripButton openButton;
        private ToolStripButton saveButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton helpButton;
        private ToolStripMenuItem NewToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripPanel toolStripPanel;
        private ToolStripMenuItem closeAllDocumentsMenuItem;
        private IContainer components;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripMenuItem saveAllToolStripMenuItem;
        private ToolStripMenuItem recentConnectionsToolStripMenuItem;
        private ToolStripMenuItem checkForToolStripMenuItem;
        private ToolStripStatusLabel managedMemoryToolStripStatusLabel;
        private ToolStrip queryFormToolStrip;
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            this.InitializeComponent();

            this.helpButton.Click += this.helpButton_Click;
            this.mnuAbout.Click += this.mnuAbout_Click;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.LoadLayout();

            DateTime start = Process.GetCurrentProcess().StartTime;
            DateTime end = DateTime.Now;
            TimeSpan elapsed = end - start;

            string message = $"Current user: {WindowsIdentity.GetCurrent().Name}. Application loaded in {new StopwatchTimeSpan(elapsed).ToString(3)} seconds.";
            this.toolStripStatusLabel.Text = message;
            log.Trace(message);

            this.UpdateTotalMemory();

            this.timer = new System.Windows.Forms.Timer(this.components)
            {
                Interval = 10000, // 10 seconds
            };
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        public void UpdateTotalMemory()
        {
            long totalMemory = GC.GetTotalMemory(false);
            double totalMemoryMB = (double)totalMemory/1024.0/1024.0;
            string text = $"{Math.Round(totalMemoryMB, 0)} MB";

            this.managedMemoryToolStripStatusLabel.Text = text;

            this.managedMemoryToolStripStatusLabel.ForeColor = totalMemoryMB <= 256
                ? SystemColors.ControlText
                : Color.Red;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.UpdateTotalMemory();
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
                    this.components.Dispose();
                }
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
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.NewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.recentConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecentFileList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFont = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllDocumentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnConnect = new System.Windows.Forms.ToolStripButton();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ActiveMdiChildToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.managedMemoryToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.mainMenu.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.toolStripPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem1,
            this.mnuFont,
            this.mnuWindow,
            this.mnuHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.MdiWindowListItem = this.mnuWindow;
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(792, 24);
            this.mainMenu.TabIndex = 1;
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripMenuItem,
            this.mnuConnect,
            this.mnuOpen,
            this.recentConnectionsToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.mnuRecentFileList,
            this.mnuExit});
            this.menuItem1.MergeIndex = 1;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(79, 20);
            this.menuItem1.Text = "&Database";
            // 
            // NewToolStripMenuItem
            // 
            this.NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            this.NewToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.NewToolStripMenuItem.Text = "&Create";
            this.NewToolStripMenuItem.Click += new System.EventHandler(this.CreateMenuItem_Click);
            // 
            // mnuConnect
            // 
            this.mnuConnect.Image = ((System.Drawing.Image)(resources.GetObject("mnuConnect.Image")));
            this.mnuConnect.MergeIndex = 0;
            this.mnuConnect.Name = "mnuConnect";
            this.mnuConnect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuConnect.Size = new System.Drawing.Size(187, 22);
            this.mnuConnect.Text = "&Connect";
            this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // mnuOpen
            // 
            this.mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuOpen.Image")));
            this.mnuOpen.MergeIndex = 1;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(187, 22);
            this.mnuOpen.Text = "&Open";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // recentConnectionsToolStripMenuItem
            // 
            this.recentConnectionsToolStripMenuItem.Name = "recentConnectionsToolStripMenuItem";
            this.recentConnectionsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.recentConnectionsToolStripMenuItem.Text = "Recent connections";
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveAllToolStripMenuItem.Text = "Save All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // mnuRecentFileList
            // 
            this.mnuRecentFileList.MergeIndex = 2;
            this.mnuRecentFileList.Name = "mnuRecentFileList";
            this.mnuRecentFileList.Size = new System.Drawing.Size(187, 22);
            this.mnuRecentFileList.Text = "Recent &File List";
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuExit.Size = new System.Drawing.Size(187, 22);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuFont
            // 
            this.mnuFont.MergeIndex = 2;
            this.mnuFont.Name = "mnuFont";
            this.mnuFont.Size = new System.Drawing.Size(43, 20);
            this.mnuFont.Text = "Font";
            this.mnuFont.Click += new System.EventHandler(this.mnuFont_Click);
            // 
            // mnuWindow
            // 
            this.mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeAllDocumentsMenuItem});
            this.mnuWindow.MergeIndex = 6;
            this.mnuWindow.Name = "mnuWindow";
            this.mnuWindow.Size = new System.Drawing.Size(63, 20);
            this.mnuWindow.Text = "&Window";
            // 
            // closeAllDocumentsMenuItem
            // 
            this.closeAllDocumentsMenuItem.Name = "closeAllDocumentsMenuItem";
            this.closeAllDocumentsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeAllDocumentsMenuItem.Text = "Close All Documents";
            this.closeAllDocumentsMenuItem.Click += new System.EventHandler(this.closeAllDocumentsMenuItem_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.checkForToolStripMenuItem,
            this.mnuAbout});
            this.mnuHelp.MergeIndex = 7;
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.contentsToolStripMenuItem.Text = "Contents";
            this.contentsToolStripMenuItem.Click += new System.EventHandler(this.contentsToolStripMenuItem_Click);
            // 
            // checkForToolStripMenuItem
            // 
            this.checkForToolStripMenuItem.Name = "checkForToolStripMenuItem";
            this.checkForToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.checkForToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.checkForToolStripMenuItem.Text = "Check for updates ";
            this.checkForToolStripMenuItem.Click += new System.EventHandler(this.checkForToolStripMenuItem_Click);
            // 
            // mnuAbout
            // 
            this.mnuAbout.MergeIndex = 0;
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(198, 22);
            this.mnuAbout.Text = "About...";
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.ImageList = this.imageList;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConnect,
            this.openButton,
            this.saveButton,
            this.toolStripSeparator1,
            this.helpButton,
            this.toolStripSeparator2,
            this.ActiveMdiChildToolStripTextBox});
            this.toolStrip.Location = new System.Drawing.Point(3, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(518, 25);
            this.toolStrip.TabIndex = 2;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            // 
            // btnConnect
            // 
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(23, 22);
            this.btnConnect.ToolTipText = "Connect to database";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(23, 22);
            this.openButton.Text = "toolStripButton1";
            this.openButton.ToolTipText = "Open database";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Enabled = false;
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(23, 22);
            this.saveButton.ToolTipText = "Save Query";
            this.saveButton.Click += new EventHandler(this.saveButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // helpButton
            // 
            this.helpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpButton.Image = ((System.Drawing.Image)(resources.GetObject("helpButton.Image")));
            this.helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(23, 22);
            this.helpButton.Text = "Help";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // activeMdiChildToolStripTextBox
            // 
            this.ActiveMdiChildToolStripTextBox.Name = "ActiveMdiChildToolStripTextBox";
            this.ActiveMdiChildToolStripTextBox.ReadOnly = true;
            this.ActiveMdiChildToolStripTextBox.Size = new System.Drawing.Size(400, 25);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.managedMemoryToolStripStatusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 531);
            this.statusBar.Name = "statusBar";
            this.statusBar.ShowItemToolTips = true;
            this.statusBar.Size = new System.Drawing.Size(792, 22);
            this.statusBar.TabIndex = 3;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(677, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // managedMemoryToolStripStatusLabel
            // 
            this.managedMemoryToolStripStatusLabel.AutoSize = false;
            this.managedMemoryToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.managedMemoryToolStripStatusLabel.Name = "managedMemoryToolStripStatusLabel";
            this.managedMemoryToolStripStatusLabel.Size = new System.Drawing.Size(100, 17);
            this.managedMemoryToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.managedMemoryToolStripStatusLabel.ToolTipText = "Managed memory";
            this.managedMemoryToolStripStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.managedMemoryToolStripStatusLabel_MouseUp);
            // 
            // toolStripPanel
            // 
            this.toolStripPanel.Controls.Add(this.mainMenu);
            this.toolStripPanel.Controls.Add(this.toolStrip);
            this.toolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.toolStripPanel.Name = "toolStripPanel";
            this.toolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolStripPanel.Size = new System.Drawing.Size(792, 49);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(792, 553);
            this.Controls.Add(this.toolStripPanel);
            this.Controls.Add(this.statusBar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Data Commander";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.toolStripPanel.ResumeLayout(false);
            this.toolStripPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void Connect()
        {
            ConnectionForm connectionForm = new ConnectionForm(this.statusBar);

            if (connectionForm.ShowDialog() == DialogResult.OK)
            {
                var connectionProperties = connectionForm.ConnectionProperties;

                var queryForm = new QueryForm(this, this.MdiChildren.Length, connectionProperties.Provider, connectionProperties.ConnectionString,
                    connectionProperties.Connection, this.statusBar);

                queryForm.MdiParent = this;
                if (this.SelectedFont != null)
                {
                    queryForm.Font = this.SelectedFont;
                }
                queryForm.FormClosing += this.queryForm_FormClosing;

                switch (this.WindowState)
                {
                    case FormWindowState.Normal:
                        int width = Math.Max(this.ClientSize.Width + 70, 100);
                        int height = Math.Max(this.ClientSize.Height - 120, 50);
                        queryForm.ClientSize = new Size(width, height);
                        break;

                    case FormWindowState.Maximized:
                        //queryForm.WindowState = FormWindowState.Maximized;
                        break;

                    default:
                        break;
                }

                string message = $@"Connection opened in {StopwatchTimeSpan.ToString(connectionForm.Duration, 3)} seconds.
ServerVersion: {connectionProperties.Connection.ServerVersion}";

                var infoMessage = new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message);
                queryForm.AddInfoMessage(infoMessage);

                queryForm.Show();

                if (this.WindowState == FormWindowState.Maximized)
                {
                    queryForm.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void queryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel)
            {
                var form = (Form)sender;
                // form.MdiParent = null;

                if (this.queryFormToolStrip != null)
                {
                    this.toolStripPanel.Controls.Remove(this.queryFormToolStrip);
                    this.queryFormToolStrip = null;
                }
            }
        }

        private void mnuConnect_Click(object sender, EventArgs e)
        {
            this.Connect();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();

            //MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveLayout()
        {
            ApplicationData applicationData = DataCommanderApplication.Instance.ApplicationData;
            FormPosition.Save(this, applicationData);
            ConfigurationNode folder = applicationData.CurrentType;
            string[] array = new string[this.recentFileList.Count];
            this.recentFileList.CopyTo(array, 0);
            folder.Attributes.SetAttributeValue("RecentFileList", array);
        }

        private void mnuRecentFile_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            int index = this.mnuRecentFileList.DropDownItems.IndexOf(menuItem);
            int count = this.recentFileList.Count;
            string path = this.recentFileList[count - index - 1];
            this.LoadFiles(new string[] {path});
        }

        private void CreateRecentFileListMenu()
        {
            ToolStripItemCollection menuItems = this.mnuRecentFileList.DropDownItems;
            menuItems.Clear();

            int count = this.recentFileList.Count;

            for (int i = 0; i < count; i++)
            {
                string path = this.recentFileList[count - i - 1];
                string text = $"{i + 1} {path}";
                var menuItem = new ToolStripMenuItem(text, null, this.mnuRecentFile_Click);
                menuItems.Add(menuItem);
            }
        }

        private void LoadLayout()
        {
            ApplicationData applicationData = DataCommanderApplication.Instance.ApplicationData;
            FormPosition.Load(applicationData, this);
            ConfigurationNode folder = applicationData.CurrentType;
            string[] array;
            bool contains = folder.Attributes.TryGetAttributeValue("RecentFileList", out array);

            if (contains && array != null)
            {
                int i;

                for (i = 0; i < array.Length; i++)
                {
                    this.recentFileList.Add(array[i]);
                }
            }

            string base64;
            contains = folder.Attributes.TryGetAttributeValue("Font", out base64);

            if (contains)
            {
                this.SelectedFont = DeserializeFont(base64);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SaveLayout();
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
                string currentDirectory = Environment.CurrentDirectory;

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (Environment.CurrentDirectory != currentDirectory)
                    {
                        Environment.CurrentDirectory = currentDirectory;
                    }

                    string fileName = fileDialog.FileName;
                    string extension = Path.GetExtension(fileName).ToLower();
                    string connectionString = null;
                    IProvider provider = null;

                    switch (fileDialog.FilterIndex)
                    {
                        case 1:
                            this.LoadFiles(fileDialog.FileNames);
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
                            {
                                if (Environment.Is64BitProcess)
                                {
                                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 8.0";
                                }
                                else
                                {
                                    connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Extended Properties=Excel 8.0";
                                }
                            }
                            else
                            {
                                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=Excel 12.0";
                            }

                            provider = ProviderFactory.CreateProvider(ProviderName.OleDb);
                            break;

                        case 5:
                            connectionString = $"Data Source={fileName}";
                            provider = ProviderFactory.CreateProvider("Msi");
                            break;

                        case 6:
                            connectionString = $"Data Source={fileName}";
                            provider = ProviderFactory.CreateProvider(ProviderName.SQLite);
                            break;

                        case 7:
                            connectionString = $"Data Source={fileName}";
                            provider = ProviderFactory.CreateProvider("SqlServerCe");
                            break;

                        case 8:
                            connectionString = $"Data Source={fileName}";
                            provider = ProviderFactory.CreateProvider(ProviderName.SqlServerCe40);
                            break;

                        default:
                            throw new NotSupportedException();
                    }

                    if (provider != null)
                    {
                        ConnectionBase connection = provider.CreateConnection(connectionString);
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
                        connectionProperties.Save(subNode);

                        var queryForm = new QueryForm(
                            this,
                            this.MdiChildren.Length,
                            provider,
                            connectionString,
                            connection, this.statusBar);

                        queryForm.MdiParent = this;
                        queryForm.Font = this.SelectedFont;
                        queryForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(LogLevel.Error, ex.ToLogString());
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        public void LoadFiles(string[] fileNames)
        {
            int i = fileNames.Length - 1;
            string path = fileNames[i];
            QueryForm queryForm = (QueryForm) this.ActiveMdiChild;
            queryForm.LoadFile(path);

            int index = this.recentFileList.IndexOf(path);

            if (index >= 0)
            {
                this.recentFileList.RemoveAt(index);
            }

            this.recentFileList.Add(path);
            this.CreateRecentFileListMenu();
        }

        public StatusStrip StatusBar => this.statusBar;

        private static string Serialize(Font font)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, font);
            byte[] bytes = memoryStream.ToArray();
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static Font DeserializeFont(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var memoryStream = new MemoryStream(bytes);
            var binaryFormatter = new BinaryFormatter();
            object obj = binaryFormatter.Deserialize(memoryStream);
            var font = (Font)obj;
            return font;
        }

        private void mnuFont_Click(object sender, EventArgs e)
        {
            var fontDialog = new FontDialog();
            fontDialog.Font = this.SelectedFont;
            var dialogResult = fontDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                this.SelectedFont = fontDialog.Font;
                ApplicationData applicationData = DataCommanderApplication.Instance.ApplicationData;
                ConfigurationNode propertyFolder = applicationData.CurrentType;
                propertyFolder.Attributes.SetAttributeValue("Font", Serialize(this.SelectedFont));
            }
        }

        public Font SelectedFont { get; private set; }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Connect();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var queryForm = (QueryForm)this.ActiveMdiChild;

            if (queryForm != null)
            {
                queryForm.Save();
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            this.ShowContents();
        }

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
                        providerName = ProviderName.SQLite;
                        connectionString = sb.ConnectionString;
                        break;

                    default:
                        throw new Exception();
                }

                var provider = ProviderFactory.CreateProvider(providerName);
                Contract.Assert(provider != null);
                var connection = provider.CreateConnection(connectionString);
                await connection.OpenAsync(CancellationToken.None);

                var queryForm = new QueryForm(this, this.MdiChildren.Length, provider, connectionString, connection, this.statusBar);
                queryForm.MdiParent = this;
                queryForm.Font = this.SelectedFont;
                queryForm.Show();
            }
        }

        private void ShowContents()
        {
            const string url = "https://github.com/csbernath/DataCommander/blob/master/README.md";
            Process.Start(url);
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowContents();
        }

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);

            this.ActiveMdiChildToolStripTextBox.Text = this.ActiveMdiChild != null ? this.ActiveMdiChild.Text : null;
            this.saveButton.Enabled = this.ActiveMdiChild != null;

            if (this.ActiveMdiChild != null)
            {
                this.toolStripPanel.ResumeLayout(false);
                if (this.queryFormToolStrip != null)
                {
                    this.toolStripPanel.Controls.Remove(this.queryFormToolStrip);
                }

                var queryForm = (QueryForm)this.ActiveMdiChild;
                var queryFormToolStrip = queryForm.ToolStrip;
                if (queryFormToolStrip != null)
                {
                    queryFormToolStrip.Visible = true;
                    var location = new Point(this.toolStrip.Right, this.toolStrip.Top);
                    this.toolStripPanel.Join(queryFormToolStrip, location);
                    this.toolStripPanel.PerformLayout();

                    this.queryFormToolStrip = queryFormToolStrip;
                }
                this.CreateRecentFileListMenu();
            }
            else
            {
                this.mnuRecentFileList.DropDownItems.Clear();
            }
        }

        private void closeAllDocumentsMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                var mdiChildren = this.MdiChildren;
                int length = mdiChildren.Length;
                if (length == 0)
                {
                    break;
                }
                var mdiChild = mdiChildren[length - 1];
                mdiChild.Close();

                if (this.MdiChildren.Length == length)
                {
                    break;
                }
            }
        }

        internal void SaveAll()
        {
            this.Cursor = Cursors.WaitCursor;
            this.toolStripStatusLabel.Text = "Saving all items...";
            log.Write(LogLevel.Trace, "Saving all items...");

            string fileNamePrefix = Path.GetTempPath() + "DataCommander.SaveAll." + '[' + DateTime.Now.ToString("yyyyMMddHHmmss.fff") + ']';
            int index = 1;
            foreach (Form mdiChild in this.MdiChildren)
            {
                var queryForm = mdiChild as QueryForm;
                if (queryForm != null)
                {
                    string text = queryForm.QueryTextBox.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        string fileName = fileNamePrefix + '[' + index.ToString().PadLeft(3, '0') + "].sql";
                        text = text.Replace("\n", "\r\n");
                        File.WriteAllText(fileName, text, Encoding.UTF8);
                        index++;
                    }
                }
            }
            this.toolStripStatusLabel.Text = $"All items saved to {fileNamePrefix}.";
            this.Cursor = Cursors.Default;
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveAll();
        }

        public ToolStripTextBox ActiveMdiChildToolStripTextBox { get; private set; }

        private void checkForToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/csbernath/DataCommander/releases";
            Process.Start(url);
        }

        private void managedMemoryToolStripStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip(this.components);

                var menuItem = new ToolStripMenuItem("Collect garbage");
                menuItem.Click += CollectGarbage_Click;
                menu.Items.Add(menuItem);

                var bounds = this.managedMemoryToolStripStatusLabel.Bounds;
                var location = e.Location;
                menu.Show(this.statusBar, bounds.X + location.X, bounds.Y + location.Y);
            }
        }

        private void CollectGarbage_Click(object sender, EventArgs e)
        {
            GC.Collect();

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(GarbageMonitor.State);
            sb.AppendLine();
            sb.Append(ThreadMonitor.ToStringTableString());
            sb.AppendLine();
            sb.Append(AppDomainMonitor.CurrentDomainState);
            log.Trace(sb.ToString());

            ThreadMonitor.Join(0);
        }
    }
}