namespace DataCommander.Foundation.Data.SqlClient.SqlLog
{
    internal sealed class SqLoglCommandExecution
    {
        public SqLoglCommandExecution(int commandNo)
        {
            this.CommandNo = commandNo;
            this.ExecutionNo = 1;
        }

        public int CommandNo { get; }

        public int ExecutionNo { get; set; }
    }
}