namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DataTransferObjectField
{
    public readonly string Name;
    public readonly string Type;

    public DataTransferObjectField(string name, string type)
    {
        Name = name;
        Type = type;
    }
}