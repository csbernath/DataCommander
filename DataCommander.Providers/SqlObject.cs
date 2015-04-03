namespace DataCommander.Providers
{
    using DataCommander.Foundation.Diagnostics;

    public sealed class SqlObject
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private string parentName;
        private readonly string parentAlias;
        private readonly SqlObjectTypes type;
        private string name;

        public SqlObject(string parentName, string parentAlias, SqlObjectTypes type, string name)
        {
            this.parentName = parentName;
            this.parentAlias = parentAlias;
            this.type = type;
            this.name = name;
        }

        public string ParentName
        {
            get
            {
                return this.parentName;
            }

            set
            {
                this.parentName = value;
            }
        }

        public string ParentAlias
        {
            get
            {
                return this.parentAlias;
            }
        }

        public SqlObjectTypes Type
        {
            get
            {
                return this.type;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }
    }
}