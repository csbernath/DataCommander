using DataCommander.Providers2;

namespace DataCommander.Providers.SqlServer2
{
    internal sealed class NonSqlObjectName : IObjectName
    {
        private readonly string _objectName;

        public NonSqlObjectName(string objectName) => _objectName = objectName;

        string IObjectName.UnquotedName => _objectName;
        string IObjectName.QuotedName => _objectName;
    }
}