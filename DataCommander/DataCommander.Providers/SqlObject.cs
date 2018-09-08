using Foundation.Log;

namespace DataCommander.Providers
{
    public sealed class SqlObject
    {
        private static ILog _log = LogFactory.Instance.GetCurrentTypeLog();

        public SqlObject(string parentName, string parentAlias, SqlObjectTypes type, string name)
        {
            ParentName = parentName;
            ParentAlias = parentAlias;
            Type = type;
            Name = name;
        }

        public string ParentName { get; set; }
        public string ParentAlias { get; }
        public SqlObjectTypes Type { get; }
        public string Name { get; set; }
    }
}