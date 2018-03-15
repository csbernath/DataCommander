using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Log
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="callerInformation"></param>
        /// <param name="message"></param>
        public static void Trace(this ILog log, CallerInformation callerInformation, string message)
        {
            FoundationContract.Requires<ArgumentNullException>(log != null);
            FoundationContract.Requires<ArgumentNullException>(callerInformation != null);

            var messageWithCallerInformation =
                $"CallerInformation: {callerInformation.CallerMemberName},{callerInformation.CallerFilePath},{callerInformation.CallerLineNumber}\r\n{message}";

            log.Trace(messageWithCallerInformation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="callerInformation"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Trace(
            this ILog log,
            CallerInformation callerInformation,
            string format,
            params object[] args)
        {
            var message = string.Format(format, args);
            log.Trace(callerInformation, message);
        }
    }
}