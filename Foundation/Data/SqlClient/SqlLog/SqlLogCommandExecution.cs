namespace Foundation.Data.SqlClient.SqlLog
{
    internal sealed class SqLoglCommandExecution
    {
        public SqLoglCommandExecution(int commandNo)
        {
            CommandNo = commandNo;
            ExecutionNo = 1;
        }

        public int CommandNo { get; }

        public int ExecutionNo { get; set; }
    }
}