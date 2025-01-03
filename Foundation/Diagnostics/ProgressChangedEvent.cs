
namespace Foundation.Diagnostics;

public class ProgressChangedEvent(int taskCount, int percent)
{
    public readonly int TaskCount = taskCount;
    public readonly int Percent = percent;
}