namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T> : IList<T>
    {
        private T[] array;
        private int head;
        private int tail;
        private int count;

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
        public int Capacity
        {
            get
            {
                return this.array.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void AddHead(T item)
        {
            Contract.Requires<ArgumentException>(this.Count < this.Capacity);

            if (this.head == -1)
            {
                this.head = 0;
                this.tail = 0;
            }
            else
            {
                this.head = (this.head - 1) % this.array.Length;
            }

            this.array[this.head] = item;
            this.count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void AddTail(T item)
        {
            Contract.Assert(this.count < this.array.Length);

            if (this.head == -1)
            {
                this.head = 0;
                this.tail = 0;
            }
            else
            {
                this.tail = (this.tail + 1) % this.array.Length;
            }

            this.array[this.tail] = item;
            this.count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void AddTail(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

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
            Contract.Requires<InvalidOperationException>(this.Count > 0);
            return this.array[this.head];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveHead()
        {
            Contract.Requires<InvalidOperationException>(this.Count > 0);

            var item = this.array[this.head];
            this.array[this.head] = default(T);
            this.head = (this.head + 1) % this.array.Length;
            this.count--;

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T PeekTail()
        {
            Contract.Requires<InvalidOperationException>(this.Count > 0);

            return this.array[this.tail];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveTail()
        {
            Contract.Requires<InvalidOperationException>(this.Count > 0);

            var item = this.array[this.tail];
            this.array[this.tail] = default(T);
            this.tail = (this.tail - 1) % this.array.Length;
            this.count--;
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public void SetCapacity(int capacity)
        {
            Contract.Requires<InvalidOperationException>(capacity >= this.Count);

            var target = new T[capacity];
            if (this.count > 0)
            {
                if (this.head <= this.tail)
                {
                    Array.Copy(this.array, this.head, target, 0, this.count);
                }
                else
                {
                    int headCount = this.array.Length - this.head;
                    Array.Copy(this.array, this.head, target, 0, headCount);
                    Array.Copy(this.array, 0, target, headCount, this.tail + 1);
                }

                this.head = 0;
                this.tail = this.count - 1;
            }
            else
            {
                this.head = -1;
                this.tail = -1;
            }

            this.array = target;
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
                index = (this.head + index) % this.array.Length;
                return this.array[index];
            }

            set
            {
                index = (this.head + index) % this.array.Length;
                this.array[index] = value;
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
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (this.count > 0)
            {
                int current = this.head;
                while (true)
                {
                    var item = this.array[current];
                    yield return item;
                    if (current == this.tail)
                    {
                        break;
                    }

                    current = (current + 1) %this.array.Length;
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