namespace DataCommander.Providers
{
    public sealed class ObjectName : IObjectName
    {
        private readonly string objectName;

        public ObjectName(string objectName)
        {
            this.objectName = objectName;
        }

        string IObjectName.UnquotedName => this.objectName;

        string IObjectName.QuotedName => this.objectName;
    }
}