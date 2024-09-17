using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodInvocationStackCollection
{
    private readonly ConcurrentDictionary<int, Stack<MethodInvocation>> _stacks = new();

    public void Push(int threadId, int methodId, long beginTime)
    {
        Stack<MethodInvocation> stack = _stacks.GetOrAdd(threadId, key => new Stack<MethodInvocation>());
        MethodInvocation parent = stack.Count > 0 ? stack.Peek() : null;
        MethodInvocation item = new MethodInvocation(parent, methodId, beginTime);
        stack.Push(item);
    }

    public MethodInvocation Pop(int threadId)
    {
        Stack<MethodInvocation> stack = _stacks[threadId];
        return stack.Pop();
    }
}