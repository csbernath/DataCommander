namespace DataCommander.Providers
{
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;

    public sealed class FourPartName
    {
        private string server;
        private string database;
        private string owner;
        private string name;

        public FourPartName(string server, string database, string owner, string name)
        {
            this.server = server;
            this.database = database;
            this.owner = owner;
            this.name = name;
        }

        public FourPartName(string currentDatabase, string name)
        {
            if (name != null)
            {
                var parser = new IdentifierParser(new StringReader(name));
                var parts = parser.Parse().ToArray();

                int i = parts.Length - 1;
                var commandBuilder = new SqlCommandBuilder();

                if (i >= 0)
                {
                    this.name = parts[i];
                    i--;

                    if (i >= 0)
                    {
                        this.owner = parts[i];
                        i--;

                        if (i >= 0)
                        {
                            this.database = parts[i];
                            i--;

                            if (i >= 0)
                            {
                                this.server = parts[i];
                            }
                        }
                    }
                }
            }

            if (database == null)
            {
                database = currentDatabase;
            }

            if (string.IsNullOrEmpty(owner))
            {
                owner = null;
            }

            if (this.name != null)
            {
                int length = this.name.Length;

                if (length > 0 && this.name[0] == '[')
                {
                    this.name = this.name.Substring(1);
                    length--;
                }

                if (length > 0 && this.name[length - 1] == ']')
                {
                    this.name = this.name.Substring(0, length - 1);
                }
            }
        }

        public string Database
        {
            get
            {
                return this.database;
            }

            set
            {
                this.database = value;
            }
        }

        public string Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.database != null)
            {
                sb.Append( this.database );
            }

            if (this.owner != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append( '.' );
                }
                sb.Append( this.owner );
            }

            if (sb.Length > 0)
            {
                sb.Append( '.' );
            }

            sb.Append( this.name );
            return sb.ToString();
        }
    }
}