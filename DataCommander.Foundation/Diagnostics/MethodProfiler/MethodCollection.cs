namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    internal sealed class MethodCollection : IEnumerable<MethodBase>
    {
        private Int32 idSequence;
        private Dictionary<MethodBase, Int32> methods = new Dictionary<MethodBase, Int32>();

        public Boolean TryGetValue(MethodBase method, out Int32 methodId)
        {
            return this.methods.TryGetValue(method, out methodId);
        }

        public Int32 Add(MethodBase method)
        {
            Int32 id;

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