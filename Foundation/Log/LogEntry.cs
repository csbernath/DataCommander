using System;

namespace Foundation.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEntry
    {
        #region Private Fields

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logName"></param>
        /// <param name="creationTime"></param>
        /// <param name="managedThreadId"></param>
        /// <param name="threadName"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public LogEntry(
            long id,
            string logName,
            DateTime creationTime,
            int managedThreadId,
            string threadName,
            string userName,
            string hostName,
            string message,
            LogLevel logLevel)
        {
            Id = id;
            LogName = logName;
            CreationTime = creationTime;
            ManagedThreadId = managedThreadId;
            ThreadName = threadName;
            UserName = userName;
            HostName = hostName;
            Message = message;
            LogLevel = logLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public string LogName { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ManagedThreadId { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ThreadName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 
        /// </summary>
        public LogLevel LogLevel { get; }
    }
}