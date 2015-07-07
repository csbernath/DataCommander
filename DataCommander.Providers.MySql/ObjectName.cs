namespace DataCommander.Providers.MySql
{
    using System.Text;

    internal sealed class ObjectName : IObjectName
    {
        private readonly string databaseName;
        private readonly string objectName;

        public ObjectName(string databaseName, string objectName)
        {
            this.databaseName = databaseName;
            this.objectName = objectName;
        }

        string IObjectName.UnquotedName
        {
            get
            {
                var sb = new StringBuilder();
                if (this.databaseName != null)
                {
                    sb.Append(this.databaseName);
                    sb.Append('.');
                }

                sb.Append(this.objectName);

                return sb.ToString();
            }
        }

        string IObjectName.QuotedName
        {
            get
            {
                // TODO

                var sb = new StringBuilder();
                if (this.databaseName != null)
                {
                    sb.Append(this.databaseName);
                    sb.Append('.');
                }

                sb.Append(this.objectName);

                return sb.ToString();
            }
        }
    }
}