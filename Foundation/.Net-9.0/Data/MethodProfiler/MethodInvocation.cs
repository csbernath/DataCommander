using System.Threading;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodInvocation(
    MethodInvocation? parent,
    int methodId,
    long beginTime)
{
    private static int _idSequence;

    public MethodInvocation? Parent { get; } = parent;

    public int Id { get; } = Interlocked.Increment(ref _idSequence);

    public int MethodId { get; } = methodId;

    public long BeginTime { get; } = beginTime;

    public long EndTime { get; set; }
}