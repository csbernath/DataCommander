using System;

namespace Foundation.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly long Id;

        /// <summary>
        /// 
        /// </summary>
        public readonly string LogName;

        /// <summary>
        /// 
        /// </summary>
        public readonly DateTime CreationTime;

        /// <summary>
        /// 
        /// </summary>
        public readonly int ManagedThreadId;

        /// <summary>
        /// 
        /// </summary>
        public readonly string ThreadName;

        /// <summary>
        /// 
        /// </summary>
        public readonly string HostName;

        /// <summary>
        /// 
        /// </summary>
        public readonly string UserName;

        /// <summary>
        /// 
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// 
        /// </summary>
        public readonly LogLevel LogLevel;

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
        public LogEntry(long id, string logName, DateTime creationTime, int managedThreadId, string threadName, string userName, string hostName, string message, LogLevel logLevel)
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
    }
}