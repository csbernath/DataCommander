namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DataTransferObjectField(string name, string type)
{
    public readonly string Name = name;
    public readonly string Type = type;
}