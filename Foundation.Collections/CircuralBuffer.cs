using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

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
            Assert.IsValidOperation(Count < Capacity);

            if (_head == -1)
            {
                _head = 0;
                _tail = 0;
            }
            else
                _head = (_head - 1) % _array.Length;

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
            //Assert.IsTrue(this.Count < this.array.Length);

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
            Assert.IsNotNull(items);

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
            Assert.IsValidOperation(Count > 0);
            return _array[_head];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveHead()
        {
            Assert.IsValidOperation(Count > 0);

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
            Assert.IsValidOperation(Count > 0);

            return _array[_tail];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveTail()
        {
            Assert.IsValidOperation(Count > 0);

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
            Assert.IsValidOperation(capacity >= Count);

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
            var enumerable = (IEnumerable<T>) this;
            return enumerable.GetEnumerator();
        }

        #endregion
    }
}