using System.Threading.Tasks;

namespace Foundation.Threading.Tasks
{
    public sealed class CreateTaskResponse<TResult>
    {
        public Task<TResult> Task;
        public TaskInfo TaskInfo;
    }
}