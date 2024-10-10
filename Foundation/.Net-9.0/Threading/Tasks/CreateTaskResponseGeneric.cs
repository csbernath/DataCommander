using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public sealed class CreateTaskResponse<TResult>(Task<TResult> task, TaskInfo taskInfo)
{
    public readonly Task<TResult> Task = task;
    public readonly TaskInfo TaskInfo = taskInfo;
}