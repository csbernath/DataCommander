namespace DataCommander.Providers
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Windows.Forms;
    using Timer = System.Windows.Forms.Timer;

    internal sealed class OpenConnectionForm : Form
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private Button btnCancel;
        private TextBox tbTimer;
        private TextBox textBox;
        private Timer timer;
        private IContainer components;
        private readonly ConnectionProperties connectionProperties;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly EventWaitHandle handleCreatedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Task task;
        private long duration;

        public long Duration => this.duration;

        private void EndConnectionOpenInvoke(Exception exception)
        {
            DialogResult dialogResult;

            if (exception == null)
            {
                dialogResult = DialogResult.OK;
            }
            else
            {
                log.Write(LogLevel.Error, exception.ToString());
                IProvider provider = this.connectionProperties.Provider;
                string message;

                if (provider != null)
                {
                    try
                    {
                        message = provider.GetExceptionMessage(exception);
                    }
                    catch
                    {
                        message = exception.ToString();
                    }
                }
                else
                {
                    message = exception.ToString();
                }

                MessageBox.Show(
                    this,
                    message,
                    "Failed to open connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                dialogResult = DialogResult.Cancel;
            }

            this.DialogResult = dialogResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        private void EndConnectionOpen(Exception exception)
        {
            this.timer.Enabled = false;
            this.handleCreatedEvent.WaitOne();
            this.Invoke(() => this.EndConnectionOpenInvoke(exception));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionProperties"></param>
        public OpenConnectionForm(ConnectionProperties connectionProperties)
        {
            this.InitializeComponent();
            this.HandleCreated += this.OpenConnectionForm_HandleCreated;

            this.stopwatch.Start();
            this.timer.Enabled = true;

            var dbConnectionStringBuilder = new DbConnectionStringBuilder();
            dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
            object dataSourceObject;
            bool contains = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out dataSourceObject);
            object userId;
            dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.UserId, out userId);
            string dataSource = (string)dataSourceObject;
            this.textBox.Text =
                $"Connection name: {connectionProperties.ConnectionName}\r\nProvider name: {connectionProperties.ProviderName}\r\nData Source: {dataSource}\r\nUserId: {userId}";
            this.connectionProperties = connectionProperties;
            this.Cursor = Cursors.AppStarting;

            if (this.connectionProperties.Provider == null)
            {
                this.connectionProperties.Provider = ProviderFactory.CreateProvider(this.connectionProperties.ProviderName);
            }

            var connection = this.connectionProperties.Provider.CreateConnection(connectionProperties.ConnectionString);
            this.cancellationTokenSource = new CancellationTokenSource();

            var stopwatch = Stopwatch.StartNew();

            this.task = Task.Factory.StartNew(() =>
            {
                var task = connection.OpenAsync(this.cancellationTokenSource.Token);
                task.ContinueWith(t =>
                {
                    this.duration = stopwatch.ElapsedTicks;

                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        connectionProperties.Connection = connection;
                        this.EndConnectionOpen(task.Exception);
                    }
                });
            });
        }

        private void OpenConnectionForm_HandleCreated(object sender, EventArgs e)
        {
            this.handleCreatedEvent.Set();
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbTimer = new System.Windows.Forms.TextBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(296, 81);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbTimer
            // 
            this.tbTimer.BackColor = System.Drawing.SystemColors.Control;
            this.tbTimer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbTimer.Location = new System.Drawing.Point(8, 86);
            this.tbTimer.Name = "tbTimer";
            this.tbTimer.Size = new System.Drawing.Size(64, 14);
            this.tbTimer.TabIndex = 1;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(8, 6);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(647, 75);
            this.textBox.TabIndex = 2;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // OpenConnectionForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(667, 106);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.tbTimer);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "OpenConnectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Opening connection...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cancellationTokenSource.Cancel();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            long ticks = this.stopwatch.ElapsedTicks;
            this.tbTimer.Text = StopwatchTimeSpan.ToString(ticks, 0);
        }
    }
}