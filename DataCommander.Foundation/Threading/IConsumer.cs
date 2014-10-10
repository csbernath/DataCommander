namespace DataCommander.Foundation.Threading
{
    using System;

    /// <summary>
    /// AsyncQueue implementors must implement this interface.
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// Called when Consumer's thread is started and before the first item is consumed by this Consumer.
        /// </summary>
        /// <returns>A state object can be returned. <see cref="Exit"/> is called with this state object.</returns>
        Object Enter();

        /// <summary>
        /// Called when an item from the queue is to be consumed by this consumer.
        /// </summary>
        /// <param name="item"></param>
        void Consume(Object item);

        /// <summary>
        /// Called when the thread is terminating.
        /// </summary>
        /// <param name="state">The object returned by <see cref="Enter"/>.</param>
        void Exit(Object state);
    }
}