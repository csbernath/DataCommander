
namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class SqLoglCommandExecution(int commandNo)
{
    public int CommandNo { get; } = commandNo;

    public int ExecutionNo { get; set; } = 1;
}