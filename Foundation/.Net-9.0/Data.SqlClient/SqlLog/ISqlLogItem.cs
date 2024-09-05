
namespace Foundation.Data.SqlClient.SqlLog;

internal interface ISqlLogItem
{
    string CommandText
    {
        get;
    }
}