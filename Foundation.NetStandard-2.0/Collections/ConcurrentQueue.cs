#if FOUNDATION_3_5

namespace Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentQueue<T> : IProducerConsumerCollection<T>
    {
        private readonly Queue<T> queue = new Queue<T>();

        #region IProducerConsumerCollection<T> Members

        bool IProducerConsumerCollection<T>.TryAdd( T item )
        {
            lock (this.queue)
            {
                this.queue.Enqueue( item );
            }

            return true;
        }

        bool IProducerConsumerCollection<T>.TryTake( out T item )
        {
            bool succeeded = false;
            item = default( T );

            if (this.queue.Count > 0)
            {
                lock (this.queue)
                {
                    if (this.queue.Count > 0)
                    {
                        item = this.queue.Dequeue();
                        succeeded = true;
                    }
                }
            }

            return succeeded;
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo( Array array, int index )
        {
            lock (this.queue)
            {
                ( (ICollection)this.queue.ToList() ).CopyTo( array, index );
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.queue;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.queue.GetEnumerator();
        }

        #endregion
    }
}

#endif