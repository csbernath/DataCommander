namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;

    internal sealed class MethodInvocationStackCollection
    {
        public void Push(int threadId, int methodId, long beginTime)
        {
            Stack<MethodInvocation> stack;

            lock (this.stacks)
            {
                bool contains = this.stacks.TryGetValue(threadId, out stack);

                if (!contains)
                {
                    stack = new Stack<MethodInvocation>();
                    this.stacks.Add(threadId, stack);
                }
            }

            MethodInvocation parent;

            if (stack.Count > 0)
            {
                parent = stack.Peek();
            }
            else
            {
                parent = null;
            }

            MethodInvocation item = new MethodInvocation(parent, methodId, beginTime);
            stack.Push(item);
        }

        public MethodInvocation Pop(int threadId)
        {
            Stack<MethodInvocation> stack = this.stacks[threadId];
            return stack.Pop();
        }

        private Dictionary<int, Stack<MethodInvocation>> stacks = new Dictionary<int, Stack<MethodInvocation>>();
    }
}