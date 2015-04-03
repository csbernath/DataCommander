namespace DataCommander.Foundation.Data.SqlClient
{
    internal sealed class SqLoglCommandExecution
    {
        private readonly int commandNo;
        private int executionNo;

        public SqLoglCommandExecution(int commandNo)
        {
            this.commandNo = commandNo;
            this.executionNo = 1;
        }

        public int CommandNo
        {
            get
            {
                return this.commandNo;
            }
        }

        public int ExecutionNo
        {
            get
            {
                return this.executionNo;
            }

            set
            {
                this.executionNo = value;
            }
        }
    }
}