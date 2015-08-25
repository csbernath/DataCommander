namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BeforeExecuteCommandEventArgs : LoggedEventArgs
    {
        private readonly LoggedDbCommandInfo command;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public BeforeExecuteCommandEventArgs(LoggedDbCommandInfo command)
        {
            this.command = command;
        }

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandInfo Command
        {
            get
            {
                return this.command;
            }
        }
    }
}