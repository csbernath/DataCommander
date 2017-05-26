using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Circular_buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CircularBuffer<T> : IList<T>
    {
        #region Private Fields

        private T[] _array;
        private int _head;
        private int _tail;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public CircularBuffer(int capacity)
        {
            this.SetCapacity(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity => this._array.Length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void AddHead(T item)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentException>(this.Count < this.Capacity);
#endif

            if (this._head == -1)
            {
                this._head = 0;
                this._tail = 0;
            }
            else
            {
                this._head = (this._head - 1) % this._array.Length;
            }

            this._array[this._head] = item;
            this.Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void AddTail(T item)
        {
#if CONTRACTS_FULL
            Contract.Assert(this.Count < this.array.Length);
#endif

            if (this._head == -1)
            {
                this._head = 0;
                this._tail = 0;
            }
            else
            {
                this._tail = (this._tail + 1) % this._array.Length;
            }

            this._array[this._tail] = item;
            this.Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void AddTail(IEnumerable<T> items)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(items != null);
#endif

            foreach (var item in items)
            {
                this.AddTail(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T PeekHead()
        {
#if CONTRACTS_FULL
            Contract.Requires<InvalidOperationException>(this.Count > 0);
#endif
            return this._array[this._head];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveHead()
        {
#if CONTRACTS_FULL
            Contract.Requires<InvalidOperationException>(this.Count > 0);
#endif

            var item = this._array[this._head];
            this._array[this._head] = default(T);
            this._head = (this._head + 1) % this._array.Length;
            this.Count--;

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T PeekTail()
        {
#if CONTRACTS_FULL
            Contract.Requires<InvalidOperationException>(this.Count > 0);
#endif

            return this._array[this._tail];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveTail()
        {
#if CONTRACTS_FULL
            Contract.Requires<InvalidOperationException>(this.Count > 0);
#endif

            var item = this._array[this._tail];
            this._array[this._tail] = default(T);
            this._tail = (this._tail - 1) % this._array.Length;
            this.Count--;
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public void SetCapacity(int capacity)
        {
#if CONTRACTS_FULL
            Contract.Requires<InvalidOperationException>(capacity >= this.Count);
#endif

            var target = new T[capacity];
            if (this.Count > 0)
            {
                if (this._head <= this._tail)
                {
                    Array.Copy(this._array, this._head, target, 0, this.Count);
                }
                else
                {
                    var headCount = this._array.Length - this._head;
                    Array.Copy(this._array, this._head, target, 0, headCount);
                    Array.Copy(this._array, 0, target, headCount, this._tail + 1);
                }

                this._head = 0;
                this._tail = this.Count - 1;
            }
            else
            {
                this._head = -1;
                this._tail = -1;
            }

            this._array = target;
        }

#region IList<T> Members

        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                index = (this._head + index) % this._array.Length;
                return this._array[index];
            }

            set
            {
                index = (this._head + index) % this._array.Length;
                this._array[index] = value;
            }
        }

#endregion

#region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

#endregion

#region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (this.Count > 0)
            {
                var current = this._head;
                while (true)
                {
                    var item = this._array[current];
                    yield return item;
                    if (current == this._tail)
                    {
                        break;
                    }

                    current = (current + 1) %this._array.Length;
                }
            }
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable<T>)this;
            return enumerable.GetEnumerator();
        }

#endregion
    }
}