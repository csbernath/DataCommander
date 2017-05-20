namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    internal sealed class MethodCollection : IEnumerable<MethodBase>
    {
        private int _idSequence;
        private readonly ConcurrentDictionary<MethodBase, int> _methods = new ConcurrentDictionary<MethodBase, int>();

        public bool TryGetValue(MethodBase method, out int methodId)
        {
            return this._methods.TryGetValue(method, out methodId);
        }

        public int Add(MethodBase method)
        {
            var id = Interlocked.Increment(ref this._idSequence);
            this._methods.TryAdd(method, id);
            return id;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._methods.Values.GetEnumerator();
        }

        IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
        {
            return this._methods.Keys.GetEnumerator();
        }
    }
}