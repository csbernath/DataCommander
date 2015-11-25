using System;
using System.Diagnostics.Contracts;

namespace DataCommander.Foundation.Diagnostics
{
    using Log;

    /// <summary>
    /// 
    /// </summary>
    public static class ILogExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="callerInformation"></param>
        /// <param name="message"></param>
        public static void Trace(
            this ILog log,
            CallerInformation callerInformation,
            string message)
        {
            Contract.Requires<ArgumentNullException>(log != null);
            Contract.Requires<ArgumentNullException>(callerInformation != null);

            string messageWithCallerInformation =
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
            string message = string.Format(format, args);
            log.Trace(callerInformation, message);
        }
    }
}