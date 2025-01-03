using System;

namespace Foundation.Threading;

/// <summary>
/// AsyncQueue implementors must implement this interface.
/// </summary>
public interface IAsyncQueue
{
    /// <summary>
    /// Creates an <see cref=" IConsumer"/> instance.
    /// </summary>
    /// <param name="thread">The consumer will run in this thread</param>
    /// <param name="id">The id of the consumer</param>
    IConsumer CreateConsumer(WorkerThread thread, int id);

    EventHandler<AsyncQueueConsumeEventArgs> BeforeConsume { get; }

    EventHandler<AsyncQueueConsumeEventArgs> AfterConsume { get; }
}