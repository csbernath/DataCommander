using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Foundation.Data.MethodProfiler
{
    internal sealed class MethodCollection : IEnumerable<MethodBase>
    {
        private int _idSequence;
        private readonly ConcurrentDictionary<MethodBase, int> _methods = new();

        public bool TryGetValue(MethodBase method, out int methodId)
        {
            return _methods.TryGetValue(method, out methodId);
        }

        public int Add(MethodBase method)
        {
            var id = Interlocked.Increment(ref _idSequence);
            _methods.TryAdd(method, id);
            return id;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _methods.Values.GetEnumerator();
        }

        IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
        {
            return _methods.Keys.GetEnumerator();
        }
    }
}