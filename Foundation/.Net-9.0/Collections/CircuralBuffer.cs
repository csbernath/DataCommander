using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

/// <summary>
///     https://en.wikipedia.org/wiki/Circular_buffer
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class CircularBuffer<T> : IList<T>
{
    public CircularBuffer(int capacity)
    {
        SetCapacity(capacity);
    }

    public int Capacity => _array.Length;

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        if (Count > 0)
        {
            int current = _head;
            while (true)
            {
                T item = _array[current];
                yield return item;
                if (current == _tail) break;

                current = (current + 1) % _array.Length;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        IEnumerable<T> enumerable = (IEnumerable<T>) this;
        return enumerable.GetEnumerator();
    }

    public void AddHead(T item)
    {
        Assert.IsValidOperation(Count < Capacity);

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

    private void AddTail(T item)
    {
        Assert.IsTrue(Count < _array.Length);

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

    public void AddTail(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        foreach (T item in items)
            AddTail(item);
    }

    public T PeekHead()
    {
        Assert.IsValidOperation(Count > 0);
        return _array[_head];
    }

    public T RemoveHead()
    {
        Assert.IsValidOperation(Count > 0);

        T item = _array[_head];
        _array[_head] = default;
        _head = (_head + 1) % _array.Length;
        Count--;

        return item;
    }

    public T PeekTail()
    {
        Assert.IsValidOperation(Count > 0);

        return _array[_tail];
    }

    public T RemoveTail()
    {
        Assert.IsValidOperation(Count > 0);

        T item = _array[_tail];
        _array[_tail] = default;
        _tail = (_tail - 1) % _array.Length;
        Count--;
        return item;
    }

    public void SetCapacity(int capacity)
    {
        Assert.IsValidOperation(capacity >= Count);

        T[] target = new T[capacity];
        if (Count > 0)
        {
            if (_head <= _tail)
            {
                Array.Copy(_array, _head, target, 0, Count);
            }
            else
            {
                int headCount = _array.Length - _head;
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

    private T[] _array;
    private int _head;
    private int _tail;

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

    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    void ICollection<T>.Clear() => throw new NotImplementedException();
    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public int Count { get; private set; }

    bool ICollection<T>.IsReadOnly => false;

    bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
}