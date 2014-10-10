namespace DataCommander.Foundation.Threading.Tasks
{
    using System;

    internal sealed class MonitoredTaskActionState : MonitoredTaskState
    {
        public Action<object> Action = null;
    }
}