using System;
using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Diagnostics;

public class Progress
{
    private readonly int _startPercent;
    private readonly int _endPercent;
    private readonly int _taskCount;
    private readonly long _startTimestamp;
    private readonly Action<ProgressChangedEvent> _handleEvent;
    private int _currentTaskCount;
    private int _currentPercent;

    public Progress(int startPercent, int endPercent, int taskCount, long startTimestamp, Action<ProgressChangedEvent> handleEvent)
    {
        _startPercent = startPercent;
        _endPercent = endPercent;
        _taskCount = taskCount;
        _startTimestamp = startTimestamp;
        _handleEvent = handleEvent;

        _currentTaskCount = 0;
        _currentPercent = _startPercent;
    }

    public void Add(int value, long timestamp)
    {
        Assert.IsTrue(value >= 0);
        int newTaskCount = _currentTaskCount + value;
        Assert.IsTrue(newTaskCount <= _taskCount);

        double newRatio = (double)newTaskCount / _taskCount;
        double newPercentDouble = _startPercent + newRatio * (_endPercent - _startPercent);

        long elapsedTimeAmount = timestamp - _startTimestamp;
        long estimatedTimeAmount = (long)(elapsedTimeAmount / newRatio);
        string estimatedTimeAmountString = StopwatchTimeSpan.ToString(estimatedTimeAmount, 3);

        System.Diagnostics.Debug.WriteLine(
            $"newTaskCount: {newTaskCount}, newPercentDouble: {newPercentDouble}, elapsed:  {StopwatchTimeSpan.ToString(elapsedTimeAmount, 3)} estimatedTimeAmount: {estimatedTimeAmountString}");

        int newPercent = (int)newPercentDouble;
        bool changed = _currentPercent < newPercent;
        _currentPercent = newPercent;
        _currentTaskCount = newTaskCount;
        if (changed)
            _handleEvent(new ProgressChangedEvent(newTaskCount, newPercent));
    }
}