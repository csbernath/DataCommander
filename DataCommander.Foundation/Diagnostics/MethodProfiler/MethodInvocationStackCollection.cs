namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal sealed class MethodInvocationStackCollection
    {
        private readonly ConcurrentDictionary<int, Stack<MethodInvocation>> stacks = new ConcurrentDictionary<int, Stack<MethodInvocation>>();

        public void Push(int threadId, int methodId, long beginTime)
        {
            var stack = this.stacks.GetOrAdd(threadId, key => new Stack<MethodInvocation>());
            var parent = stack.Count > 0 ? stack.Peek() : null;
            var item = new MethodInvocation(parent, methodId, beginTime);
            stack.Push(item);
        }

        public MethodInvocation Pop(int threadId)
        {
            var stack = this.stacks[threadId];
            return stack.Pop();
        }
    }
}