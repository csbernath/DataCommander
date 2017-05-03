namespace DataCommander.Providers.SqlServer
{
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// See <see cref="http://msdn.microsoft.com/en-us/library/ms177563.aspx"/>.
    /// </summary>
    internal sealed class DatabaseObjectMultipartName
    {
        private string server;

        public DatabaseObjectMultipartName(string server, string database, string schema, string name)
        {
            this.server = server;
            this.Database = database;
            this.Schema = schema;
            this.Name = name;
        }

        public DatabaseObjectMultipartName(string currentDatabase, string name)
        {
            if (name != null)
            {
                var parser = new IdentifierParser(new StringReader(name));
                var parts = parser.Parse().ToArray();

                var i = parts.Length - 1;
                var commandBuilder = new SqlCommandBuilder();

                if (i >= 0)
                {
                    this.Name = parts[i];
                    i--;

                    if (i >= 0)
                    {
                        this.Schema = parts[i];
                        i--;

                        if (i >= 0)
                        {
                            this.Database = parts[i];
                            i--;

                            if (i >= 0)
                            {
                                this.server = parts[i];
                            }
                        }
                    }
                }
            }

            if (this.Database == null)
            {
                this.Database = currentDatabase;
            }

            if (string.IsNullOrEmpty(this.Schema))
            {
                this.Schema = null;
            }

            if (this.Name != null)
            {
                var length = this.Name.Length;

                if (length > 0 && this.Name[0] == '[')
                {
                    this.Name = this.Name.Substring(1);
                    length--;
                }

                if (length > 0 && this.Name[length - 1] == ']')
                {
                    this.Name = this.Name.Substring(0, length - 1);
                }
            }
        }

        public string Database { get; set; }

        public string Schema { get; set; }

        public string Name { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Database != null)
            {
                sb.Append(this.Database);
            }

            if (this.Schema != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }
                sb.Append(this.Schema);
            }

            if (sb.Length > 0)
            {
                sb.Append('.');
            }

            sb.Append(this.Name);
            return sb.ToString();
        }
    }
}