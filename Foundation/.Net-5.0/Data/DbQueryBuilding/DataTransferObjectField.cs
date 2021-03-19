
namespace Foundation.Data.DbQueryBuilding
{
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
}