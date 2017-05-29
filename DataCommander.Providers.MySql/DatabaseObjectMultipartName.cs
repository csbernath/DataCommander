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
                    this.Name = nameParts[i];
                    i--;


                    if (i >= 0)
                    {
                        this.Database = nameParts[i];
                    }
                }
            }

            if (this.Database == null)
            {
                this.Database = currentDatabase;
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

        public string Name { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Database != null)
            {
                sb.Append(this.Database);
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