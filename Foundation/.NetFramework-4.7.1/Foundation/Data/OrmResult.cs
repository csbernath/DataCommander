using System.Collections.Generic;

namespace Foundation.Data
{
    public sealed class OrmResult
    {
        public readonly string RecordTypeName;
        public readonly IReadOnlyCollection<OrmColumn> Columns;

        public OrmResult(string recordTypeName, IReadOnlyCollection<OrmColumn> columns)
        {
            RecordTypeName = recordTypeName;
            Columns = columns;
        }
    }
}