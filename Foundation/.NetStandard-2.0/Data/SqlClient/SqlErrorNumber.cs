namespace Foundation.Data.SqlClient
{
    public static class SqlErrorNumber
    {
        /// <summary>
        /// Transaction (Process ID %d) was deadlocked on %.*ls resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
        /// </summary>
        public const int TransactionWasDeadlocked = 1205;

        /// <summary>
        /// %d percent processed.
        /// </summary>
        public const int PercentProcessed = 3211;
    }
}