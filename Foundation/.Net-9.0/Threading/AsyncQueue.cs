﻿using System;
using System.Collections;
using System.Threading;
using Foundation.Assertions;

namespace Foundation.Threading;

/// <summary>
/// Asynchronous producer/consumer queue handler class.
/// </summary>
public class AsyncQueue
{
    private string? _name;
    private IAsyncQueue? _asyncQueue;
    private readonly Queue _queue = new();
    private readonly AutoResetEvent _queueEvent = new(false);

    private sealed class ConsumerThread
    {
        private readonly AsyncQueue _queue;
        private readonly IConsumer _consumer;

        public ConsumerThread(
            AsyncQueue queue,
            int id,
            ThreadPriority priority)
        {
            _queue = queue;
            Thread = new WorkerThread(ThreadStart)
            {
                Name = $"Consumer({queue._name},{id})",
                Priority = priority
            };
            _consumer = _queue._asyncQueue!.CreateConsumer(Thread, id);
        }

        private void ThreadStart()
        {
            var state = _consumer.Enter();

            try
            {
                while (!Thread.IsStopRequested)
                {
                    _queue.Dequeue(this);
                }
            }
            finally
            {
                _consumer.Exit(state);
            }
        }

        public void Consume(object item) => _consumer.Consume(item);

        public WorkerThread Thread { get; }
    }

    protected AsyncQueue()
    {
    }

    /// <summary>
    /// Inherited class must call this initializer method first.
    /// </summary>
    /// <param name="name">The name of the <see cref="AsyncQueue"/></param>
    /// <param name="asyncQueue">The <see cref="IAsyncQueue"/> implementation</param>
    /// <param name="consumerCount">Number of consumers</param>
    /// <param name="priority">priority of consumer threads</param>
    protected void Initialize(
        string name,
        IAsyncQueue asyncQueue,
        int consumerCount,
        ThreadPriority priority)
    {
        Assert.IsTrue(consumerCount > 0);

        _name = name;
        _asyncQueue = asyncQueue;

        for (var id = 0; id < consumerCount; id++)
        {
            var consumerThread = new ConsumerThread(this, id, priority);
            Consumers.Add(consumerThread.Thread);
        }
    }

    /// <summary>
    /// Calls <see cref="Initialize"/> method.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asyncQueue"></param>
    /// <param name="consumerCount"></param>
    /// <param name="priority"></param>
    public AsyncQueue(
        string name,
        IAsyncQueue asyncQueue,
        int consumerCount,
        ThreadPriority priority)
    {
        Initialize(name, asyncQueue, consumerCount, priority);
    }

    /// <summary>
    /// Adds an item to the queue.
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(object item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_queue)
            _queue.Enqueue(item);

        _queueEvent.Set();
    }

    private object? Dequeue()
    {
        object? item = null;

        if (_queue.Count > 0)
        {
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.Dequeue();
                }
            }
        }

        return item;
    }

    private void Consume(ConsumerThread consumerThread, object item)
    {
        ArgumentNullException.ThrowIfNull(consumerThread);

        var args = new AsyncQueueConsumeEventArgs(item);
        var eventHandler = _asyncQueue!.BeforeConsume;

        if (eventHandler != null)
            eventHandler(this, args);

        consumerThread.Consume(item);

        eventHandler = _asyncQueue.AfterConsume;

        if (eventHandler != null)
            eventHandler(this, args);
    }

    private void Dequeue(ConsumerThread consumerThread)
    {
        var thread = consumerThread.Thread;
        WaitHandle[] waitHandles = [thread.StopRequest, _queueEvent];

        while (!thread.IsStopRequested)
        {
            var item = Dequeue();

            if (item != null)
                Consume(consumerThread, item);
            else
                WaitHandle.WaitAny(waitHandles);
        }
    }

    /// <summary>
    /// Gets the number of unconsumed items (queued items).
    /// </summary>
    public int Count => _queue.Count;

    /// <summary>
    /// Gets the consumer thread list.
    /// </summary>
    public WorkerThreadCollection Consumers { get; } = [];
}