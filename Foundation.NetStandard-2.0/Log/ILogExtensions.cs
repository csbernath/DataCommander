using Foundation.Assertions;

namespace Foundation.Log
{
    public static class LogExtensions
    {
        public static void Trace(this ILog log, CallerInformation callerInformation, string message)
        {
            Assert.IsNotNull(log);
            Assert.IsNotNull(callerInformation);

            var messageWithCallerInformation =
                $"CallerInformation: {callerInformation.CallerMemberName},{callerInformation.CallerFilePath},{callerInformation.CallerLineNumber}\r\n{message}";

            log.Trace(messageWithCallerInformation);
        }

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