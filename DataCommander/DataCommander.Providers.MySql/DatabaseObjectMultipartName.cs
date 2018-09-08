namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;

    internal sealed class DatabaseObjectMultipartName
    {
        public DatabaseObjectMultipartName(string currentDatabase, List<string> nameParts)
        {
            if (nameParts != null)
            {
                var i = nameParts.Count - 1;
                var commandBuilder = new SqlCommandBuilder();

                if (i >= 0)
                {
                    Name = nameParts[i];
                    i--;


                    if (i >= 0)
                    {
                        Database = nameParts[i];
                    }
                }
            }

            if (Database == null)
            {
                Database = currentDatabase;
            }

            if (Name != null)
            {
                var length = Name.Length;

                if (length > 0 && Name[0] == '[')
                {
                    Name = Name.Substring(1);
                    length--;
                }

                if (length > 0 && Name[length - 1] == ']')
                {
                    Name = Name.Substring(0, length - 1);
                }
            }
        }

        public string Database { get; set; }

        public string Name { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Database != null)
            {
                sb.Append(Database);
            }

            if (sb.Length > 0)
            {
                sb.Append('.');
            }

            sb.Append(Name);
            return sb.ToString();
        }
    }
}