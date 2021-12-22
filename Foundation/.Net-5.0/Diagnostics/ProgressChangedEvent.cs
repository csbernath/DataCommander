
namespace Foundation.Diagnostics;

public class ProgressChangedEvent
{
    public readonly int TaskCount;
    public readonly int Percent;

    public ProgressChangedEvent(int taskCount, int percent)
    {
        TaskCount = taskCount;
        Percent = percent;
    }
}