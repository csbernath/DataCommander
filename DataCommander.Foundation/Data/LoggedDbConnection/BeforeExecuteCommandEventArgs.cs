namespace DataCommander.Foundation.Data
{

#if FOUNDATION_3_5

#else

#endif

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
        public BeforeExecuteCommandEventArgs( LoggedDbCommandInfo command )
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
