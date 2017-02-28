namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    internal sealed class MethodCollection : IEnumerable<MethodBase>
    {
        private int idSequence;
        private readonly ConcurrentDictionary<MethodBase, int> methods = new ConcurrentDictionary<MethodBase, int>();

        public bool TryGetValue(MethodBase method, out int methodId)
        {
            return this.methods.TryGetValue(method, out methodId);
        }

        public int Add(MethodBase method)
        {
            var id = Interlocked.Increment(ref this.idSequence);
            this.methods.TryAdd(method, id);
            return id;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.methods.Values.GetEnumerator();
        }

        IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
        {
            return this.methods.Keys.GetEnumerator();
        }
    }
}