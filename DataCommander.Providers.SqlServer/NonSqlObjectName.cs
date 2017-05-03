namespace DataCommander.Providers.SqlServer
{
    internal sealed class NonSqlObjectName : IObjectName
    {
        private readonly string objectName;

        public NonSqlObjectName(string objectName)
        {
            this.objectName = objectName;
        }

        string IObjectName.UnquotedName => this.objectName;

        string IObjectName.QuotedName => this.objectName;
    }
}