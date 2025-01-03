using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public sealed class CreateTaskResponse
{
    public Task Task;
    public TaskInfo TaskInfo;
}