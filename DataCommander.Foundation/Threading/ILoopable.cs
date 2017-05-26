using System;

namespace Foundation.Threading
{
    /// <summary>
    /// LoopThread implementors must implement this interface
    /// </summary>
    public interface ILoopable
    {
        /// <summary>
        /// This method is called before starting the loop.
        /// </summary>
        void First(Exception exception);

        /// <summary>thread.IsRunning
        /// This method is called in a loop.
        /// If an exception is occured in the loop the loop restarts with <see cref="First"/>.
        /// </summary>
        void Next();

        /// <summary>
        /// This method is called after terminating the loop.
        /// </summary>
        void Last();
    }
}