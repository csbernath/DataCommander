namespace DataCommander.Providers
{
    using DataCommander.Foundation.Diagnostics;
    using Foundation.Diagnostics.Log;

    public sealed class SqlObject
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();

        public SqlObject(string parentName, string parentAlias, SqlObjectTypes type, string name)
        {
            this.ParentName = parentName;
            this.ParentAlias = parentAlias;
            this.Type = type;
            this.Name = name;
        }

        public string ParentName { get; set; }

        public string ParentAlias { get; }

        public SqlObjectTypes Type { get; }

        public string Name { get; set; }
    }
}