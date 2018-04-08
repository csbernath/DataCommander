using System.Collections.ObjectModel;
using Foundation.Assertions;

namespace Foundation.Data
{
    public sealed class OrmResult
    {
        public readonly string RecordClassName;
        public readonly ReadOnlyCollection<OrmColumn> Columns;

        public OrmResult(string recordClassName, ReadOnlyCollection<OrmColumn> columns)
        {
            Assert.IsNotNull(columns);
            RecordClassName = recordClassName;
            Columns = columns;
        }
    }
}