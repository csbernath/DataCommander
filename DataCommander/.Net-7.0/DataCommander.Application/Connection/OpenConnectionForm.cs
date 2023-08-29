using System;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Core;
using Foundation.Log;
using Timer = System.Windows.Forms.Timer;

namespace DataCommander.Application.Connection;

internal sealed class OpenConnectionForm : Form
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private Button _btnCancel;
    private TextBox _tbTimer;
    private TextBox _textBox;
    private Timer _timer;
    private IContainer components;
    private readonly ConnectionProperties _connectionProperties;
    private readonly Stopwatch _stopwatch = new();
    private readonly EventWaitHandle _handleCreatedEvent = new(false, EventResetMode.ManualReset);
    private readonly CancellationTokenSource _cancellationTokenSource;

    public OpenConnectionForm(ConnectionProperties connectionProperties, ColorTheme? colorTheme)
    {
        InitializeComponent();
        HandleCreated += OpenConnectionForm_HandleCreated;

        _stopwatch.Start();
        _timer.Enabled = true;

        colorTheme?.Apply(this);

        var dbConnectionStringBuilder = new DbConnectionStringBuilder();
        dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
        dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject);
        var containsIntegratedSecurity = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurity);
        var containsUserId = dbConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.UserId, out var userId);
        var dataSource = (string)dataSourceObject;
        var provider = ProviderFactory.GetProviders().First(p => p.Identifier == connectionProperties.ProviderIdentifier);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($@"Connection name: {connectionProperties.ConnectionName}
Provider name: {provider.Name}
{ConnectionStringKeyword.DataSource}: {dataSource}");
        
        if (containsIntegratedSecurity)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
        
        if (containsUserId) 
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {userId}");

        _textBox.Text = stringBuilder.ToString();
        _connectionProperties = connectionProperties;
        Cursor = Cursors.AppStarting;

        if (_connectionProperties.Provider == null)
            _connectionProperties.Provider = ProviderFactory.CreateProvider(_connectionProperties.ProviderIdentifier);

        var connection = _connectionProperties.Provider.CreateConnection(connectionProperties.ConnectionString);
        _cancellationTokenSource = new CancellationTokenSource();

        var stopwatch = Stopwatch.StartNew();

        Task.Factory.StartNew(() =>
        {
            var task = connection.OpenAsync(_cancellationTokenSource.Token);
            task.ContinueWith(t =>
            {
                Duration = stopwatch.ElapsedTicks;

                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    connectionProperties.Connection = connection;
                    EndConnectionOpen(task.Exception);
                }
            });
        });
    }

    public long Duration { get; private set; }

    private void EndConnectionOpenInvoke(Exception exception)
    {
        DialogResult dialogResult;

        if (exception == null)
        {
            dialogResult = DialogResult.OK;
        }
        else
        {
            Log.Write(LogLevel.Error, exception.ToString());
            var provider = _connectionProperties.Provider;
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

        DialogResult = dialogResult;
    }

    private void EndConnectionOpen(Exception exception)
    {
        _timer.Enabled = false;
        _handleCreatedEvent.WaitOne();
        Invoke(() => EndConnectionOpenInvoke(exception));
    }

    private void OpenConnectionForm_HandleCreated(object sender, EventArgs e) => _handleCreatedEvent.Set();

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            if (components != null)
                components.Dispose();

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
        this._btnCancel = new System.Windows.Forms.Button();
        this._tbTimer = new System.Windows.Forms.TextBox();
        this._textBox = new System.Windows.Forms.TextBox();
        this._timer = new System.Windows.Forms.Timer(this.components);
        this.SuspendLayout();
        // 
        // btnCancel
        // 
        this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._btnCancel.Location = new System.Drawing.Point(296, 81);
        this._btnCancel.Name = "_btnCancel";
        this._btnCancel.Size = new System.Drawing.Size(75, 23);
        this._btnCancel.TabIndex = 0;
        this._btnCancel.Text = "Cancel";
        this._btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // tbTimer
        // 
        this._tbTimer.BackColor = System.Drawing.SystemColors.Control;
        this._tbTimer.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this._tbTimer.Location = new System.Drawing.Point(8, 86);
        this._tbTimer.Name = "_tbTimer";
        this._tbTimer.Size = new System.Drawing.Size(64, 14);
        this._tbTimer.TabIndex = 1;
        // 
        // textBox
        // 
        this._textBox.Location = new System.Drawing.Point(8, 6);
        this._textBox.Multiline = true;
        this._textBox.Name = "_textBox";
        this._textBox.ReadOnly = true;
        this._textBox.Size = new System.Drawing.Size(647, 75);
        this._textBox.TabIndex = 2;
        // 
        // timer
        // 
        this._timer.Interval = 1000;
        this._timer.Tick += new System.EventHandler(this.timer_Tick);
        // 
        // OpenConnectionForm
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
        this.CancelButton = this._btnCancel;
        this.ClientSize = new System.Drawing.Size(667, 106);
        this.Controls.Add(this._textBox);
        this.Controls.Add(this._tbTimer);
        this.Controls.Add(this._btnCancel);
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
        _cancellationTokenSource.Cancel();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        var ticks = _stopwatch.ElapsedTicks;
        _tbTimer.Text = StopwatchTimeSpan.ToString(ticks, 0);
    }
}