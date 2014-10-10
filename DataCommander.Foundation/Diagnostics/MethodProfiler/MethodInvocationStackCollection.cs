namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;

    internal sealed class MethodInvocationStackCollection
    {
        public void Push(Int32 threadId, Int32 methodId, Int64 beginTime)
        {
            Stack<MethodInvocation> stack;

            lock (this.stacks)
            {
                Boolean contains = this.stacks.TryGetValue(threadId, out stack);

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

        public MethodInvocation Pop(Int32 threadId)
        {
            Stack<MethodInvocation> stack = this.stacks[threadId];
            return stack.Pop();
        }

        private Dictionary<Int32, Stack<MethodInvocation>> stacks = new Dictionary<Int32, Stack<MethodInvocation>>();
    }
}