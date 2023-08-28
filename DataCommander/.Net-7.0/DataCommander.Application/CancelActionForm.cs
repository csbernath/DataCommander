using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;
using Foundation.Core;

namespace DataCommander.Application;

public partial class CancelActionForm : Form
{
    private readonly Control _owner;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly TimeSpan _dueTime;
    private System.Threading.Timer? _elapsedTimeTimer;
    private long _startTimestamp;

    public CancelActionForm(Control owner, CancellationTokenSource cancellationTokenSource, TimeSpan dueTime, string text, ColorTheme? colorTheme)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner;
        _dueTime = dueTime;
        _cancellationTokenSource = cancellationTokenSource;
        _elapsedTimeTimer = new System.Threading.Timer(ElapsedTimeTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        InitializeComponent();
        colorTheme?.Apply(this);
        textBox.AppendText(text);
    }

    public void BeforeExecuteAction()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        var cancellationToken = _cancellationTokenSource.Token;
        Task.Delay(TimeSpan.FromSeconds(2), cancellationToken)
            .ContinueWith(_ =>
            {
                var period = TimeSpan.FromSeconds(1);
                if (_elapsedTimeTimer != null && !cancellationToken.IsCancellationRequested)
                {
                    _elapsedTimeTimer.Change(_dueTime, period);
                    _owner.Invoke(() => ShowDialog(_owner));
                }
            }, cancellationToken);
    }

    public void AfterExecuteAction()
    {
        if (_elapsedTimeTimer != null)
        {
            _elapsedTimeTimer.Dispose();
            _elapsedTimeTimer = null;
        }

        if (IsHandleCreated)
            Invoke(Close);
    }

    private void ElapsedTimeTimerCallback(object? state)
    {
        if (IsHandleCreated)
        {
            var elapsed = Stopwatch.GetTimestamp() - _startTimestamp;
            Invoke(() => { elapsedTimeTextBox.Text = StopwatchTimeSpan.ToString(elapsed, 0); });
        }
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        cancelButton.Enabled = false;
        textBox.AppendText("\r\nCanceling action...");
        _cancellationTokenSource.Cancel();
    }
}