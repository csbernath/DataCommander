namespace DataCommander.Providers.SqlServer2005
{
    internal sealed class NonSqlObjectName : IObjectName
    {
        private readonly string objectName;

        public NonSqlObjectName(string objectName)
        {
            this.objectName = objectName;
        }

        string IObjectName.UnquotedName
        {
            get
            {
                return this.objectName;
            }
        }

        string IObjectName.QuotedName
        {
            get
            {
                return this.objectName;
            }
        }
    }
}