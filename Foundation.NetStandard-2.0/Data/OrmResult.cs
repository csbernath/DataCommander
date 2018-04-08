using System.Collections.ObjectModel;

namespace Foundation.Data
{
    public sealed class OrmResult
    {
        public readonly string RecordClassName;
        public readonly ReadOnlyCollection<OrmColumn> Columns;

        public OrmResult(string recordClassName, ReadOnlyCollection<OrmColumn> columns)
        {
            RecordClassName = recordClassName;
            Columns = columns;
        }
    }
}