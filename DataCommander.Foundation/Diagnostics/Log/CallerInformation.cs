namespace DataCommander.Foundation.Diagnostics.Log
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CallerInformation
    {
        private readonly string callerMemberName;
        private readonly string callerFilePath;
        private readonly int callerLineNumber;

        private CallerInformation(string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            this.callerMemberName = callerMemberName;
            this.callerFilePath = callerFilePath;
            this.callerLineNumber = callerLineNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CallerMemberName => this.callerMemberName;

        /// <summary>
        /// 
        /// </summary>
        public string CallerFilePath => this.callerFilePath;

        /// <summary>
        /// 
        /// </summary>
        public int CallerLineNumber => this.callerLineNumber;

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