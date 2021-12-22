using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;
#if FOUNDATION_3_5
#else
#endif

/// <summary>
/// 
/// </summary>
public sealed class CreateTaskResponse
{
    /// <summary>
    /// 
    /// </summary>
    public Task Task;

    /// <summary>
    /// 
    /// </summary>
    public TaskInfo TaskInfo;
}