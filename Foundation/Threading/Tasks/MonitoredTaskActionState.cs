using System;

namespace Foundation.Threading.Tasks
{
    internal sealed class MonitoredTaskActionState : MonitoredTaskState
    {
        public Action<object> Action = null;
    }
}