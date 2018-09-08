using System.Runtime.CompilerServices;

namespace Foundation.Log
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CallerInformation
    {
        public readonly string CallerMemberName;
        public readonly string CallerFilePath;
        public readonly int CallerLineNumber;

        private CallerInformation(string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static CallerInformation Get(
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new CallerInformation(callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}