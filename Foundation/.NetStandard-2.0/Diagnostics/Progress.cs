using System;
using Foundation.Assertions;

namespace Foundation.Diagnostics
{
    public class Progress
    {
        private readonly int _startPercent;
        private readonly int _endPercent;
        private readonly int _taskCount;
        private readonly Action<ProgressChangedEvent> _handleEvent;
        private int _currentTaskCount;
        private int _currentPercent;

        public Progress(int startPercent, int endPercent, int taskCount, Action<ProgressChangedEvent> handleEvent)
        {
            _startPercent = startPercent;
            _endPercent = endPercent;
            _taskCount = taskCount;
            _handleEvent = handleEvent;

            _currentTaskCount = 0;
            _currentPercent = _startPercent;
        }

        public void Add(int value)
        {
            Assert.IsTrue(value >= 0);
            var newTaskCount = _currentTaskCount + value;
            Assert.IsTrue(newTaskCount <= _taskCount);

            var newRatio = (double) newTaskCount / _taskCount;
            var newPercentDouble = _startPercent + newRatio * (_endPercent - _startPercent);
            var newPercent = (int) newPercentDouble;
            var changed = _currentPercent < newPercent;
            _currentPercent = newPercent;
            _currentTaskCount = newTaskCount;
            if (changed)
                _handleEvent(new ProgressChangedEvent(newTaskCount, newPercent));
        }
    }
}