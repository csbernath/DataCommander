namespace DataCommander.Foundation.Diagnostics.Log
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CallerInformation
    {
        private CallerInformation(string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            this.CallerMemberName = callerMemberName;
            this.CallerFilePath = callerFilePath;
            this.CallerLineNumber = callerLineNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CallerMemberName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string CallerFilePath { get; }

        /// <summary>
        /// 
        /// </summary>
        public int CallerLineNumber { get; }

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