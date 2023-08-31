using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;
using Foundation.Core;

namespace DataCommander.Application;

public sealed partial class CancelableOperationForm : Form
{
    private readonly Control _owner;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private System.Threading.Timer? _elapsedTimeTimer;
    private long _startTimestamp;
    private bool _operationCanceled;

    public CancelableOperationForm(
        Control owner,
        CancellationTokenSource cancellationTokenSource,
        string formText,
        string textBoxText,
        ColorTheme? colorTheme)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner;
        _cancellationTokenSource = cancellationTokenSource;
        InitializeComponent();
        colorTheme?.Apply(this);
        Text = formText;
        textBox.AppendText(textBoxText);
        ActiveControl = cancelButton;
    }

    public void Start(Task task, TimeSpan showDialogDelay)
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        var cancellationToken = _cancellationTokenSource.Token;
        task.ContinueWith(_ =>
        {
            if (IsHandleCreated)
                Invoke(Close);
        }, cancellationToken);
        task.Start();
        var completed = task.Wait(showDialogDelay);
        if (!completed)
        {
            _elapsedTimeTimer = new System.Threading.Timer(ElapsedTimeTimerCallback, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            ShowDialog(_owner);
        }
    }

    public long StartTimestamp => _startTimestamp;
    public bool OperationCanceled => _operationCanceled;

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        _elapsedTimeTimer.Dispose();
    }

    private void ElapsedTimeTimerCallback(object? state)
    {
        if (IsHandleCreated)
        {
            var elapsed = Stopwatch.GetTimestamp() - _startTimestamp;
            var text = StopwatchTimeSpan.ToString(elapsed, 0);
            Invoke(() => { elapsedTimeTextBox.Text = text; });
        }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        cancelButton.Enabled = false;
        _operationCanceled = true;
        textBox.AppendText("\r\nCanceling operation...");
        _cancellationTokenSource.Cancel();
    }
}