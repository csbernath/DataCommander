namespace DataCommander.Foundation.Data.SqlClient
{
    using System;

    internal sealed class SqLoglCommandExecution
    {
        private readonly Int32 commandNo;
        private Int32 executionNo;

        public SqLoglCommandExecution(Int32 commandNo)
        {
            this.commandNo = commandNo;
            this.executionNo = 1;
        }

        public Int32 CommandNo
        {
            get
            {
                return this.commandNo;
            }
        }

        public Int32 ExecutionNo
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