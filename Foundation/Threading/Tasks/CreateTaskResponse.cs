using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public sealed class CreateTaskResponse(Task task, TaskInfo taskInfo)
{
    public readonly Task Task = task;
    public readonly TaskInfo TaskInfo = taskInfo;
}