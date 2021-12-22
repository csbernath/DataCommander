using System.Threading;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodInvocation
{
    private static int _idSequence;

    public MethodInvocation(
        MethodInvocation parent,
        int methodId,
        long beginTime)
    {
        Parent = parent;
        Id = Interlocked.Increment(ref _idSequence);
        MethodId = methodId;
        BeginTime = beginTime;
    }

    public MethodInvocation Parent { get; }

    public int Id { get; }

    public int MethodId { get; }

    public long BeginTime { get; }

    public long EndTime { get; set; }
}