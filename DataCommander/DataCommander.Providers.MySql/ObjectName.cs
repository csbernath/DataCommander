using System.Text;

namespace DataCommander.Providers.MySql
{
    internal sealed class ObjectName : IObjectName
    {
        private readonly string _databaseName;
        private readonly string _objectName;

        public ObjectName(string databaseName, string objectName)
        {
            this._databaseName = databaseName;
            this._objectName = objectName;
        }

        string IObjectName.UnquotedName
        {
            get
            {
                var sb = new StringBuilder();
                if (_databaseName != null)
                {
                    sb.Append(_databaseName);
                    sb.Append('.');
                }

                sb.Append(_objectName);

                return sb.ToString();
            }
        }

        string IObjectName.QuotedName
        {
            get
            {
                // TODO

                var sb = new StringBuilder();
                if (_databaseName != null)
                {
                    sb.Append(_databaseName);
                    sb.Append('.');
                }

                sb.Append(_objectName);

                return sb.ToString();
            }
        }
    }
}