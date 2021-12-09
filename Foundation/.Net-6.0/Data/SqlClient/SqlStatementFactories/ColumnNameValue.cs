using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Data.SqlClient.SqlStatementFactories
{
    public sealed class ColumnNameValue
    {
        public readonly string Name;
        public readonly string Value;

        public ColumnNameValue(string name, string value)
        {
            Assert.IsTrue(!name.IsNullOrEmpty());

            Name = name;
            Value = value;
        }
    }
}