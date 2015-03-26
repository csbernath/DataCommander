namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    internal sealed class MethodCollection : IEnumerable<MethodBase>
    {
        private int idSequence;
        private Dictionary<MethodBase, int> methods = new Dictionary<MethodBase, int>();

        public bool TryGetValue(MethodBase method, out int methodId)
        {
            return this.methods.TryGetValue(method, out methodId);
        }

        public int Add(MethodBase method)
        {
            int id;

            lock (this.methods)
            {
                id = Interlocked.Increment(ref this.idSequence);
                this.methods.Add(method, id);
            }

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