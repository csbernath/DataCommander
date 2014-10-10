namespace DataCommander.Providers.Tfs
{
    internal sealed class ObjectName : IObjectName
    {
        private string objectName;

        public ObjectName(string obejctName)
        {
            this.objectName = obejctName;
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