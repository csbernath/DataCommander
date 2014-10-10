namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class LogEntry
    {
        #region Private Fields
        private readonly Int64 id;
        private readonly String logName;
        private DateTime creationTime;
        private Int32 managedThreadId;
        private String threadName;
        private String userName;
        private String hostName;
        private String message;
        private LogLevel logLevel;
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
            Int64 id,
            String logName,
            DateTime creationTime,
            Int32 managedThreadId,
            String threadName,
            String userName,
            String hostName,
            String message,
            LogLevel logLevel)
        {
            this.id = id;
            this.logName = logName;
            this.creationTime = creationTime;
            this.managedThreadId = managedThreadId;
            this.threadName = threadName;
            this.userName = userName;
            this.hostName = hostName;
            this.message = message;
            this.logLevel = logLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String LogName
        {
            get
            {
                return this.logName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return this.creationTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ManagedThreadId
        {
            get
            {
                return this.managedThreadId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ThreadName
        {
            get
            {
                return this.threadName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String HostName
        {
            get
            {
                return this.hostName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String UserName
        {
            get
            {
                return this.userName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Message
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LogLevel LogLevel
        {
            get
            {
                return this.logLevel;
            }
        }
    }
}