using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DataCommander.Providers.SqlServer
{
    /// <summary>
    ///     See <see cref="http://msdn.microsoft.com/en-us/library/ms177563.aspx" />.
    /// </summary>
    internal sealed class DatabaseObjectMultipartName
    {
        private string _server;

        public DatabaseObjectMultipartName(string server, string database, string schema, string name)
        {
            _server = server;
            Database = database;
            Schema = schema;
            Name = name;
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
                    Name = parts[i];
                    i--;

                    if (i >= 0)
                    {
                        Schema = parts[i];
                        i--;

                        if (i >= 0)
                        {
                            Database = parts[i];
                            i--;

                            if (i >= 0) _server = parts[i];
                        }
                    }
                }
            }

            if (Database == null) Database = currentDatabase;

            if (string.IsNullOrEmpty(Schema)) Schema = null;

            if (Name != null)
            {
                var length = Name.Length;

                if (length > 0 && Name[0] == '[')
                {
                    Name = Name.Substring(1);
                    length--;
                }

                if (length > 0 && Name[length - 1] == ']') Name = Name.Substring(0, length - 1);
            }
        }

        public string Database { get; }
        public string Schema { get; set; }
        public string Name { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Database != null) sb.Append(Database);

            if (Schema != null)
            {
                if (sb.Length > 0) sb.Append('.');
                sb.Append(Schema);
            }

            if (sb.Length > 0) sb.Append('.');

            sb.Append(Name);
            return sb.ToString();
        }
    }
}