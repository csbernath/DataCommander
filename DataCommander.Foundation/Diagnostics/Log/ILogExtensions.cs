namespace DataCommander.Foundation.Diagnostics.Log
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(log != null);
            Contract.Requires<ArgumentNullException>(callerInformation != null);
#endif

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