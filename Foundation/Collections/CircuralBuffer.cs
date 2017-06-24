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
            SetCapacity(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity => _array.Length;

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

            if (_head == -1)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head - 1) % _array.Length;
            }

            _array[_head] = item;
            Count++;
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

            if (_head == -1)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _tail = (_tail + 1) % _array.Length;
            }

            _array[_tail] = item;
            Count++;
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
                AddTail(item);
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
            return _array[_head];
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

            var item = _array[_head];
            _array[_head] = default(T);
            _head = (_head + 1) % _array.Length;
            Count--;

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

            return _array[_tail];
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

            var item = _array[_tail];
            _array[_tail] = default(T);
            _tail = (_tail - 1) % _array.Length;
            Count--;
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
            if (Count > 0)
            {
                if (_head <= _tail)
                {
                    Array.Copy(_array, _head, target, 0, Count);
                }
                else
                {
                    var headCount = _array.Length - _head;
                    Array.Copy(_array, _head, target, 0, headCount);
                    Array.Copy(_array, 0, target, headCount, _tail + 1);
                }

                _head = 0;
                _tail = Count - 1;
            }
            else
            {
                _head = -1;
                _tail = -1;
            }

            _array = target;
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
                index = (_head + index) % _array.Length;
                return _array[index];
            }

            set
            {
                index = (_head + index) % _array.Length;
                _array[index] = value;
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
            if (Count > 0)
            {
                var current = _head;
                while (true)
                {
                    var item = _array[current];
                    yield return item;
                    if (current == _tail)
                    {
                        break;
                    }

                    current = (current + 1) % _array.Length;
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