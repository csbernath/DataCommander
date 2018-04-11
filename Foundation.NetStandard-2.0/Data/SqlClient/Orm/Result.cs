using System.Collections.ObjectModel;

namespace Foundation.Data.SqlClient.Orm
{
    public class Result
    {
        public readonly string Name;
        public readonly ReadOnlyCollection<Field> Fields;

        public Result(string name, ReadOnlyCollection<Field> fields)
        {
            Name = name;
            Fields = fields;
        }
    }
}